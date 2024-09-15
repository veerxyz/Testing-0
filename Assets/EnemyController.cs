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
    
    //adding a flag to avoid running MoveTowardsStandbyPosition
    private bool hasReachedStandbyPosition = false;
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
                if(!hasReachedStandbyPosition){
                MoveTowardsInitialStandbyPosition(); //or previously i named it MoveTowardsTargetZ, this only happens once when spawned
                }
                else //it has reached the standby point
                {   
                    //if its melee, we move it as per melee logic
                    if(isMelee)
                    {
                    MoveMeleeEnemyStepByStep();
                    }
                }
                break;

            case EnemyState.Attack:
                PerformAttack();
                ChangeState(EnemyState.Idle); // Reset after attack
                break;

            case EnemyState.Hit:
                // React to being hit (animations, etc.)
                if (currentHealth <= 0)
                {
                    Debug.Log("Enemy Hit-Death Triggered");
                    ChangeState(EnemyState.Death);
                }
                else
                {
                    Debug.Log("Enemy Hit-Idle Triggered");
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
        if (currentState == EnemyState.Death) return;

    if (isMelee)
    {
        // Melee enemy: Move or attack based on step count
        if (meleeStepCount < 3)
        {
            ChangeState(EnemyState.Move);
            MoveMeleeEnemyStepByStep();
        }
        else
        {
            // yo attack after 3 steps
            ChangeState(EnemyState.Attack);
            PerformAttack();
        }
    }
    else
    {
        // Ranged enemy: Attack from standby position
        ChangeState(EnemyState.Attack);
        PerformAttack();
    }
       
        Debug.Log("Enemy currentState: " + currentState);
    }

    void MoveTowardsInitialStandbyPosition()
    {
        float targetZ = 10f; // Where enemies should stop moving
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);

        // Move enemy towards target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyData.movementSpeed * Time.deltaTime);

        // Check if the enemy has reached the initial Z=10 position
        if (transform.position.z <= targetZ)
        {
            hasReachedStandbyPosition=true;
           // All enemies move to Idle after reaching the standby position
        ChangeState(EnemyState.Idle);
        
        // Notify GameManager that this enemy has reached standby, gonna use instance, remove all FindObject on Gamemanager since there is only one GameManager
        // GameManager.ins.OnEnemyReachedStandby(this);
        }
    }
void MoveMeleeEnemyStepByStep()
{
    Debug.Log($"Step Count: {meleeStepCount}, Before Step Position: {transform.position}");

    // Check if the melee enemy has completed 3 steps
    if (meleeStepCount < 3)
    {
        // Calculate player position and keep the y position locked
        Vector3 playerPosition = PlayerController.ins.transform.position;
        playerPosition.y = transform.position.y;  // Keep the y-axis constant

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        // Define a stopping distance to avoid overlap (adjust this value as needed)
        float stoppingDistance = 5f;

        // Check if the enemy is within stopping distance
        if (distanceToPlayer > stoppingDistance)
        {
            // Calculate the number of remaining steps (including the current step)
            int remainingSteps = 3 - meleeStepCount;

            // Calculate the step distance: total distance to the player / (remaining steps + 1)
            float stepDistance = (distanceToPlayer - stoppingDistance) / (remainingSteps + 1);

            // Calculate direction towards the player
            Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

            // Calculate the target position for this step
            Vector3 targetPosition = transform.position + directionToPlayer * stepDistance;

            // Move melee enemy one step closer to the player
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyData.movementSpeed * Time.deltaTime);

            // Check if the enemy has reached the target position for this step
            if (transform.position == targetPosition)
            {
                Debug.Log($"After Step Position: {transform.position}");
                meleeStepCount++; // Increment the step count
                ChangeState(EnemyState.Idle); // Change to Idle after reaching one step
            }
        }
        else
        {
            // If the enemy is within the stopping distance, stop moving and go idle
            Debug.Log("Enemy is within stopping distance.");
            ChangeState(EnemyState.Idle);
        }
    }
    else
    {
        // After completing 3 steps, we attack the player
        ChangeState(EnemyState.Attack);
    }
}

    void LookAtPlayer()
    {
        //forgot that it involves whole rotation, we dont want that, just directional change
        // Vector3 lookDirection = (FindObjectOfType<PlayerController>().transform.position - transform.position).normalized;
        // transform.rotation = Quaternion.LookRotation(lookDirection); 

         // We Lock y-axis to avoid tilt bug
    Vector3 playerPosition = FindObjectOfType<PlayerController>().transform.position;
    Vector3 lookDirection = new Vector3(playerPosition.x, transform.position.y, playerPosition.z) - transform.position;
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

    //this is called when enemy currentState is Death.
    void Die()
    {
        Debug.Log($"{enemyData.enemyName} has died!");
        
        // Play death animation, particle effects, etc.
       //instead of destroying the gameobject her, destroy in gamemanager once wave round is completed
       //and just setfalse here
        // Destroy(gameObject);
        gameObject.SetActive(false);
        if(GameManager.ins != null)
        {
            GameManager.ins.OnEnemyDeath(this);//we notify gamemanager about this enemy's death
        }
    }
    

}
