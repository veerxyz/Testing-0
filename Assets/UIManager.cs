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
    public Canvas canvas; //so we can make coin prefab child of this on instantiate, assign this in inspector.
    private RectTransform canvasRectTransform;
    public RectTransform coinCounterRectTransform;
    public GameObject testCubePrefab;

    public Camera mainCam;
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
        // coinCounterRectTransform = coinCounterText.GetComponent<RectTransform>(); // we get RectTransform of coin counter for coin end anim

        mainCam = Camera.main;
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
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

    // Call this function from EnemyController.cs when an enemy dies
    public void SpawnCoinAtEnemyPosition(Vector3 enemyWorldPosition)
    {
        //instantiate the coin at the canvas (initially at an arbitrary position)
        GameObject coin = Instantiate(coinPrefab.gameObject, canvasRectTransform);
        RectTransform coinRectTransform = coin.GetComponent<RectTransform>();

        //convert the enemy world position to screen space
        Vector2 screenPos = mainCam.WorldToScreenPoint(enemyWorldPosition);
        // GameObject testCubePrefabclone = Instantiate(testCubePrefab, new Vector3(screenPos.x, screenPos.y, 0), Quaternion.identity);

        //convert screen position to canvas space (anchored position)
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCam, out anchoredPos);

        //set the coin's anchored position to the calculated canvas space position
        coinRectTransform.anchoredPosition = anchoredPos;

        //animate the coin to the coin counter position using DOTween
        coinRectTransform.DOMove(coinCounterRectTransform.position, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            // After animation completes, destroy the coin object
            Destroy(coin);
            ShakeCoinAtEndOfCoinWorldToCanvasAnim();
        });
    }
    private void ShakeCoinAtEndOfCoinWorldToCanvasAnim()
    {
        if (coinCounterRectTransform != null)
        {
            // Shake effect
            coinCounterRectTransform.DOShakePosition(0.5f, 10f, 10, 90, false, true);
        }
    }


}

