using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    // requirements
    [SerializeField] private Camera_MinimapProjector _minimapCamera;

    [Header("UI Components")]
    [SerializeField] private RawImage _minimapImage;
    [SerializeField] private RectTransform _minimapRect;
    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;

    [Header("Size Settings")]
    [SerializeField] private float _minimapSize = 200f;

    private void Awake()
    {
        if (_minimapCamera == null)
        {
            Debug.LogError("Minimap Camera not found!");
            return;
        }

        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        if (_minimapRect != null) _minimapRect.sizeDelta = new Vector2(_minimapSize, _minimapSize);

        if (_minimapImage != null && _minimapCamera != null)
        {
            RenderTexture renderTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
            _minimapCamera.GetComponent<Camera>().targetTexture = renderTexture;
            _minimapImage.texture = renderTexture;
        }
    }

    private void SetupButtons()
    {
        if (_zoomInButton != null) _zoomInButton.onClick.AddListener(() => _minimapCamera.ZoomIn());
        if (_zoomOutButton != null) _zoomOutButton.onClick.AddListener(() => _minimapCamera.ZoomOut());
    }

    private void OnDestroy()
    {
        if (_minimapImage != null && _minimapImage.texture != null) Destroy(_minimapImage.texture);
    }
} 