using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject LoadingScreen;

    public void StartGame()
    {
        StartCoroutine(LoadingLevel());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadingLevel()
    {
        LoadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        
        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
    }
}
