using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum PlayerState
{
    Idle,
    Shoot,
    Hit,
    Death
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController ins;
    public PlayerState currentState;
    public GameObject bulletPrefab; //assign in editor
    public Transform gunPoint;
    public int health = 100;
    // public int attackPower = 10; //initially used, but moved to bullet damage, more flexible incase we need to change player bullet or weapon.
    public Animator animator;

    public GameManager gameManager; //reference to GameManager

    private bool hasShot = false; //to avoid shooting spam, and only shoot once, per player turn.
    private void Awake()
    {

        if (ins == null)
        {
            ins = this;
        }

    }
    private void Start()
    {
        currentState = PlayerState.Idle;
        // You can find GameManager instance here or assign via inspector, or make a public instance in the gamemanager itself to access throughout, lets see what team wants.
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                // Waiting for the fight to start
                hasShot = false; //resets bool to allow shooting again.
                break;
            case PlayerState.Shoot:
                Shoot();
                break;
            case PlayerState.Hit:
                TakeHit();
                break;
            case PlayerState.Death:
                Die();
                break;
        }
    }
    #region Player Attack -> Shoot Logic along with selection of its enemy target
    private void Shoot()
    {
        if (hasShot)
        {
            return;
        }
        //projectile style since ricochet
        // Select the best target according to the target selection logic
        EnemyController targetEnemy = SelectTarget();

        if (targetEnemy != null)
        {
            Debug.Log("Shooting Bullet");
            hasShot = true;
            // Projectile-based shooting for ricochet
            GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetTarget(targetEnemy, bulletScript.damage, ricochetChance: 0.2f); // 20% chance to ricochet, and use bullet damage instead of "attackPower" of player

            // Rotate player towards the target
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            direction.y = 0;
            //rotates only on one plane, ie horizontal, there are other ways to do it also like i did in enemy, but just doing this one here so no tilt.
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            //player shooting animation here
            animator.SetTrigger("Shoot");

        }


        hasShot = true;
        currentState = PlayerState.Idle; // Go back to idle after shooting
        // Notify GameManager that the player's attack is complete
        if (gameManager != null)
        {
            gameManager.OnPlayerAttackComplete();
        }

    }
    private EnemyController SelectTarget()
    {
        List<EnemyController> rangedEnemies = new List<EnemyController>();
        List<EnemyController> meleeEnemies = new List<EnemyController>();
        List<EnemyController> readyToAttackMeleeEnemies = new List<EnemyController>();

        //we categorize them and put them in different lists
        foreach (var enemy in gameManager.enemies)
        {
            
            if (enemy.isMelee)
            {
                if (enemy.currentState != EnemyController.EnemyState.Death)
                {
                    meleeEnemies.Add(enemy);
                    // Check if the melee enemy is ready to attack (completed 3 steps)
                    if (enemy.currentState == EnemyController.EnemyState.Idle && enemy.meleeStepCount >= 3)
                    {
                        readyToAttackMeleeEnemies.Add(enemy);
                    }
                }
            }
            else if (enemy.currentState != EnemyController.EnemyState.Death)
            {
                rangedEnemies.Add(enemy);
            }
        }

        // Target selection logic
        // Check if there are any ranged enemies
        if (rangedEnemies.Count > 0)
        {
            return rangedEnemies.OrderBy(e => e.currentHealth).First();
        }
        // Check if there are melee enemies who are ready to attack and also if there are ranged enemies left
        else if (readyToAttackMeleeEnemies.Count > 0 && rangedEnemies.Count > 0)
        {
            return readyToAttackMeleeEnemies.OrderBy(e => e.currentHealth).First();
        }
        // Check if there are only melee enemies
        else if (meleeEnemies.Count > 0)
        {
            return meleeEnemies.OrderBy(e => e.currentHealth).ThenBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
        }
        //add an else case here if team wants more but first check with above conditions
        return null; // No enemies left
    }
    #endregion
    private void TakeHit()
    {
        Debug.Log("Player is taking hit");
        // Handle getting hit, reducing health, playing animation
        currentState = health > 0 ? PlayerState.Idle : PlayerState.Death;
    }

    private void Die()
    {
        // Trigger death animation and end game logic
        Debug.Log("Player has died, spawn gameover and lost screen");
    }
}

