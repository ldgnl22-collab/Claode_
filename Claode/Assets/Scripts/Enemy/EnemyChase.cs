using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _chaseSpeed = 2f;

    private Enemy _enemy;

    private void Awake() => _enemy = GetComponent<Enemy>();

    private void Update()
    {
        if (_enemy.IsDead)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _target.position, _chaseSpeed * Time.deltaTime);
    }
}
