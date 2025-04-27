using UnityEngine;

public enum WeaponType
{
    MainWeapon,
    SubWeapon,
    MeleeWeapon
}

public class PlayerAttackFunction : PlayerComponent
{
    // requirements
    [SerializeField] private Weapon_GunShootingFunction _gunShootingFunction;

    // imshi
    [SerializeField] private GameObject _mainWeapon;
    [SerializeField] private GameObject _subWeapon;
    [SerializeField] private GameObject _meleeWeapon;

    [Header("Weapon_Grenade Settings")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private Transform _grenadeSpawnPoint;
    [SerializeField] private UI_Grenade _grenadeUI;
    [SerializeField] private float _minThrowForce = 5f;
    [SerializeField] private float _maxThrowForce = 25f;
    [SerializeField] private float _chargeTime = 3f;
    [SerializeField] private float _rechargeTime = 5f;
    [SerializeField] private int _maxGrenades = 3;

    private float _currentThrowForce;
    private float _chargeTimer;
    private int _currentGrenades;
    private bool _isCharging;
    private float _rechargeTimer;
    private Camera _mainCamera;

    protected override void Awake()
    {
        base.Awake();
        if (_player == null) return;

        _currentGrenades = _maxGrenades;
        if (_grenadeUI != null) _grenadeUI.UpdateGrenadeText(_currentGrenades, _maxGrenades);

        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) _gunShootingFunction.Shoot(); 
        if (Input.GetKeyDown(KeyCode.R)) _gunShootingFunction.Reload();

        HandleGrenadeInput();
        HandleGrenadeRecharge();
    }

    private void HandleGrenadeInput()
    {
        if (_currentGrenades <= 0) return;

        if (Input.GetKeyDown(KeyCode.G)) StartCharging();
        else if (Input.GetKey(KeyCode.G)) ContinueCharging();
        else if (Input.GetKeyUp(KeyCode.G)) ThrowGrenade();
    }

    private void StartCharging()
    {
        _isCharging = true;
        _chargeTimer = 0f;
        _currentThrowForce = _minThrowForce;
        if (_grenadeUI != null) _grenadeUI.StartChargeUI();
    }

    private void ContinueCharging()
    {
        if (!_isCharging) return;

        _chargeTimer += Time.deltaTime;
        float chargeRatio = Mathf.Clamp01(_chargeTimer / _chargeTime);
        _currentThrowForce = Mathf.Lerp(_minThrowForce, _maxThrowForce, chargeRatio);
    }

    private void ThrowGrenade()
    {
        if (!_isCharging || _currentGrenades <= 0) return;

        if (_grenadePrefab == null || _grenadeSpawnPoint == null || _mainCamera == null) return;

        GameObject grenade = Instantiate(_grenadePrefab, _grenadeSpawnPoint.position, _mainCamera.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        
        if (rb != null) rb.AddForce(_mainCamera.transform.forward * _currentThrowForce, ForceMode.Impulse);

        _currentGrenades--;
        _isCharging = false;
        _rechargeTimer = _rechargeTime;

        if (_grenadeUI != null)
        {
            _grenadeUI.StopChargeUI();
            _grenadeUI.UpdateGrenadeText(_currentGrenades, _maxGrenades);
        }
    }

    private void HandleGrenadeRecharge()
    {
        if (_currentGrenades >= _maxGrenades) return;

        _rechargeTimer -= Time.deltaTime;
        if (_rechargeTimer <= 0)
        {
            _currentGrenades++;
            _rechargeTimer = _rechargeTime;
            if (_grenadeUI != null) _grenadeUI.UpdateGrenadeText(_currentGrenades, _maxGrenades);
        }
    }
}
