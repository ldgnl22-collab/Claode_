using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 인덱스 = 타격 순서. 예) [Deflect, Drag] 는 1회째 튕겨내고 2회째 끌려온다
    [SerializeField] private HarpoonHitReaction[] _hitReactions = { HarpoonHitReaction.Drag };
    [SerializeField] private Color _deadColor = Color.gray;

    private Renderer _renderer;
    private int _hitCount;
    private bool _isDead;

    public bool IsDead => _isDead;

    private void Awake() => _renderer = GetComponentInChildren<Renderer>();

    public HarpoonHitReaction TakeHarpoonHit()
    {
        // 사망한 적은 일반 물체처럼 작살을 튕겨낸다
        if (_isDead)
        {
            return HarpoonHitReaction.Deflect;
        }

        HarpoonHitReaction reaction = _hitReactions[Mathf.Min(_hitCount, _hitReactions.Length - 1)];
        _hitCount++;

        if (reaction == HarpoonHitReaction.Die)
        {
            Die();
        }

        return reaction;
    }

    private void Die()
    {
        _isDead = true;
        _renderer.material.color = _deadColor;
    }
}
