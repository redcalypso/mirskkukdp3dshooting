using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Object_ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [Header("Barrel Settings")]
    [SerializeField] private float _maxHealth = 50f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionForce = 50f;
    [SerializeField] private int _explosionDamage = 50;
    [SerializeField] private float _destroyDelay = 3f;
    [SerializeField] private LayerMask _damageableLayers;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _explosionEffect;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private Material _damagedMaterial;

    private bool _isExploded = false;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private Coroutine _destroyCoroutine;

    public bool IsDead => _isExploded;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (_isExploded) return;

        _currentHealth -= damage;
        if (_currentHealth <= 0) Explode();
    }

    private void Explode()
    {
        if (_isExploded) return;
        
        _isExploded = true;

        if (_explosionEffect != null)
        {
            ParticleSystem ExplosionEffect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(ExplosionEffect.gameObject, ExplosionEffect.main.duration);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _damageableLayers);
        
        foreach (Collider hit in colliders)
        {
            if(hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_explosionDamage);
            }
        }

        if (_rigidbody != null)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            _rigidbody.AddForce(randomDirection * _explosionForce, ForceMode.Impulse);
            _rigidbody.AddTorque(Random.insideUnitSphere * _explosionForce, ForceMode.Impulse);
        }

        Destroy(gameObject, _destroyDelay);
    }
} 