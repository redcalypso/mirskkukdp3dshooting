using UnityEngine;

public enum CameraMode
{
    FPS,
    TPS,
    TDV
}

public class Camera_CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public CameraMode CurrentMode = CameraMode.TPS;
    public Transform Target;
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("FPS Camera Settings")]
    [SerializeField] private Vector3 _fpsOffset = new(0f, 1.45f, 0f);
    [SerializeField] private Vector3 _fpsRotationOffset = Vector3.zero;

    [Header("TPS Camera Settings")]
    [SerializeField] private Vector3 _tpsOffset = new(1.3f, 1.3f, -3f);
    [SerializeField] private Vector3 _tpsRotationOffset = new(0.4f, 0f, 0f);

    [Header("Top-Down Camera Settings")]
    [SerializeField] private Vector3 _topDownOffset = new(0f, 12f, -4.5f);
    [SerializeField] private Vector3 _topDownRotationOffset = new(70f, 0f, 0f);

    [Header("Mouse Settings")]
    public float MouseSensitivity = 2f;
    public bool InvertY = false;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

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
        // if (Input.GetKeyDown(KeyCode.F1)) SetCameraMode(CameraMode.FPS);
        if (Input.GetKeyDown(KeyCode.F2)) SetCameraMode(CameraMode.TPS);
        else if (Input.GetKeyDown(KeyCode.F3)) SetCameraMode(CameraMode.TDV);

        ProcessMouseInput();
    }

    void LateUpdate()
    {
        UpdateCameraPositionAndRotation();
    }

    private void ProcessMouseInput()
    {
        if (CurrentMode != CameraMode.TDV)
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

            if (InvertY) mouseY = -mouseY;

            _rotationY += mouseX;
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        }
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
            case CameraMode.TDV:
                UpdateTopDownCamera();
                break;
        }
    }

    void UpdateFPSCamera()
    {
        Vector3 desiredPosition = Target.position + Target.TransformDirection(_fpsOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        float finalRotationX = _rotationX;
        float finalRotationY = _rotationY;

        Quaternion targetRotation = Quaternion.Euler(
            finalRotationX + _fpsRotationOffset.x,
            finalRotationY + _fpsRotationOffset.y,
            _fpsRotationOffset.z
        );
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _smoothSpeed * Time.deltaTime);
    }

    void UpdateTPSCamera()
    {
        float finalRotationX = _rotationX;
        float finalRotationY = _rotationY;

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
}