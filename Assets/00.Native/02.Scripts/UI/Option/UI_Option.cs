using DG.Tweening.Core.Easing;
using UnityEngine;

public class UI_Option : UI_PopUp
{
    public void OnClickContinueButton()
    {
        // GameManager.Instance.Continue();

        gameObject.SetActive(false);
    }

    public void OnClickRetryButton()
    {
        // GameManager.Instance.Restart();
    }

    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickCreditButton()
    {
        Manager_PopUp.instance.Open(EPopupType.UI_Credit);
    }
}
