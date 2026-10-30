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

    private HarpoonState _state = HarpoonState.Ready;
    private Transform _returnTarget;
    private float _traveledDistance;
    private float _stuckTimer;
    private Transform _stuckTarget;
    private Vector3 _stuckLocalPosition;
    private Quaternion _stuckLocalRotation;

    public bool IsReady => _state == HarpoonState.Ready;

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

    private void Fly()
    {
        float step = _flySpeed * Time.deltaTime;

        // 빠른 속도에서 대상을 뚫고 지나가지 않도록 이번 프레임 이동 구간을 미리 검사한다
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, step, _hitLayers, QueryTriggerInteraction.Ignore))
        {
            StickTo(hit);
            return;
        }

        transform.position += transform.forward * step;
        _traveledDistance += step;

        if (_traveledDistance >= _maxDistance)
        {
            _state = HarpoonState.Returning;
        }
    }

    private void StickTo(RaycastHit hit)
    {
        // 대상이 움직여도 같은 자리에 박혀 있도록 대상 기준의 로컬 위치/회전으로 기억한다
        _stuckTarget = hit.transform;
        _stuckLocalPosition = _stuckTarget.InverseTransformPoint(hit.point);
        _stuckLocalRotation = Quaternion.Inverse(_stuckTarget.rotation) * transform.rotation;
        _stuckTimer = _stuckDuration;
        _state = HarpoonState.Stuck;
    }

    private void StayStuck()
    {
        transform.SetPositionAndRotation(
            _stuckTarget.TransformPoint(_stuckLocalPosition),
            _stuckTarget.rotation * _stuckLocalRotation);

        _stuckTimer -= Time.deltaTime;
        if (_stuckTimer <= 0f)
        {
            _state = HarpoonState.Returning;
        }
    }

    private void ReturnToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _returnTarget.position, _returnSpeed * Time.deltaTime);

        if (transform.position == _returnTarget.position)
        {
            _state = HarpoonState.Ready;
            gameObject.SetActive(false);
        }
    }
}
