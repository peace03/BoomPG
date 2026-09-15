using BoomPG.Core.Logging;
using UnityEngine;

namespace BoomPG.Gameplay.Camera
{
    /// <summary>마우스 회전을 누적해 대상을 구면 좌표로 추적한다.</summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _heightOffset = 1.6f;
        [SerializeField] private float _sensitivity = 0.15f;

        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            if (_target == null)
            {
                GameLog.Error("Camera", $"{name} 추적 대상 누락");
                enabled = false;
                return;
            }

            _yaw = transform.eulerAngles.y;
            _pitch = Mathf.Clamp(Mathf.DeltaAngle(0f, transform.eulerAngles.x), -40f, 70f);
        }

        private void LateUpdate()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 focus = _target.position + Vector3.up * _heightOffset;
            transform.SetPositionAndRotation(focus - rotation * Vector3.forward * _distance, rotation);
        }

        /// <summary>마우스 델타(픽셀)를 감도(도/픽셀)로 회전에 반영한다.</summary>
        public void AddLookInput(Vector2 delta)
        {
            _yaw += delta.x * _sensitivity;
            _pitch = Mathf.Clamp(_pitch - delta.y * _sensitivity, -40f, 70f);
        }
    }
}
