using UnityEngine;

public class Weapon_GunController : MonoBehaviour
{
    // requirements\
    private Camera_CameraController _cameraController;

    [Header("Hand Settings")]
    public Transform handTransform;
    public float handRotationSpeed = 10f;
    public float maxHandRotation = 80f;
    public float minHandRotation = -80f;
    public float recoilReturnSpeed = 5f;

    private float _currentRecoilX;
    private float _currentRecoilY;
    private float _targetRecoilX;
    private float _targetRecoilY;

    void Start()
    {
        _cameraController = Camera.main.GetComponent<Camera_CameraController>();
        if (_cameraController == null) Debug.LogWarning("Camera_CameraController not found in the scene");
    }

    void Update()
    {
        if (_cameraController != null && handTransform != null)
        {
            float cameraRotationX = _cameraController.GetCameraRotationX();
            /*float recoilX = _cameraController.GetRecoilX();
            float recoilY = _cameraController.GetRecoilY();
            
            _targetRecoilX = recoilX;
            _targetRecoilY = recoilY;
            _currentRecoilX = Mathf.Lerp(_currentRecoilX, _targetRecoilX, Time.deltaTime * recoilReturnSpeed);
            _currentRecoilY = Mathf.Lerp(_currentRecoilY, _targetRecoilY, Time.deltaTime * recoilReturnSpeed);
            
            float targetRotation = Mathf.Clamp(cameraRotationX - _currentRecoilX, minHandRotation, maxHandRotation);
            
            Quaternion targetQuaternion = Quaternion.Euler(targetRotation, _currentRecoilY, 0f);
            handTransform.localRotation = Quaternion.Slerp(handTransform.localRotation, targetQuaternion, handRotationSpeed * Time.deltaTime);*/
        }
    }
} 