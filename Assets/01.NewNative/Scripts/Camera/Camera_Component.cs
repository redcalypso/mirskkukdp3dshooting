using UnityEngine;

public abstract class Camera_Component : MonoBehaviour
{
    protected Camera_Master _cameraMaster;
    public Camera_Master Camera_Master => _cameraMaster;

    protected virtual void Awake()
    {
        _cameraMaster = GetComponent<Camera_Master>();
        if (_cameraMaster == null) Debug.LogError($"Camera Master Component Not Found For {GetType().Name}");
    }
}
