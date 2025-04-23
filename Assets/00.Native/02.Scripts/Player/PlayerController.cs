using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        // 카메라 방향에 따라 플레이어 Y축 회전
        if (cameraController != null)
        {
            Vector3 cameraForward = cameraController.transform.forward;
            cameraForward.y = 0; // Y축 회전만 적용하기 위해 Y값을 0으로 설정
            if (cameraForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
    }
}
