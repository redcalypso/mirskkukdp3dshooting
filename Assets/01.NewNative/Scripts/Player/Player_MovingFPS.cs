using UnityEngine;

public class Player_MovingFPS : Player_Component
{
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

        // Rotate player to match camera rotation
        PlayerMaster.RotateTowards(cameraForward);
    }
}
