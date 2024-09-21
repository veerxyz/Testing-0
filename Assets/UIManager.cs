using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UIManager : MonoBehaviour
{
    public static UIManager ins;
    public GameObject mainPanel;
    public GameObject playPanel;
    public GameObject gameoverPanel;

    public GameObject debugPanel;
    public TextMeshProUGUI gameoverText;
    public TextMeshProUGUI coinCounterText;
    public TextMeshProUGUI gameStateText;
    public RectTransform coinPrefab;
    public RectTransform canvasRectTransform; //so we can make coin prefab child of this on instantiate, assign this in inspector.
    private RectTransform coinCounterRectTransform;
    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
    }
    void Start()
    {
        // ShowMainPanel(); // Show MainPanel on start, control from gamemanager
        coinCounterRectTransform = coinCounterText.GetComponent<RectTransform>(); // we get RectTransform of coin counter
   
    }
    public void ShowDebugPanel()
    {
        debugPanel.SetActive(true);
    }
    public void HideDebugPanel()
    {
        debugPanel.SetActive(false);
    }
    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        playPanel.SetActive(false);
        gameoverPanel.SetActive(false);
    }

    public void ShowPlayPanel()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        gameoverPanel.SetActive(false);
    }

    //Call this from GameManager and pass the "txt" accordingly
    public void ShowGameOverPanel(string txt)
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(false);
        gameoverPanel.SetActive(true);
        gameoverText.text = txt;

    }

    //to update coin counter text and display, Enemy Dies, we get coin > GameManager updates > UIManager displays
    public void UpdateCoinCounter(int coinCount)
    {
        if (coinCounterText != null)
        {
            coinCounterText.text = coinCount.ToString();
            AudioManager.ins.PlayCoinCollectSFX();
            //tween shake effect.
            ShakeCoinCounter();
        }
    }

    private void ShakeCoinCounter()
    {
        if (coinCounterText != null)
        {
            // Shake effect parameters: duration, strength, vibrato, randomness
            coinCounterText.transform.DOShakePosition(0.5f, 10f, 10, 90, false, true);
        }
    }
    // now to animate coin from enemy position to coin counter..


}

