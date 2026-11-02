using UnityEngine;

public class Harpoon : MonoBehaviour
{
    private enum HarpoonState
    {
        Ready,
        Flying,
        Stuck,
        Returning
    }

    [SerializeField] private float _flySpeed = 30f;
    [SerializeField] private float _returnSpeed = 20f;
    [SerializeField] private float _maxDistance = 20f;
    [SerializeField] private float _stuckDuration = 3f;
    [SerializeField] private LayerMask _hitLayers = ~0;
    [SerializeField] private float _mashGain = 0.2f;
    [SerializeField] private float _gaugeDecayRate = 0.3f;

    private HarpoonState _state = HarpoonState.Ready;
    private Transform _returnTarget;
    private float _traveledDistance;
    private float _stuckTimer;
    private float _mashGauge;
    private Enemy _draggedEnemy;
    private Vector3 _stuckLocalPosition;
    private Quaternion _stuckLocalRotation;
    private Vector3 _dragOffset;

    public bool IsReady => _state == HarpoonState.Ready;
    public bool IsStuck => _state == HarpoonState.Stuck;
    public float MashGauge => _mashGauge;

    private void Update()
    {
        switch (_state)
        {
            case HarpoonState.Flying:
                Fly();
                break;
            case HarpoonState.Stuck:
                StayStuck();
                break;
            case HarpoonState.Returning:
                ReturnToTarget();
                break;
        }
    }

    public void Launch(Vector3 origin, Vector3 direction, Transform returnTarget)
    {
        _returnTarget = returnTarget;
        _traveledDistance = 0f;
        _state = HarpoonState.Flying;

        transform.SetPositionAndRotation(origin, Quaternion.LookRotation(direction));
        gameObject.SetActive(true);
    }

    public void Mash()
    {
        // 적에게 박혀 있는 동안의 연타만 게이지에 반영한다
        if (_state != HarpoonState.Stuck)
        {
            return;
        }

        _mashGauge = Mathf.Min(1f, _mashGauge + _mashGain);
    }

    private void Fly()
    {
        float step = _flySpeed * Time.deltaTime;

        // 빠른 속도에서 대상을 뚫고 지나가지 않도록 이번 프레임 이동 구간을 미리 검사한다
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, step, _hitLayers, QueryTriggerInteraction.Ignore))
        {
            HandleHit(hit);
            return;
        }

        transform.position += transform.forward * step;
        _traveledDistance += step;

        if (_traveledDistance >= _maxDistance)
        {
            _state = HarpoonState.Returning;
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        // 적이 아닌 일반 물체, 튕김, 사망 반응은 모두 즉시 회수한다
        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy == null || enemy.TakeHarpoonHit() != HarpoonHitReaction.Drag)
        {
            _state = HarpoonState.Returning;
            return;
        }

        StickTo(enemy, hit.point);
    }

    private void StickTo(Enemy enemy, Vector3 hitPoint)
    {
        // 대상이 움직여도 같은 자리에 박혀 있도록 대상 기준의 로컬 위치/회전으로 기억한다
        _draggedEnemy = enemy;
        Transform target = enemy.transform;
        _stuckLocalPosition = target.InverseTransformPoint(hitPoint);
        _stuckLocalRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        _stuckTimer = _stuckDuration;
        _mashGauge = 0f;
        _state = HarpoonState.Stuck;
    }

    private void StayStuck()
    {
        Transform target = _draggedEnemy.transform;
        transform.SetPositionAndRotation(
            target.TransformPoint(_stuckLocalPosition),
            target.rotation * _stuckLocalRotation);

        // 게이지가 가득 차면 즉시 끌어오고, 제한 시간 안에 채우지 못하면 적을 놓고 작살만 회수한다
        if (_mashGauge >= 1f)
        {
            // 회수 중에는 작살이 적을 끌고 오므로, 작살 기준 적의 위치 차이를 기억한다
            _dragOffset = target.position - transform.position;
            _state = HarpoonState.Returning;
            return;
        }

        _mashGauge = Mathf.Max(0f, _mashGauge - _gaugeDecayRate * Time.deltaTime);
        _stuckTimer -= Time.deltaTime;
        if (_stuckTimer <= 0f)
        {
            _draggedEnemy = null;
            _state = HarpoonState.Returning;
        }
    }

    private void ReturnToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _returnTarget.position, _returnSpeed * Time.deltaTime);

        if (_draggedEnemy != null)
        {
            _draggedEnemy.transform.position = transform.position + _dragOffset;
        }

        if (transform.position != _returnTarget.position)
        {
            return;
        }

        // 끌고 온 적은 회수가 끝나는 순간 자동으로 제거한다
        if (_draggedEnemy != null)
        {
            Destroy(_draggedEnemy.gameObject);
            _draggedEnemy = null;
        }

        _state = HarpoonState.Ready;
        gameObject.SetActive(false);
    }
}
