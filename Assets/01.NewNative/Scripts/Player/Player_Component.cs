using UnityEngine;

public abstract class Player_Component : MonoBehaviour
{
    protected Player_Master _playerMaster;
    public Player_Master PlayerMaster => _playerMaster;

    protected virtual void Awake()
    {
        _playerMaster = GetComponent<Player_Master>();
        if (_playerMaster == null) Debug.LogError($"Player Master component not found for {GetType().Name}");
    }
}
