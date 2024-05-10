using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public void PlayGame()
    {
        StartCoroutine(loadAsynchronously(1));
    }
    public void PlayGame2()
    {
        StartCoroutine(loadAsynchronously(2));
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator loadAsynchronously(int i)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + i);

        loadingScreen.SetActive(true);

        while(!operation.isDone)
        {
            //Debug.Log(""+operation.progress);
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;

            yield return null;
        }
    }
}
