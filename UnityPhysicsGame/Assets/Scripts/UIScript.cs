using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    private Image airstrikeProgressBar;
    [SerializeField]
    private TextMeshProUGUI airstrikeProgressBarText;
    private Color airstrikeBarColour;

    [SerializeField]
    private TextMeshProUGUI pointsText;
    [SerializeField]
    private string pointsTextPrefix = "Points: ";

    [SerializeField]
    private Image explosiveImage;
    [SerializeField]
    private Image trackingImage;

    Dictionary<string, Image> pointsDictionary = new Dictionary<string, Image>();

    private void Start()
    {
        airstrikeBarColour = airstrikeProgressBar.color;

        pointsDictionary["Explosive"] = explosiveImage;
        pointsDictionary["Tracking"] = trackingImage;
    }

    public void SetIcon(string type, bool state)
    {
        pointsDictionary[type].gameObject.SetActive(state);
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
}
