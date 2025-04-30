using UnityEngine;

public class Player_MovingTDV : Player_Component
{
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    private Vector3 _currentRotationVelocity;

    private void Update()
    {
        if (PlayerMaster == null) return;

        // Get camera forward and right vectors
        Vector3 cameraForward = PlayerMaster.CameraHandler.forward;
        Vector3 cameraRight = PlayerMaster.CameraHandler.right;

        // Project vectors onto horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction based on camera orientation
        Vector3 moveDirection = cameraForward * PlayerMaster.MoveDirection.z + cameraRight * PlayerMaster.MoveDirection.x;
        moveDirection.Normalize();

        // Set movement direction and move
        PlayerMaster.SetMoveDirection(moveDirection);
        PlayerMaster.Move();

        // Get mouse position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // Calculate direction to mouse position
            Vector3 targetDirection = hit.point - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            // Smoothly rotate towards mouse position
            Vector3 smoothDirection = Vector3.SmoothDamp(
                transform.forward,
                targetDirection,
                ref _currentRotationVelocity,
                _rotationSmoothTime
            );
            PlayerMaster.RotateTowards(smoothDirection);
        }
    }
}
