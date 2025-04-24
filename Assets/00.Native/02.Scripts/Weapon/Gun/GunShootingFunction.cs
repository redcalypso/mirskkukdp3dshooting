using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GunShootingFunction : MonoBehaviour
{
    [SerializeField] private GunPreset _gunPreset;
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private UI_Ammo _ammoUI;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private float _shakeAmount = 0.1f;
    [SerializeField] private float _shakeDuration = 0.1f;

    private float _nextFireTime;
    private bool _isReloading;
    private AudioSource _audioSource;
    private int _currentAmmo;
    private float _reloadTimer;
    private int _currentRecoilIndex;
    private float _lastShotTime;
    private float _recoilResetTime = 0.5f;
    private Sequence _recoilSequence;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();

        if (_playerCamera == null) _playerCamera = Camera.main;

        if (_ammoUI == null) _ammoUI = FindObjectOfType<UI_Ammo>();

        if (_cameraController == null) _cameraController = _playerCamera.GetComponent<CameraController>();

        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;

        _currentAmmo = _gunPreset.maxAmmo;
        UpdateAmmoUI();
    }

    private void OnDestroy()
    {
        _recoilSequence?.Kill();
    }

    private void Update()
    {
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

        // 일정 시간 동안 발사하지 않으면 반동 패턴 초기화
        if (Time.time - _lastShotTime > _recoilResetTime)
        {
            _currentRecoilIndex = 0;
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
            if (_currentAmmo <= 0)
            {
                // PlaySound(_gunPreset.emptySound);
            }
            return;
        }

        _nextFireTime = Time.time + _gunPreset.fireRate;
        _currentAmmo--;
        _lastShotTime = Time.time;

        // 총구 이펙트
        if (_gunPreset.muzzleFlash != null)
        {
            Instantiate(_gunPreset.muzzleFlash, _muzzlePoint.position, _muzzlePoint.rotation);
        }

        // 사운드 재생
        PlaySound(_gunPreset.shootSound);

        // 총기 반동
        ApplyRecoil();

        // 총기 흔들림
        ApplyGunShake();

        // 레이캐스트 발사
        RaycastHit hit;
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _gunPreset.range))
        {
            // 히트 이펙트
            if (_gunPreset.hitFlash != null)
            {
                Instantiate(_gunPreset.hitFlash, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        // 탄약 UI 업데이트
        UpdateAmmoUI();
    }

    private void ApplyRecoil()
    {
        _recoilSequence?.Kill();
        _recoilSequence = DOTween.Sequence();

        // 현재 반동 패턴 가져오기
        Vector2 recoilDirection = _gunPreset.recoilPattern[_currentRecoilIndex];
        
        // 카메라 회전에 반동 적용
        float recoilX = recoilDirection.y * _gunPreset.recoilAmount * 10f; // 수직 반동
        float recoilY = recoilDirection.x * _gunPreset.recoilAmount * 5f;  // 수평 반동

        _cameraController.AddRecoil(recoilX, recoilY);

        // 다음 반동 패턴으로 이동
        _currentRecoilIndex = (_currentRecoilIndex + 1) % _gunPreset.recoilPattern.Length;
    }

    private void ApplyGunShake()
    {
        // 랜덤한 방향으로 흔들림
        Vector3 shakeOffset = new Vector3(
            Random.Range(-_shakeAmount, _shakeAmount),
            Random.Range(-_shakeAmount, _shakeAmount),
            Random.Range(-_shakeAmount, _shakeAmount)
        );

        // DoTween 시퀀스 생성
        Sequence shakeSequence = DOTween.Sequence();

        // 위치 흔들림
        shakeSequence.Append(transform.DOLocalMove(_originalPosition + shakeOffset, _shakeDuration * 0.5f))
            .Append(transform.DOLocalMove(_originalPosition, _shakeDuration * 0.5f));


        // 이징 설정
        shakeSequence.SetEase(Ease.OutQuad);
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
        if (clip != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void UpdateAmmoUI()
    {
        if (_ammoUI != null)
        {
            _ammoUI.UpdateAmmoText(_currentAmmo, _gunPreset.maxAmmo);
        }
    }
} 