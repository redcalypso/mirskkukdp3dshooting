using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform handTransform;
    public float handRotationSpeed = 10f;
    public float maxHandRotation = 80f;
    public float minHandRotation = -80f;

    private CameraController cameraController;

    void Start()
    {
        // CameraController 컴포넌트 찾기
        cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController == null)
        {
            Debug.LogWarning("CameraController not found in the scene!");
        }
    }

    void Update()
    {
        if (cameraController != null && handTransform != null)
        {
            // 카메라의 X축 회전값을 가져와서 Hand의 회전에 적용
            float cameraRotationX = cameraController.GetCameraRotationX();
            
            // 회전값을 제한
            float targetRotation = Mathf.Clamp(cameraRotationX, minHandRotation, maxHandRotation);
            
            // Hand의 회전을 부드럽게 적용
            Quaternion targetQuaternion = Quaternion.Euler(targetRotation, 0f, 0f);
            handTransform.localRotation = Quaternion.Slerp(handTransform.localRotation, targetQuaternion, handRotationSpeed * Time.deltaTime);
        }
    }
} 