using UnityEngine;

public enum CameraMode
{
    FPS,
    TPS,
    TopDown
}

public class Camera_CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public CameraMode CurrentMode = CameraMode.FPS;
    public Transform Target;
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("FPS Camera Settings")]
    [SerializeField] private Vector3 _fpsOffset = new(0f, 0.45f, 0f);
    [SerializeField] private Vector3 _fpsRotationOffset = Vector3.zero;

    [Header("TPS Camera Settings")]
    [SerializeField] private Vector3 _tpsOffset = new(1.3f, 0.3f, -3f);
    [SerializeField] private Vector3 _tpsRotationOffset = new(0.4f, 0f, 0f);

    [Header("Top-Down Camera Settings")]
    [SerializeField] private Vector3 _topDownOffset = new(0f, 12f, -4.5f);
    [SerializeField] private Vector3 _topDownRotationOffset = new(70f, 0f, 0f);

    [Header("Mouse Settings")]
    public float MouseSensitivity = 2f;
    public bool InvertY = false;

    [Header("Recoil Settings")]
    [SerializeField] private float _recoilReturnSpeed = 5f;
    [SerializeField] private float _maxRecoilX = 10f;
    [SerializeField] private float _maxRecoilY = 10f;
    [SerializeField] private float _recoilAccumulationFactor = 1.2f;
    [SerializeField] private float _maxAccumulatedRecoilX = 20f;
    [SerializeField] private float _maxAccumulatedRecoilY = 20f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;
    private float _currentRecoilX = 0f;
    private float _currentRecoilY = 0f;
    private float _accumulatedRecoilX = 0f;
    private float _accumulatedRecoilY = 0f;

    void Start()
    {
        if (Target == null)
        {
            Debug.LogWarning("Camera Target is not set! Please assign a Target in the inspector.");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) SetCameraMode(CameraMode.FPS);
        else if (Input.GetKeyDown(KeyCode.F2)) SetCameraMode(CameraMode.TPS);
        else if (Input.GetKeyDown(KeyCode.F3)) SetCameraMode(CameraMode.TopDown);

        ProcessMouseInput();
        UpdateRecoil();
    }

    void LateUpdate()
    {
        UpdateCameraPositionAndRotation();
    }

    private void ProcessMouseInput()
    {
        if (CurrentMode != CameraMode.TopDown)
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

            if (InvertY) mouseY = -mouseY;

            _rotationY += mouseX;
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        }
    }

    private void UpdateRecoil()
    {
        _accumulatedRecoilX = Mathf.Lerp(_accumulatedRecoilX, 0f, Time.deltaTime * _recoilReturnSpeed);
        _accumulatedRecoilY = Mathf.Lerp(_accumulatedRecoilY, 0f, Time.deltaTime * _recoilReturnSpeed);

        _currentRecoilX = Mathf.Lerp(_currentRecoilX, 0f, Time.deltaTime * _recoilReturnSpeed);
        _currentRecoilY = Mathf.Lerp(_currentRecoilY, 0f, Time.deltaTime * _recoilReturnSpeed);
    }

    private void UpdateCameraPositionAndRotation()
    {
        switch (CurrentMode)
        {
            case CameraMode.FPS:
                UpdateFPSCamera();
                break;
            case CameraMode.TPS:
                UpdateTPSCamera();
                break;
            case CameraMode.TopDown:
                UpdateTopDownCamera();
                break;
        }
    }

    void UpdateFPSCamera()
    {
        Vector3 desiredPosition = Target.position + Target.TransformDirection(_fpsOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        float finalRotationX = _rotationX + _currentRecoilX + _accumulatedRecoilX;
        float finalRotationY = _rotationY + _currentRecoilY + _accumulatedRecoilY;

        Quaternion targetRotation = Quaternion.Euler(
            finalRotationX + _fpsRotationOffset.x,
            finalRotationY + _fpsRotationOffset.y,
            _fpsRotationOffset.z
        );
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _smoothSpeed * Time.deltaTime);
    }

    void UpdateTPSCamera()
    {
        float finalRotationX = _rotationX + _currentRecoilX + _accumulatedRecoilX;
        float finalRotationY = _rotationY + _currentRecoilY + _accumulatedRecoilY;

        Vector3 rotation = new(
            finalRotationX + _tpsRotationOffset.x,
            finalRotationY + _tpsRotationOffset.y,
            _tpsRotationOffset.z
        );
        Quaternion targetRotation = Quaternion.Euler(rotation);

        Vector3 desiredPosition = Target.position + targetRotation * _tpsOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _smoothSpeed * Time.deltaTime);
    }

    void UpdateTopDownCamera()
    {
        Vector3 desiredPosition = Target.position + _topDownOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(_topDownRotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _smoothSpeed * Time.deltaTime);
    }

    public void SetCameraMode(CameraMode mode)
    {
        CurrentMode = mode;
    }

    public float GetCameraRotationX()
    {
        return _rotationX;
    }

    public void SetCameraRotationX(float value)
    {
        _rotationX = value;
    }

    public void AddRecoil(float x, float y)
    {
        _currentRecoilX = Mathf.Clamp(_currentRecoilX - x, -_maxRecoilX, _maxRecoilX);
        _currentRecoilY = Mathf.Clamp(_currentRecoilY + y, -_maxRecoilY, _maxRecoilY);

        _accumulatedRecoilX = Mathf.Clamp(_accumulatedRecoilX - (x * _recoilAccumulationFactor), -_maxAccumulatedRecoilX, _maxAccumulatedRecoilX);
        _accumulatedRecoilY = Mathf.Clamp(_accumulatedRecoilY + (y * _recoilAccumulationFactor), -_maxAccumulatedRecoilY, _maxAccumulatedRecoilY);
    }

    public float GetRecoilX()
    {
        return _currentRecoilX + _accumulatedRecoilX;
    }

    public float GetRecoilY()
    {
        return _currentRecoilY + _accumulatedRecoilY;
    }
}