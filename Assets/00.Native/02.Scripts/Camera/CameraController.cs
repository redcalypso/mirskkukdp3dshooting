using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        FPS,
        TPS,
        TopDown
    }

    [Header("Camera Settings")]
    public CameraMode currentMode = CameraMode.FPS;
    public Transform target;
    public float smoothSpeed = 10f;

    [Header("FPS Camera Settings")]
    public Vector3 fpsOffset = new Vector3(0f, 0.45f, 0f);
    public Vector3 fpsRotationOffset = Vector3.zero;

    [Header("TPS Camera Settings")]
    public Vector3 tpsOffset = new Vector3(1.3f, 0.3f, -3f);
    public Vector3 tpsRotationOffset = new Vector3(0.4f, 0f, 0f);
    public float tpsRotationSpeed = 2f;

    [Header("Top-Down Camera Settings")]
    public Vector3 topDownOffset = new Vector3(0f, 12f, -4.5f);
    public Vector3 topDownRotationOffset = new Vector3(70f, 0f, 0f);

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public bool invertY = false;

    [Header("Recoil Settings")]
    public float recoilReturnSpeed = 5f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float recoilX = 0f;
    private float recoilY = 0f;
    private float targetRecoilX = 0f;
    private float targetRecoilY = 0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool isInitialized;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target is not set! Please assign a target in the inspector.");
            return;
        }
        // 마우스 커서 숨기기 및 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return;

        // 카메라 모드 변경 키 입력 처리
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetCameraMode(CameraMode.FPS);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SetCameraMode(CameraMode.TPS);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SetCameraMode(CameraMode.TopDown);
        }

        // 마우스 입력 처리
        ProcessMouseInput();
        UpdateRecoil();
    }

    void LateUpdate()
    {
        if (!isInitialized) return;

        // 카메라 위치 및 회전 업데이트
        UpdateCameraPositionAndRotation();
    }

    private void ProcessMouseInput()
    {
        if (currentMode != CameraMode.TopDown)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            if (invertY)
                mouseY = -mouseY;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        }
    }

    private void UpdateRecoil()
    {
        // 반동값을 부드럽게 원래 위치로 복귀
        recoilX = Mathf.Lerp(recoilX, targetRecoilX, Time.deltaTime * recoilReturnSpeed);
        recoilY = Mathf.Lerp(recoilY, targetRecoilY, Time.deltaTime * recoilReturnSpeed);
    }

    private void UpdateCameraPositionAndRotation()
    {
        switch (currentMode)
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
        // FPS 카메라 위치 설정
        Vector3 desiredPosition = target.position + target.TransformDirection(fpsOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // FPS 카메라 회전 설정
        Quaternion targetRotation = Quaternion.Euler(
            rotationX - recoilX + fpsRotationOffset.x, 
            rotationY + recoilY + fpsRotationOffset.y, 
            fpsRotationOffset.z
        );
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }

    void UpdateTPSCamera()
    {
        // TPS 카메라 위치 계산
        Vector3 rotation = new Vector3(
            rotationX - recoilX + tpsRotationOffset.x, 
            rotationY + recoilY + tpsRotationOffset.y, 
            tpsRotationOffset.z
        );
        Quaternion targetRotation = Quaternion.Euler(rotation);
        
        Vector3 desiredPosition = target.position + targetRotation * tpsOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // 카메라가 타겟을 바라보도록 설정
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }

    void UpdateTopDownCamera()
    {
        // Top-Down 카메라 위치 계산
        Vector3 desiredPosition = target.position + topDownOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Top-Down 카메라 회전 설정
        Quaternion targetRotation = Quaternion.Euler(topDownRotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }

    // 카메라 모드 변경 메서드
    public void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;
    }

    // 카메라의 X축 회전값을 반환하는 메서드
    public float GetCameraRotationX()
    {
        return rotationX;
    }

    public void SetCameraRotationX(float value)
    {
        rotationX = value;
    }

    public void AddRecoil(float x, float y)
    {
        targetRecoilX += x;
        targetRecoilY += y;
    }

    public float GetRecoilX()
    {
        return recoilX;
    }

    public float GetRecoilY()
    {
        return recoilY;
    }
}
