using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public int NextSceneIndex = 2;

    public Slider ProgressSlider;

    public TextMeshProUGUI ProgressText;

    private IEnumerator LoadNextScene_Coroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(NextSceneIndex);
        ao.allowSceneActivation = false;

        while(ao.isDone == false)
        {
            Debug.Log(ao.progress);
            ProgressSlider.value = ao.progress;
            ProgressText.text = $"{ao.progress * 100f}%";

            if (ao.progress <= 0.1f)
            {
                ProgressText.text = $"{ao.progress * 100f}% 어쩌구 저쩌구 말 넣기 할수있음";
            }

            if (ao.progress >=0.9f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
