using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFXManager : MonoBehaviour
{
    public static PFXManager ins;

    [Header("Particle Effects")]
    public ParticleSystem enemyHitEffect;
    public ParticleSystem enemyDeathEffect;
    public ParticleSystem muzzleFlashEffect;

    private void Awake()
    {
    
        if (ins == null)
        {
            ins = this;
        }
        
    }

    // I would have used separate HIT PFX as I did for Audio, separate hit for Enemy and separate hit for player, but lets see what the team would like. 
    public void PlayHitPFX(Vector3 position)
    {
        if (enemyHitEffect != null)
        {
            ParticleSystem effect = Instantiate(enemyHitEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration); //clean up after it finishes
        }
    }

    public void PlayEnemyDeathPFX(Vector3 position)
    {
        if (enemyDeathEffect != null)
        {
            ParticleSystem effect = Instantiate(enemyDeathEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration); 
        }
    }

    public void PlayMuzzleFlashPFX(Vector3 position, Quaternion rotation)
    {
        if (muzzleFlashEffect != null)
        {
            ParticleSystem effect = Instantiate(muzzleFlashEffect, position, rotation);
            Destroy(effect.gameObject, effect.main.duration);
        }
    }
}
