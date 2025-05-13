using DG.Tweening;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Weapon_GunShootingFunction : MonoBehaviour
{
    // requirements
    [SerializeField] private UI_Ammo _ammoUI;
    private Camera_CameraController _cameraController;
    private AudioSource _audioSource;

    [Header("Gun Settings")]
    [SerializeField] private Weapon_GunPreset _gunPreset;
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _shakeAmount = 0.1f;
    [SerializeField] private float _shakeDuration = 0.1f;

    private int _currentAmmo;
    private float _nextFireTime;
    private bool _isReloading;
    private float _reloadTimer;
    private float _lastShotTime;
    private float _fireTimer = 0f;


    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
        if (_playerCamera == null) _playerCamera = Camera.main;
        if (_cameraController == null) _cameraController = _playerCamera.GetComponent<Camera_CameraController>();

        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;

        _currentAmmo = _gunPreset.maxAmmo;
        UpdateAmmoUI();
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer <= 0)
            {
                _currentAmmo = _gunPreset.maxAmmo;
                _isReloading = false;
                UpdateAmmoUI();
            }
        }
    }

    public void Shoot()
    {
        if (_isReloading)
        {
            _isReloading = false;
            _reloadTimer = 0f;
            if (_ammoUI != null) _ammoUI.StopReloadUI();
            return;
        }

        if (Time.time < _nextFireTime || _currentAmmo <= 0)
        {
            if (_currentAmmo <= 0) // PlaySound(_gunPreset.emptySound);
            return;
        }

        if(_fireTimer < _gunPreset.fireRate) return;

        _fireTimer = 0f;
        _nextFireTime = Time.time + _gunPreset.fireRate;
        _currentAmmo--;
        _lastShotTime = Time.time;

        if (_gunPreset.muzzleFlash != null) Instantiate(_gunPreset.muzzleFlash, _muzzlePoint.position, _muzzlePoint.rotation);
        PlaySound(_gunPreset.shootSound);

        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _gunPreset.range))
        {
            if (_gunPreset.hitFlash != null) Instantiate(_gunPreset.hitFlash, hit.point, Quaternion.LookRotation(hit.normal));
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable) && !damageable.IsDead) damageable.TakeDamage(_gunPreset.damage);
        }

        UpdateAmmoUI();
    }

    public void Reload()
    {
        if (_isReloading || _currentAmmo == _gunPreset.maxAmmo) return;

        _isReloading = true;
        _reloadTimer = _gunPreset.reloadTime;
        PlaySound(_gunPreset.reloadSound);
        if (_ammoUI != null) _ammoUI.StartReloadUI(_gunPreset.reloadTime);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && _audioSource != null) _audioSource.PlayOneShot(clip);
    }

    private void UpdateAmmoUI()
    {
        if (_ammoUI != null) _ammoUI.UpdateAmmoText(_currentAmmo, _gunPreset.maxAmmo);
    }
} 