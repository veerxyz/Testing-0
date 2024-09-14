using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // [Header("Dont assign 'Enemy Data' or 'Is Melee'" publicly here)]
    [Header("Enemy State")]
    [HideInInspector] public EnemyData enemyData; // Reference to the , probably assign it in EnemySpawner.
    
    //defining isMelee, which we will take from our enemydata SO.
    [HideInInspector] public bool isMelee; //made it public to be accesibble to other scripts.

    
    public int currentHealth;
    

    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Hit,
        Death
    }
    public EnemyState currentState;

    

    public int meleeStepCount = 0; // used public just like the rest, but prefer getter and setter for better encapsulation. For now it is just direct public.

    void Start()
    {
        if (enemyData != null)
        {
            currentHealth = enemyData.health;
            isMelee = enemyData.isMelee;
        }
        //commented below line coz now I control initial enemy state in EnemySpawner
        // currentState = EnemyState.Idle; // Start in Idle state
    }

    void Update()
    {
        LookAtPlayer();

        //state logic below
        switch (currentState)
        {
            case EnemyState.Idle:
                // Wait for the player's turn or further instructions
                break;

            case EnemyState.Move:
                //moves all type of enemies, and later once they reach
                //their attack position, proceed according to their nature of movement.
                MoveTowardsTargetZ();
                break;

            case EnemyState.Attack:
                PerformAttack();
                ChangeState(EnemyState.Idle); // Reset after attack
                break;

            case EnemyState.Hit:
                // React to being hit (animations, etc.)
                if (currentHealth <= 0)
                {
                    ChangeState(EnemyState.Death);
                }
                else
                {
                    ChangeState(EnemyState.Idle);
                }
                break;

            case EnemyState.Death:
                Die();
                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    // void MoveTowardsPlayer()
    // {
    //     transform.Translate(Vector3.forward * enemyData.movementSpeed * Time.deltaTime);
    // }
    //to handle enemy's turn logic
    public void TakeTurn()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // Optionally handle Idle state during enemy's turn
                break;

            case EnemyState.Move:
                MoveTowardsTargetZ();
                break;

            case EnemyState.Attack:
                PerformAttack();
                ChangeState(EnemyState.Idle); // Reset after attack
                break;

            case EnemyState.Hit:
                // Handle being hit if necessary
                break;

            case EnemyState.Death:
                // Handle death if necessary
                break;
        } 
    }

    void MoveTowardsTargetZ()
    {
        float targetZ = 10f; // Where enemies should stop moving
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);

        // Move enemy towards target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyData.movementSpeed * Time.deltaTime);

        // Check if the enemy has reached the initial Z=10 position
        if (transform.position.z <= targetZ)
        {
            if (enemyData.isMelee)
            {
                // If it's a melee enemy, move step-by-step towards the player
                MoveMeleeEnemyStepByStep();
            }
            else
            {
                // Ranged enemies attack immediately after reaching Z=10
                ChangeState(EnemyState.Attack);
            }
        }
    }
    void MoveMeleeEnemyStepByStep()
    {
        // Check if the melee enemy has completed 3 steps
        if (meleeStepCount < 3)
        {
            // Calculate distance between melee enemy and player
            Vector3 playerPosition = FindObjectOfType<PlayerController>().transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

            // Divide the total distance by 3 to get the distance for each step
            float stepDistance = distanceToPlayer / (3 - meleeStepCount);  // Remaining steps

            // Target position for the next step
            Vector3 targetPosition = Vector3.MoveTowards(transform.position, playerPosition, stepDistance);

            // Move melee enemy one step closer to the player
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyData.movementSpeed * Time.deltaTime);

            meleeStepCount++; // Increment the step count

            // Wait for the next turn to move another step
            ChangeState(EnemyState.Idle);
        }
        else
        {
            // After completing 3 steps, we attack the player
            ChangeState(EnemyState.Attack);
        }
    }
    void LookAtPlayer()
    {
        Vector3 lookDirection = (FindObjectOfType<PlayerController>().transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
    void PerformAttack()
    {
        // Execute attack logic, e.g., ranged or melee attack
        if (!enemyData.isMelee)
        {
            // Ranged attack logic
            Debug.Log($"{enemyData.enemyName} performed ranged attack!");
        }
        else
        {
            // Melee attack after 3 steps
            Debug.Log($"{enemyData.enemyName} performed melee attack!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} took fatal damage. Transitioning to Death state.");
            ChangeState(EnemyState.Death); // Switch to Death state when health is 0
        }
        else
        {
            ChangeState(EnemyState.Hit); // Switch to Hit state if still alive
        }
    }

    void Die()
    {
        Debug.Log($"{enemyData.enemyName} has died!");
        // Play death animation, particle effects, etc.
        Destroy(gameObject);
    }

}
