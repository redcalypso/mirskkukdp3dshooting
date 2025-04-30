using UnityEngine;

public class Player_MovingTPS : Player_Component
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

        // Only rotate if moving
        if (moveDirection.magnitude > 0.1f)
        {
            // Smoothly rotate towards movement direction
            Vector3 targetDirection = Vector3.SmoothDamp(
                transform.forward,
                moveDirection,
                ref _currentRotationVelocity,
                _rotationSmoothTime
            );
            PlayerMaster.RotateTowards(targetDirection);
        }
    }
}
