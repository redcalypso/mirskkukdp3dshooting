using UnityEngine;

public class PlayerShooting : PlayerComponent
{
    private GunShootingFunction _gunShootingFunction;

    protected override void Awake()
    {
        base.Awake();
        if (_player == null) return;
    }

    private void Update()
    {
        if (_gunShootingFunction == null) return;

        if (Input.GetMouseButton(0)) _gunShootingFunction.Shoot();

        if (Input.GetKeyDown(KeyCode.R)) _gunShootingFunction.Reload();
    }
}
