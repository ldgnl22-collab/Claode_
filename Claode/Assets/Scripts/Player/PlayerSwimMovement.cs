using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSwimMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _swimForce = 10f;
    [SerializeField] private float _waterDamping = 2f;

    private Rigidbody _rigidbody;
    private Vector3 _swimDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // 물속처럼 떠 있고, 입력이 없으면 서서히 멈추도록 한다
        _rigidbody.useGravity = false;
        _rigidbody.linearDamping = _waterDamping;
        _rigidbody.freezeRotation = true;
    }

    private void OnEnable() => _moveAction.action.Enable();

    private void Update()
    {
        Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

        // 카메라가 바라보는 방향(위아래 포함)을 앞으로 삼는다
        _swimDirection = _cameraTransform.forward * moveInput.y + _cameraTransform.right * moveInput.x;
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(Vector3.ClampMagnitude(_swimDirection, 1f) * _swimForce, ForceMode.Acceleration);
    }

    private void OnDisable() => _moveAction.action.Disable();
}
