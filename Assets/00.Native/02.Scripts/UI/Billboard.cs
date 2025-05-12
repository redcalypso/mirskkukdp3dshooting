using UnityEngine;

public enum BillboardType
{
    ForPlayer,
    ForEnemy,
}

public class Billboard : MonoBehaviour
{
    // requirements
    private BillboardType _billboardType;

    private Camera _mainCamera;
    private bool _isVisible = true;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera == null) return;

        switch(_billboardType)
        {
            case BillboardType.ForPlayer:
                _isVisible = true;
                break;
            case BillboardType.ForEnemy:
                // Check if the object is visible to the camera
                _isVisible = IsVisibleToCamera();
                gameObject.SetActive(_isVisible);
                break;
        }

        if (_isVisible)
        {
            // Make the UI face the camera
            transform.rotation = _mainCamera.transform.rotation;
        }
    }

    private bool IsVisibleToCamera()
    {
        if (_mainCamera == null) return false;

        // Check if the object is in the camera's view frustum
        Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.z > 0 && 
               viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
               viewportPoint.y >= 0 && viewportPoint.y <= 1;
    }
} 