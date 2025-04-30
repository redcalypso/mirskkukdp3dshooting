using UnityEditor;
using UnityEngine;

public enum CameraState
{
    FPS,
    TPS,
    TDV,
}

public class Camera_Master : MonoBehaviour
{
    // requirements
    private CameraState _currentCameraState = CameraState.TDV;
    private Camera_Component[] _cameraControllers;

    // misc
    private int _currentCameraIndex = 0;

    private void Awake()
    {
        _cameraControllers = GetComponents<Camera_Component>();
        SwitchCamera(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) SetCameraMode(CameraState.FPS);
        else if (Input.GetKeyDown(KeyCode.F2)) SetCameraMode(CameraState.TPS);
        else if (Input.GetKeyDown(KeyCode.F3)) SetCameraMode(CameraState.TDV);
    }

    

    private void SetCameraMode(CameraState _camState)
    {
        switch (_camState)
        {
            case CameraState.FPS:
                SwitchCamera(0);
                break;
            case CameraState.TPS:
                SwitchCamera(1);
                break;
            case CameraState.TDV:
                SwitchCamera(2);
                break;
        }
    }

    private void SwitchCamera(int index)
    {
        if (index < 0 || index >= _cameraControllers.Length) return;

        for (int i = 0; i < _cameraControllers.Length; i++)
        {
            _cameraControllers[i].enabled = (i == index);
        }
        _currentCameraIndex = index;
    }
}
