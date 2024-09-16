using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarSprite;

    private Camera mainCam;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBarSprite.fillAmount = currentHealth / maxHealth;

        // Changing Health Color depending on health left.
        if (healthBarSprite.fillAmount > 0.5f)
        {
            healthBarSprite.color = Color.green;
        }
        else if (healthBarSprite.fillAmount > 0.3f)
        {
            healthBarSprite.color = Color.yellow;
        }
        else
        {
            healthBarSprite.color = Color.red;
        }

        //disable if player dies, aka, health = 0, or fillamount is 0.
        if (healthBarSprite.fillAmount <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            healthBarSprite.gameObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }

    //below is just for debugging some UI thing, aka, UI debugs.     
    //public void UpdateHealthBar(float currentHealth, float maxHealth)
    // {
    //     if (maxHealth == 0)
    //     {
    //         Debug.LogError("Max health cannot be zero.");
    //         return;
    //     }

    //     float healthPercentage = currentHealth / maxHealth;

    //     // Check for NaN or invalid values
    //     if (float.IsNaN(healthPercentage) || healthPercentage < 0 || healthPercentage > 1)
    //     {
    //         Debug.LogError($"Invalid health percentage: {healthPercentage}");
    //         return;
    //     }

    //     healthBarSprite.fillAmount = healthPercentage;
    // }

}
