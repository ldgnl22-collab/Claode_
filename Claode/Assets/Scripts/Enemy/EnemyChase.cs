using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _chaseSpeed = 2f;
    [SerializeField] private float _detectRange = 10f;

    private Enemy _enemy;

    private void Awake() => _enemy = GetComponent<Enemy>();

    private void Update()
    {
        if (_enemy.IsDead)
        {
            return;
        }

        // 감지 거리 밖이면 멈추고, 다시 들어오면 추적을 재개한다 (제곱 거리로 비교해 제곱근 계산을 피한다)
        if ((_target.position - transform.position).sqrMagnitude > _detectRange * _detectRange)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _target.position, _chaseSpeed * Time.deltaTime);
    }
}
