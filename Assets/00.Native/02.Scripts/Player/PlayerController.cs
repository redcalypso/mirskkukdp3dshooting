using UnityEngine;

public class PlayerController : PlayerComponent
{
    private Camera_CameraController _cameraController;

    protected override void Awake()
    {
        base.Awake();
        _cameraController = Camera.main.GetComponent<Camera_CameraController>();
        if (_cameraController == null) Debug.LogWarning("Camera_CameraController not found in the scene!");
    }

    private void Update()
    {
        if (_cameraController != null)
        {
            if(_cameraController.CurrentMode == CameraMode.FPS || _cameraController.CurrentMode == CameraMode.TPS)
            { 
                Vector3 cameraForward = _cameraController.transform.forward;
                cameraForward.y = 0;
                if (cameraForward != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                }
            }
            else if(_cameraController.CurrentMode == CameraMode.TDV)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int groundLayerMaskIndex = LayerMask.GetMask("Terrain");

                if(Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayerMaskIndex))
                {
                    Vector3 targetPoint = hit.point;
                    Vector3 direction = targetPoint - transform.position;
                    direction.y = 0;

                    if(direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                    }
                }
            }
        }
    }
}
