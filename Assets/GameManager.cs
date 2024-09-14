//git on
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public EnemySpawner enemySpawner;
    public List<EnemyController> enemies = new List<EnemyController>(); //Store list of enemies spawned per wave
    public int totalWaves = 3;
    public float waveInterval = 5f; //timer start after all enemies are dead in current wave and there are more waves.
    private int currentWave = 0;

    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        Idle,
        GameOver,
        GameWon
    }
    public GameState currentState = GameState.Idle;
    private void Start()
    {
        //fight button would come here.
        StartGame();
    }

    private void StartGame()
    {
        Debug.Log("Game has started");
        currentWave = 0;
        StartNextWave();
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

        //clear the list of enemies of current/previous wave
        enemies.Clear();

        int enemyCount = Random.Range(1, 4); //spawn b/w 1-3 enemies per wave. change this to public so in editor our team can assign easily.
        enemySpawner.SpawnEnemies(enemyCount, enemies); // Use EnemySpawner
        //set state to idle until enemies reach initial target z position from spawn point.
        currentState = GameState.Idle;

        Invoke(nameof(CheckEnemiesReachedTarget), waveInterval);
    }
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
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.currentState != EnemyController.EnemyState.Death)
            {
                enemy.TakeTurn(); // Move or Attack based on enemy type
                yield return new WaitForSeconds(1f); // Delay between enemy turns
            }
        }

        // After all enemies take their turn, check if player is still alive and if we need to spawn next wave
        if (player.health <= 0)
        {
            EndGame(false); // Player lost
        }
        else if (currentWave >= totalWaves && AreAllEnemiesDefeated())
        {
            EndGame(true); // Player wins if all waves are done and enemies are defeated
        }
        else
        {
            //ensuring next wave is called only when current wave is over and all are defeated.
            if(AreAllEnemiesDefeated())
            {
            StartNextWave(); // Proceed to next wave
            }
        }
    }
    #endregion
    private bool AreAllEnemiesDefeated()
    {
        // Check if there are no more enemies alive in the list 'enemies' since even if they die they are not removed from the list, which i clear manually in next wave.

        foreach (EnemyController enemy in enemies)
        {
            if (enemy.currentState != EnemyController.EnemyState.Death)
            {
                return false; // If any enemy is not in Death state, return false
            }
        }
        return true; // All enemies are defeated if the loop completes
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

