using UnityEngine;

public abstract class PlayerComponent : MonoBehaviour
{
    protected Player _player;

    public Player Player => _player;

    protected virtual void Awake()
    {
        _player = GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError($"Player component not found for {GetType().Name}");
        }
    }
}
