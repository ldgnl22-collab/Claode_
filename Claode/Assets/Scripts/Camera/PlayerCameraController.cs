using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private Transform _target;
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _sensitivity = 0.1f;
    [SerializeField] private float _minPitch = -70f;
    [SerializeField] private float _maxPitch = 70f;

    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;
    }

    private void OnEnable()
    {
        _lookAction.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        // 마우스 델타는 이미 프레임당 이동량이므로 deltaTime을 곱하지 않는다
        Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();
        _yaw += lookInput.x * _sensitivity;
        _pitch = Mathf.Clamp(_pitch - lookInput.y * _sensitivity, _minPitch, _maxPitch);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        transform.SetPositionAndRotation(_target.position - rotation * Vector3.forward * _distance, rotation);
    }

    private void OnDisable()
    {
        _lookAction.action.Disable();
        Cursor.lockState = CursorLockMode.None;
    }
}
