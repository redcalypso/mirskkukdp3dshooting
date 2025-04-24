using UnityEngine;
using System.Collections;

public class GunShootingFunction : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GunPreset _gunPreset;
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private Camera _playerCamera;

    [Header("UI")]
    [SerializeField] private UI_Ammo _ammoUI;

    private float _nextFireTime;
    private bool _isReloading;
    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        if (_player == null) return;

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();

        if (_playerCamera == null) _playerCamera = Camera.main;

        if (_ammoUI == null) _ammoUI = FindObjectOfType<UI_Ammo>();

        UpdateAmmoUI();
    }

    public void Shoot()
    {
        if (_isReloading || Time.time < _nextFireTime || _gunPreset.currentAmmo <= 0)
        {
            if (_gunPreset.currentAmmo <= 0)
            {
                PlaySound(_gunPreset.emptySound);
            }
            return;
        }

        _nextFireTime = Time.time + _gunPreset.fireRate;
        _gunPreset.currentAmmo--;

        // 총구 이펙트
        if (_gunPreset.muzzleFlash != null)
        {
            Instantiate(_gunPreset.muzzleFlash, _muzzlePoint.position, _muzzlePoint.rotation);
        }

        // 사운드 재생
        PlaySound(_gunPreset.shootSound);

        // 레이캐스트 발사
        RaycastHit hit;
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _gunPreset.range, _gunPreset.shootableLayers))
        {
            // 히트 이펙트
            if (_gunPreset.hitFlash != null)
            {
                Instantiate(_gunPreset.hitFlash, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // 데미지 처리
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_gunPreset.damage);
            }
        }

        // 탄약 UI 업데이트
        UpdateAmmoUI();
    }

    public void Reload()
    {
        if (_isReloading || _gunPreset.currentAmmo == _gunPreset.maxAmmo) return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        _isReloading = true;
        PlaySound(_gunPreset.reloadSound);

        yield return new WaitForSeconds(_gunPreset.reloadTime);

        _gunPreset.currentAmmo = _gunPreset.maxAmmo;
        _isReloading = false;

        // 탄약 UI 업데이트
        UpdateAmmoUI();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void UpdateAmmoUI()
    {
        if (_ammoUI != null)
        {
            _ammoUI.UpdateAmmoText(_gunPreset.currentAmmo, _gunPreset.maxAmmo);
        }
    }
} 