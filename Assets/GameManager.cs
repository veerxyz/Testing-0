//git on
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager ins; // since we have one gamemanager in whole game, we use it as singleton
    public PlayerController player;
    public EnemySpawner enemySpawner;
    public List<EnemyController> enemies = new List<EnemyController>(); //Store list of enemies spawned per wave
    public int totalWaves = 3;
    public float waveInterval = 5f; //timer start after all enemies are dead in current wave and there are more waves.
    private int currentWave = 0;
    public int minEnemyCountPerWave = 1;
    public int maxEnemyCountPerWave_Exclusive = 4;
    public Button fightButton; //Reference to the Fight Button
    private int enemiesReachedStandby = 0;


    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        Idle,
        GameOver,
        GameWon
    }
    public GameState currentState = GameState.Idle;

    private void Awake() {
    
        if (ins == null)
        {
            ins = this;
        }
    
    }
    private void Start()
    {
        //fight button would come here.
        // StartGame(); //the fight button should invoke this method.
        fightButton.gameObject.SetActive(true);  // Show Fight Button at the start
        fightButton.onClick.AddListener(OnFightButtonClick);  // Listen for button click
    }
    private void OnFightButtonClick()
    {
        fightButton.gameObject.SetActive(false); //we hide fight button after its clicked
        StartGame();
    }

    private void StartGame()
    {
        Debug.Log("Game has started");
        currentWave = 0;
        StartNextWave();
    }
    private void Update()
    {
        Debug.Log("Current State of Game is " + currentState);
    }
    private void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            EndGame(true); // No more waves, player wins if alive
            return;
        }
        Debug.Log("Starting Wave " + (currentWave + 1));
        currentWave++;

        // to destroy all the GameObjects(enemies) since we disabled them in Die() instead of destroying it to simplify list modifications and reads.
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        //clear the list of enemies of current/previous wave
        enemies.Clear();
        enemiesReachedStandby = 0;


        int enemyCount = Random.Range(minEnemyCountPerWave, maxEnemyCountPerWave_Exclusive); //spawn b/w 1-3 enemies per wave. change this to public so in editor our team can assign easily.
        enemySpawner.SpawnEnemies(enemyCount, enemies); // Use EnemySpawner

        //set game state to idle until enemies reach initial target z position from spawn point.
        currentState = GameState.Idle;

        Invoke(nameof(CheckEnemiesReachedTarget), waveInterval);
    }
    
    //this checks individually, and is called from enemycontroller>MoveTowardsTargetZ
    // public void OnEnemyReachedStandby(EnemyController enemy)
    // {
    //     enemiesReachedStandby++;

    //     // Check if all enemies have reached their standby positions
    //     if (enemiesReachedStandby == enemies.Count)
    //     {
    //         StartPlayerTurn();
    //     }
    // }
    //checks all at once
    private void CheckEnemiesReachedTarget()
    {
        bool allEnemiesReady = true;

        // Check if all enemies have reached their target position (z=10)
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.transform.position.z > 10f) // Enemies should reach Z=10
            {
                allEnemiesReady = false;
                break;
            }
        }

        if (allEnemiesReady)
        {
            StartPlayerTurn();
        }
        else
        {
            Invoke(nameof(CheckEnemiesReachedTarget), 1f); // Check again after 1 second
        }
    }
    #region PlayerTurn
    private void StartPlayerTurn()
    {
        Debug.Log("Player's Turn");

        // Set the game state to PlayerTurn
        currentState = GameState.PlayerTurn;
        player.currentState = PlayerState.Shoot;
    }
    public void OnPlayerAttackComplete()
    {
        Debug.Log("Player attack complete");
        // After the player finishes their turn, start the enemies' turn.
        //perhaps this one is less clear, if you want enemies to start when they are taking damage,
        //or wanna just start as player hits them? need to discuss with team.
        StartEnemyTurn();
    }
    #endregion

    #region EnemyTurn
    private void StartEnemyTurn()
    {
        Debug.Log("Enemies' Turn");

        currentState = GameState.EnemyTurn;

        // Enemies take their turns one by one
        StartCoroutine(EnemyTurnRoutine());
    }
    private IEnumerator EnemyTurnRoutine()
    {
    
        //we use this implementation of creating a copy of that list, like a snapshotCreate a copy of the enemies list to safely iterate/modify through it;
        List<EnemyController> enemiesCopy = new List<EnemyController>(enemies);

        foreach (EnemyController enemy in enemiesCopy)
        {
            
            if (enemy != null && enemy.currentState != EnemyController.EnemyState.Death)
            {
                enemy.TakeTurn(); // Move or Attack based on enemy type
                yield return new WaitForSeconds(1f); // Delay between enemy turns
            }
          
        }
        CheckGameStatus(); //after enemy turn is complete, we check if the player has lost or not, or to move to next.
    }
    #endregion
    

    private void CheckGameStatus()
    {

        // After all enemies take their turn, check if player is still alive and if we need to spawn next wave
        if (player.health <= 0)
        {
            EndGame(false); // Player lost
        }
        else if (currentWave >= totalWaves && AreAllEnemiesDefeated())
        {
            EndGame(true); // Player wins if all waves are done and enemies are defeated
        }
        else if (AreAllEnemiesDefeated())
        {
            StartNextWave(); // Proceed to next wave
        }
        else
        {
            // Otherwise, start the player's next turn
            StartPlayerTurn();
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        // Check if there are no more enemies alive in the list 'enemies' since even if they die they are not removed from the list, which i clear manually in next wave.
         
        foreach (EnemyController enemy in enemies)
        {
            if (enemy !=null && enemy.currentState != EnemyController.EnemyState.Death)
            {
                Debug.Log("AREALLENEMIESDEFEATED? enemy state is" + enemy.currentState );
                return false; // If any enemy is not in Death state, return false
            }
        }
        Debug.Log("AREALLENEMIESDEFEATED? all enemies are defeated");
        return true; // All enemies are defeated if the loop completes
    }
    public void OnEnemyDeath(EnemyController enemy)
    {
        if (enemy != null)
        {
           //mark the enemy dead, just double check, but dont remove from the list.
           enemy.currentState = EnemyController.EnemyState.Death;
        Debug.Log($"Enemy {enemy.gameObject.name} is dead.");
            
            // Destroy(enemy.gameObject, 1); //we can destroy the enemy if it's no longer needed
          
        }
         else
        {
            Debug.LogWarning($"Enemy {enemy.gameObject.name} was not found in the list.");
        }
    }
    private void EndGame(bool won)
    {
        currentState = won ? GameState.GameWon : GameState.GameOver;

        if (won)
        {
            Debug.Log("Player Wins!");
            // Show win panel, end game logic
        }
        else
        {
            Debug.Log("Player Lost!");
            // Show lose panel, end game logic
        }
    }

}

