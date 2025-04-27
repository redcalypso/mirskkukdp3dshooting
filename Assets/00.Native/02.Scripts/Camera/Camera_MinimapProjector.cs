using UnityEngine;
using DG.Tweening;

public class Camera_MinimapProjector : MonoBehaviour
{
    private const float Y_OFFSET = 10f;

    [Header("Target Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private bool _followTargetRotation = true;

    [Header("Zoom Settings")]
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 20f;
    [SerializeField] private float _zoomSpeed = 0.5f;
    [SerializeField] private float _zoomStep = 1f;
    [SerializeField] private Ease _zoomEase = Ease.OutQuad;

    private float _currentZoom;
    private Camera _camera;
    private Sequence _zoomSequence;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _currentZoom = (_minZoom + _maxZoom) / 2;
        _camera.orthographicSize = _currentZoom;
    }

    private void OnDestroy()
    {
        _zoomSequence?.Kill();
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // Update position
        Vector3 newPosition = _target.position;
        newPosition.y += Y_OFFSET;
        transform.position = newPosition;

        // Update rotation
        if (_followTargetRotation)
        {
            Vector3 newEulerAngles = Vector3.zero;
            newEulerAngles.x = 90f; // Look down
            newEulerAngles.y = _target.eulerAngles.y; // Follow Target's Y rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newEulerAngles), Time.deltaTime * _rotationSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    public void ZoomIn()
    {
        float targetZoom = Mathf.Max(_minZoom, _currentZoom - _zoomStep);
        AnimateZoom(targetZoom);
    }

    public void ZoomOut()
    {
        float targetZoom = Mathf.Min(_maxZoom, _currentZoom + _zoomStep);
        AnimateZoom(targetZoom);
    }

    private void AnimateZoom(float targetZoom)
    {
        _zoomSequence?.Kill();
        _zoomSequence = DOTween.Sequence();

        _zoomSequence.Append(
            DOTween.To(
                () => _currentZoom,
                value =>
                {
                    _currentZoom = value;
                    _camera.orthographicSize = value;
                },
                targetZoom,
                _zoomSpeed
            ).SetEase(_zoomEase)
        );
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ToggleRotationFollow()
    {
        _followTargetRotation = !_followTargetRotation;
    }
} 