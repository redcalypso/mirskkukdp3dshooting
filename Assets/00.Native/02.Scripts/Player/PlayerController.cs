using UnityEngine;

public class PlayerController : PlayerComponent
{
    private Camera_CameraController cameraController;

    protected override void Awake()
    {
        base.Awake();
        cameraController = Camera.main.GetComponent<Camera_CameraController>();
        if (cameraController == null) Debug.LogWarning("Camera_CameraController not found in the scene!");
    }

    private void Update()
    {
        if (cameraController != null)
        {
            Vector3 cameraForward = cameraController.transform.forward;
            cameraForward.y = 0;
            if (cameraForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
    }
}
