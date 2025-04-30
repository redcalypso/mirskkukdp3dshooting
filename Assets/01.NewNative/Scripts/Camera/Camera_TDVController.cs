using UnityEngine;
using DG.Tweening;

public class Camera_TDVController : Camera_Component
{
    [SerializeField] private Transform _cameraHandler;
    [SerializeField] private float _height = 10f;
    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private float _minHeight = 5f;
    [SerializeField] private float _maxHeight = 20f;

    private Vector3 _currentPosition;
    private Vector3 _velocity;

    private void Update()
    {
        if (!enabled) return;

        // Mouse wheel to adjust height
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _height = Mathf.Clamp(_height - scroll * 2f, _minHeight, _maxHeight);

        // Get mouse position in viewport
        Vector3 mouseViewport = Input.mousePosition;
        mouseViewport.x = mouseViewport.x / Screen.width;
        mouseViewport.y = mouseViewport.y / Screen.height;

        // Calculate target position based on mouse position
        Vector3 targetPosition = new Vector3(
            (mouseViewport.x - 0.5f) * 2f * _height * 0.5f,
            _height,
            (mouseViewport.y - 0.5f) * 2f * _height * 0.5f
        );

        // Smoothly move to target position
        _cameraHandler.localPosition = Vector3.SmoothDamp(
            _cameraHandler.localPosition,
            targetPosition,
            ref _velocity,
            _smoothTime
        );

        // Always look down
        _cameraHandler.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
