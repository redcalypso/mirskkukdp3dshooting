using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform handTransform;
    public float handRotationSpeed = 10f;
    public float maxHandRotation = 80f;
    public float minHandRotation = -80f;
    public float recoilReturnSpeed = 5f;

    private CameraController _cameraController;
    private float _currentRecoilX;
    private float _currentRecoilY;
    private float _targetRecoilX;
    private float _targetRecoilY;

    void Start()
    {
        // CameraController 컴포넌트 찾기
        _cameraController = Camera.main.GetComponent<CameraController>();
        if (_cameraController == null)
        {
            Debug.LogWarning("CameraController not found in the scene!");
        }
    }

    void Update()
    {
        if (_cameraController != null && handTransform != null)
        {
            // 카메라의 X축 회전값과 반동값을 가져와서 Hand의 회전에 적용
            float cameraRotationX = _cameraController.GetCameraRotationX();
            float recoilX = _cameraController.GetRecoilX();
            float recoilY = _cameraController.GetRecoilY();
            
            // 반동값 업데이트
            _targetRecoilX = recoilX;
            _targetRecoilY = recoilY;
            _currentRecoilX = Mathf.Lerp(_currentRecoilX, _targetRecoilX, Time.deltaTime * recoilReturnSpeed);
            _currentRecoilY = Mathf.Lerp(_currentRecoilY, _targetRecoilY, Time.deltaTime * recoilReturnSpeed);
            
            // 회전값을 제한 (반동 포함)
            float targetRotation = Mathf.Clamp(cameraRotationX - _currentRecoilX, minHandRotation, maxHandRotation);
            
            // Hand의 회전을 부드럽게 적용
            Quaternion targetQuaternion = Quaternion.Euler(targetRotation, _currentRecoilY, 0f);
            handTransform.localRotation = Quaternion.Slerp(handTransform.localRotation, targetQuaternion, handRotationSpeed * Time.deltaTime);
        }
    }
} 