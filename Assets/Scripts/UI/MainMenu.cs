using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider MusicSlider;

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

    private void Awake()
    {
        if (PlayerPrefs.HasKey(MusicManager.PlayerPrefsMasterVolume))
            MusicSlider.value = PlayerPrefs.GetFloat(MusicManager.PlayerPrefsMasterVolume);
    }
}
