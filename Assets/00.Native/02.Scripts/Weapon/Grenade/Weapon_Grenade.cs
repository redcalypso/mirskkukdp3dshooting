using UnityEngine;

public class Weapon_Grenade : MonoBehaviour
{
    [SerializeField] private ParticleSystem _grenadeExplosion;
    [SerializeField] private float _explosionDelay = 3f;
    [SerializeField] private int _explosionDamage = 50;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private LayerMask _damageableLayers;

    private Rigidbody _rigidBody;
    private bool _hasExploded;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExploded) return;
        _hasExploded = true;

        Invoke("Explode", _explosionDelay);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _damageableLayers);

        ParticleSystem explosionFX = Instantiate(_grenadeExplosion, transform.position, Quaternion.identity);
        Destroy(explosionFX.gameObject, explosionFX.main.duration);

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}
