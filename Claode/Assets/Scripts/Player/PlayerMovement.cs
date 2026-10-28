using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private const float MinInputSqrMagnitude = 0.0001f;

    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void OnEnable() => _moveAction.action.Enable();

    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // 입력 Vector2(x, y)를 월드 XZ 평면에 대응시킨다
        Vector3 horizontalVelocity = new Vector3(_moveInput.x, 0f, _moveInput.y) * _moveSpeed;

        // 중력 낙하가 유지되도록 수직 속도는 그대로 둔다
        _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, _rigidbody.linearVelocity.y, horizontalVelocity.z);

        RotateTowards(horizontalVelocity);
    }

    private void OnDisable() => _moveAction.action.Disable();

    private void RotateTowards(Vector3 direction)
    {
        // 입력이 없으면 마지막으로 바라보던 방향을 유지한다
        if (direction.sqrMagnitude < MinInputSqrMagnitude)
        {
            return;
        }

        _rigidbody.MoveRotation(Quaternion.LookRotation(direction));
    }
}
