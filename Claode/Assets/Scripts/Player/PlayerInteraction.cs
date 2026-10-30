using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private float _interactRadius = 2f;

    private void OnEnable() => _interactAction.action.Enable();

    private void Update()
    {
        // Interact 액션은 Hold로 설정되어 있어, 꾹 누르기가 완료된 프레임에만 performed 된다
        if (!_interactAction.action.WasPerformedThisFrame())
        {
            return;
        }

        RemoveNearbyDeadEnemy();
    }

    private void OnDisable() => _interactAction.action.Disable();

    private void RemoveNearbyDeadEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _interactRadius, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        foreach (Collider nearby in colliders)
        {
            Enemy enemy = nearby.GetComponentInParent<Enemy>();
            if (enemy != null && enemy.IsDead)
            {
                Destroy(enemy.gameObject);
                return;
            }
        }
    }
}
