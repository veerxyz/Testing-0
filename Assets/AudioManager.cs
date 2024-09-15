using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager ins;
    private AudioSource audioSource;

    public AudioClip coinCollectClip;
    public AudioClip playerHitClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;
    public AudioClip spikeBallThrowClip;

    void Awake()
    {
        if (ins == null)
        {
            ins = this;
            DontDestroyOnLoad(gameObject);       
        }

    }
      
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
     public void PlayCoinCollectSFX()
    {
        if (audioSource != null && coinCollectClip != null)
        {
            audioSource.PlayOneShot(coinCollectClip);
        }
        else
        {
            Debug.LogError("AudioSource or Coin Collect Clip not assigned.");
        }
    }
    public void PlayPlayerHitSFX()
    {
        if (audioSource != null && playerHitClip != null)
        {
            audioSource.PlayOneShot(playerHitClip);
        }
        else
        {
            Debug.LogError("AudioSource or Player Hit Clip not assigned.");
        }
    }

    public void PlayEnemyHitSFX()
    {
        if (audioSource != null && enemyHitClip != null)
        {
            audioSource.PlayOneShot(enemyHitClip);
        }
        else
        {
            Debug.LogError("AudioSource or Enemy Hit Clip not assigned.");
        }
    }

    public void PlayEnemyDeathSFX()
    {
        if (audioSource != null && enemyDeathClip != null)
        {
            audioSource.PlayOneShot(enemyDeathClip);
        }
        else
        {
            Debug.LogError("AudioSource or Enemy Death Clip not assigned.");
        }
    }

    public void PlaySpikeBallThrowSFX()
    {
        if (audioSource != null && spikeBallThrowClip != null)
        {
            audioSource.PlayOneShot(spikeBallThrowClip);
        }
        else
        {
            Debug.LogError("AudioSource or Spike Ball Throw Clip not assigned.");
        }
    }
}
