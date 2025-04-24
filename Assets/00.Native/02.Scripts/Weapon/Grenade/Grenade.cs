using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private ParticleSystem _grenadeExplosion;
    [SerializeField] private float _explosionDelay = 3f;
    // [SerializeField] private float _explosionDamage = 100f;
    // [SerializeField] private float _explosionRadius = 5f;
    // Damage Function will be implemented after

    private Rigidbody _rb;
    private bool _hasExploded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExploded) return;
        _hasExploded = true;
        Invoke("Explode", _explosionDelay);
    }

    private void Explode()
    {
        // TODO: Add explosion effects and damage logic here
        Destroy(gameObject);
    }
}
