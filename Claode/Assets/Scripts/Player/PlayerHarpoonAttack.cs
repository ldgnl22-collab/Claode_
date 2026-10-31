using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerHarpoonAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference _attackAction;
    [FormerlySerializedAs("_harpoon")]
    [SerializeField] private Harpoon _harpoonPrefab;
    [SerializeField] private Transform _firePoint;

    private Harpoon _harpoon;

    // 프리팹 원본은 씬에 존재하지 않으므로, 실제로 날아갈 작살은 씬에 복제해 사용한다
    private void Awake() => _harpoon = Instantiate(_harpoonPrefab);

    private void OnEnable() => _attackAction.action.Enable();

    private void Update()
    {
        // 작살이 손에 돌아와 있을 때만 다시 쏠 수 있다
        if (!_attackAction.action.WasPressedThisFrame() || !_harpoon.IsReady)
        {
            return;
        }

        _harpoon.Launch(_firePoint.position, _firePoint.forward, _firePoint);
    }

    private void OnDisable() => _attackAction.action.Disable();
}
