using UnityEngine;
using System.Collections.Generic;
using Autodesk.Fbx;

public enum EPopupType
{
    UI_Option,
    UI_Credit,
}

public class Manager_PopUp : MonoBehaviour
{
    public static Manager_PopUp instance;

    public List<UI_PopUp> Popups;
    public List<UI_PopUp> _openedPopups = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Open(EPopupType type)
    {
        Open(type.ToString());
    }

    private void Open(string popUpName)
    {
        foreach (UI_PopUp popUp in Popups)
        {
            if(popUp.gameObject.name == popUpName)
            {
                popUp.Open();
                _openedPopups.Add(popUp);
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_openedPopups.Count > 0)
            {
                _openedPopups[_openedPopups.Count - 1].Close();
                _openedPopups.RemoveAt(_openedPopups.Count - 1);
            }
            // else 게임 퍼즈기능 
        }
    }
}
