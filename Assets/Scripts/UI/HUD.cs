using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    static HUD instance;
    public static HUD Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HUD>();
            }
            return instance;
        }
    }

    //GameOver UI
    [SerializeField] GameObject GameOverScreen;
    [SerializeField] GameObject GameOverText;
    [SerializeField] GameObject GameOverTextReason;
    // Player HUD
    [SerializeField] Slider m_healthBar;
    [SerializeField] Slider m_soulsBar;
    [SerializeField] Slider m_wellheartBar;

    // Pause UI
    [SerializeField] GameObject PauseScreen;
    //
    [SerializeField] GameObject WinScreen;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameManager.Instance.GameOver += OnGameOver;
        GameManager.Instance.GameWin += OnGameWin;
        SetAlphaOfGameOverScreen(0, 0);
    }

    public void UpdateWellheartHealthBar(float val)
    {
        m_wellheartBar.value = val;
    }

    public void ResumeGame()
    {
        // a button click can also call resume game instead of a only unity action so we need this
        GameManager.Instance.ResumeGame(); 
        PauseScreen.SetActive(false);
    }

    public void PauseGame(InputAction.CallbackContext obj)
    {
        PauseScreen.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Refresh()
    {
        m_healthBar.value = PlayerController.instance.m_health;
        m_soulsBar.value = GameManager.Instance.soulsCount;
    }

    public void OnGameOver(object sender, GameManager.GameOverEventArgs e)
    {
        GameOverScreen.SetActive(true);
        GameOverTextReason.GetComponent<Text>().text = e.GameOverText;
        SetAlphaOfGameOverScreen(1, 1);
        Debug.Log("Game Over");
    }

    public void OnGameWin(object sender, System.EventArgs e)
    {
        WinScreen.SetActive(true);
        Debug.Log("Game Win");
    }

    void SetAlphaOfGameOverScreen(float alpha, float duration)
    {
        GameOverScreen.GetComponent<Image>().CrossFadeAlpha(alpha, duration, true);
        GameOverText.GetComponent<Text>().CrossFadeAlpha(alpha, duration, true);
        GameOverTextReason.GetComponent<Text>().CrossFadeAlpha(alpha, duration, true);
    }
}
