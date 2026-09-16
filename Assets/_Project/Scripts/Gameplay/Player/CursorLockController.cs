using UnityEngine;
using UnityEngine.InputSystem;

namespace BoomPG.Gameplay.Player
{
    /// <summary>씬 구성을 바꾸지 않고 플레이 검증 중 커서를 제어한다.</summary>
    public class CursorLockController : MonoBehaviour
    {
        /// <summary>커서가 화면 중앙에 잠겨 게임 입력을 받는 상태인가.</summary>
        public static bool IsLocked => Cursor.lockState == CursorLockMode.Locked;

        private void Awake()
        {
            // 플레이 직후에도 인스펙터를 조작할 수 있도록 잠금을 해제한다.
            UnlockCursor();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                UnlockCursor();
            }
            else if (!IsLocked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                UnlockCursor();
            }
        }

        private void OnDestroy()
        {
            UnlockCursor();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            // 테스트 편의 장치이므로 씬 구성을 바꾸지 않고 자동으로 생성한다.
            var host = new GameObject("[CursorLock]");
            host.AddComponent<CursorLockController>();
            Object.DontDestroyOnLoad(host);
        }

        private static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
