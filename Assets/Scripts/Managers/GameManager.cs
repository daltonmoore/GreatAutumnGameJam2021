using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public event EventHandler<GameOverEventArgs> GameOver;
    int wellHeartCount = 0;
    bool isGameOver;

    public enum GameOverReason { WellHeartDestroyed, PlayerDeath}

    public void ResumeGame()
    {
        PlayerController.instance.EnablePlayerControls();
        Time.timeScale = 1;
    }

    public void PauseGame(InputAction.CallbackContext obj)
    {
        PlayerController.instance.DisablePlayerControls();
        Time.timeScale = 0;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (WellHeart wellHeart in SpawnManager.instance.WellHearts)
        {
            wellHeart.WellHeartDeath += OnWellHeartDeath;
            wellHeartCount++;
        }
        PlayerController.instance.PlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(object sender, EventArgs e)
    {
        OnGameOver(GameOverReason.PlayerDeath);
    }

    private void OnWellHeartDeath(object sender, EventArgs e)
    {
        wellHeartCount--;
        if (wellHeartCount <= 0)
        {
            OnGameOver(GameOverReason.WellHeartDestroyed);
        }
    }

    private void OnGameOver(GameOverReason gameOverReason)
    {
        if (!isGameOver)
        {
            isGameOver = true;
            GameOverEventArgs eventArgs = new GameOverEventArgs(gameOverReason);
            GameOver?.Invoke(this, eventArgs);
            PlayerController.instance.DisablePlayerControls();
            StartCoroutine(LoadSceneDelay(3, "MainMenu"));
        }
    }

    private IEnumerator LoadSceneDelay(int d, string sceneName)
    {
        yield return new WaitForSeconds(d);
        SceneManager.LoadScene(sceneName);
    }

    public class GameOverEventArgs
    {
        GameOverReason gameOverReason;
        public string GameOverText;

        public GameOverEventArgs(GameOverReason gameOverReason)
        {
            this.gameOverReason = gameOverReason;
            switch (gameOverReason)
            {
                case GameOverReason.WellHeartDestroyed:
                    GameOverText = "WellHeart Destroyed";
                    break;
                case GameOverReason.PlayerDeath:
                    GameOverText = "You Died";
                    break;
                default:
                    break;
            }
        }
    }
}
