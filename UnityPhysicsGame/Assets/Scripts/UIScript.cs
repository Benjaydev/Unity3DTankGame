using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    private GameObject airstrikeBarObject;
    [SerializeField]
    private Image airstrikeProgressBar;
    [SerializeField]
    private TextMeshProUGUI airstrikeProgressBarText;
    private Color airstrikeBarColour;

    [SerializeField]
    private TextMeshProUGUI pointsText;
    [SerializeField]
    private TextMeshProUGUI pointMultiplierText;
    [SerializeField]
    private string pointsTextPrefix = "Points: ";

    [SerializeField]
    private Image explosiveImage;
    [SerializeField]
    private Image trackingImage;

    Dictionary<string, Image> powerupDictionary = new Dictionary<string, Image>();

    [SerializeField]
    private GameObject deathScreen;
    [SerializeField]
    private TextMeshProUGUI deathPointsText;

    [SerializeField]
    private GameObject pauseScreen;

    [System.NonSerialized]
    public bool isPaused = false;

    private void Start()
    {
        airstrikeBarColour = airstrikeProgressBar.color;

        powerupDictionary["Explosive"] = explosiveImage;
        powerupDictionary["Tracking"] = trackingImage;
    }

    public void SetIcon(string type, bool state)
    {
        powerupDictionary[type].gameObject.SetActive(state);
    }


    public void UpdateAirstrikeProgress(float percentage)
    {
        airstrikeProgressBar.fillAmount = percentage;
        if(percentage >= 1)
        {
            airstrikeProgressBar.color = Color.red;
            airstrikeProgressBarText.text = "AIRSTRIKE READY!";
        }
        else
        {
            airstrikeProgressBar.color = airstrikeBarColour;
            airstrikeProgressBarText.text = "Airstrike: " + Mathf.Round(percentage * 100) + "%";
        }
    }
    public void UpdatePoints(int points)
    {
        pointsText.text = pointsTextPrefix + points;
    }

    public void UpdatePointsMultiplier(float multiplier)
    {
        pointMultiplierText.text = (Mathf.Round(multiplier*100)/100) + "x Point Multiplier";
    }


    public void ActivateDeathScreen()
    {
        deathScreen.SetActive(true);
        pointsText.gameObject.SetActive(false);
        airstrikeBarObject.SetActive(false);

        deathPointsText.text = pointsText.text;
        Time.timeScale = 0;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Level");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetPause(bool state)
    {
        isPaused = state;
        pauseScreen.SetActive(state);
        pointsText.gameObject.SetActive(!state);
        airstrikeBarObject.SetActive(!state);

        Time.timeScale = state ? 0 : 1;

        Cursor.lockState = state ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void TogglePause()
    {
        SetPause(!isPaused);
    }

}
