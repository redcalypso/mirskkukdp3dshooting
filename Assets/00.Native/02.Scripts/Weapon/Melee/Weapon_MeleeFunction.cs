using UnityEngine;

public class Weapon_MeleeFunction : MonoBehaviour
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float _attackRadius = 3f;
    [SerializeField] private float _attackAngle = 90f; 
    [SerializeField] private int _damage = 10;
    [SerializeField] private LayerMask _layerMask;
    public void MeleeAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _attackRadius, _layerMask);

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle < _attackAngle * 0.5f)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(_damage);
            }
        }
    }

    // 에디터에서 부채꼴 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 forward = transform.forward;
        Vector3 left = Quaternion.Euler(0, -_attackAngle / 2, 0) * forward;
        Vector3 right = Quaternion.Euler(0, _attackAngle / 2, 0) * forward;

        Gizmos.DrawWireSphere(transform.position, _attackRadius);

        int segments = 30;
        float deltaAngle = _attackAngle / segments;
        Vector3 prevPoint = transform.position + left * _attackRadius;
        
        for (int i = 1; i <= segments; i++)
        {
            Vector3 nextDir = Quaternion.Euler(0, -_attackAngle / 2 + deltaAngle * i, 0) * forward;
            Vector3 nextPoint = transform.position + nextDir * _attackRadius;
            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
