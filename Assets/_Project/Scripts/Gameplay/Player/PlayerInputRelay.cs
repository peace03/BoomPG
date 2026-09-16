using BoomPG.Core.Logging;
using BoomPG.Gameplay.Camera;
using BoomPG.Gameplay.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoomPG.Gameplay.Player
{
    /// <summary>입력을 읽어 카메라와 플레이어 컴포넌트에 전달한다.</summary>
    [DefaultExecutionOrder(-300)]
    public class PlayerInputRelay : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _actions;
        [SerializeField] private ThirdPersonCamera _camera;
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private JetpackController _jetpack;
        [SerializeField] private RpgLauncher _launcher;

        private InputActionMap _playerMap;
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _attack;
        private InputAction _reload;

        private void Awake()
        {
            if (_motor == null) _motor = GetComponent<PlayerMotor>();
            if (_jetpack == null) _jetpack = GetComponent<JetpackController>();
            if (_launcher == null) _launcher = GetComponent<RpgLauncher>();
            if (_actions == null || _camera == null || _motor == null ||
                _jetpack == null || _launcher == null)
            {
                GameLog.Error("Player", $"{name} 입력 참조 누락");
                enabled = false;
                return;
            }

            // 필수 맵 누락을 예외가 아닌 일회성 오류로 보고하기 위해 먼저 검사한다.
            if (_actions.FindActionMap("Player", false) == null)
            {
                GameLog.Error("Player", "Player 액션 맵 누락");
                enabled = false;
                return;
            }

            _playerMap = _actions.FindActionMap("Player", true);
            _move = _playerMap.FindAction("Move");
            _look = _playerMap.FindAction("Look");
            _jump = _playerMap.FindAction("Jump");
            _attack = _playerMap.FindAction("Attack");
            _reload = _playerMap.FindAction("Reload");
            if (_move == null || _look == null || _jump == null || _attack == null || _reload == null)
            {
                GameLog.Error("Player", "Player 필수 액션 누락");
                enabled = false;
                _playerMap = null;
            }
        }

        private void OnEnable()
        {
            _playerMap?.Enable();
        }

        private void Update()
        {
            // 커서가 풀린 동안에는 입력을 중계하지 않는다.
            // 그렇지 않으면 인스펙터를 만지는 사이에 시점이 돌아간다.
            if (!CursorLockController.IsLocked)
            {
                _motor.SetMoveInput(Vector3.zero);
                _jetpack.SetThrustInput(false, Vector3.zero);
                return;
            }

            Vector2 move = _move.ReadValue<Vector2>();
            Vector3 forward = _camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = _camera.transform.right;
            right.y = 0f;
            right.Normalize();
            Vector3 worldDirection = (forward * move.y + right * move.x).normalized;
            _motor.SetMoveInput(worldDirection);
            _camera.AddLookInput(_look.ReadValue<Vector2>());
            if (_jump.WasPressedThisFrame())
            {
                _motor.RequestJump();
            }

            _jetpack.SetThrustInput(_jump.IsPressed(), worldDirection);
            if (_attack.WasPressedThisFrame())
            {
                _launcher.TryFire(_camera.transform.position, _camera.transform.forward);
            }

            if (_reload.WasPressedThisFrame())
            {
                _launcher.RequestReload();
            }
        }

        private void OnDisable()
        {
            _playerMap?.Disable();
            if (_motor != null) _motor.SetMoveInput(Vector3.zero);
            if (_jetpack != null) _jetpack.SetThrustInput(false, Vector3.zero);
        }
    }
}
