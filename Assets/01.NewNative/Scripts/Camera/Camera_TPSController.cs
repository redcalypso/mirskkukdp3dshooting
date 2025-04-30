using UnityEngine;
using DG.Tweening;

public class Camera_TPSController : Camera_Component
{
    [SerializeField] private Transform _cameraHandler;
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -5f);

    private Vector3 _currentRotation;
    private Vector3 _rotationVelocity;

    private void Update()
    {
        if (!enabled) return;

        float mouseX = Input.GetAxis("Mouse X") * _rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * _rotationSpeed;

        _currentRotation.y += mouseX;
        _currentRotation.x -= mouseY;
        _currentRotation.x = Mathf.Clamp(_currentRotation.x, -30f, 60f);

        Vector3 targetRotation = new Vector3(_currentRotation.x, _currentRotation.y, 0f);
        _cameraHandler.localRotation = Quaternion.Euler(targetRotation);
        _cameraHandler.localPosition = _offset;
    }
}
