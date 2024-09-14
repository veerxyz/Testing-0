using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyData> enemyTypes; // List of enemy types from ScriptableObjects

private List<Vector3> spawnedPositions = new List<Vector3>();
private float minDistanceBetweenEnemies = 2.0f; // Adjust this value as needed, to avoid overlapping of enemy spawns location/position.

    public void SpawnEnemies(int count, List<EnemyController> enemyList)
    {
        for (int i = 0; i < count; i++)
        {
            EnemyData selectedEnemyData = enemyTypes[Random.Range(0, enemyTypes.Count)];
            GameObject enemyObject = Instantiate(selectedEnemyData.enemyPrefab, GetSpawnPosition(), Quaternion.identity);
            EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                // Initialize the enemy controller with data from ScriptableObject
                enemyController.enemyData = selectedEnemyData;
                enemyList.Add(enemyController);

                //to move enemy from outside of screen to its right side of the screen,
                //we need to set it to move state as it spawns.
                enemyController.ChangeState(EnemyController.EnemyState.Move);

            }
            else
            {
                Debug.LogError("Enemy prefab does not have an EnemyController component. Please add it");
            }
        }
    }

    // private Vector3 GetSpawnPosition()
    // {
    // return new Vector3(Random.Range(10f, 15f), 0f, 0f); // Spawn outside screen
    // Define x range for random positioning in front of the player
    // float randomX = Random.Range(-5f, 5f); // Adjust these values based on how wide you want the spawn area
    // float zSpawn = 20f; // Outside of screen, z = 20
    // return new Vector3(randomX, 2f, zSpawn); // Spawns at z=20 with random x positions
    // }
    private Vector3 GetSpawnPosition() //this one i created to avoid enemy spawn overlapping compared to blindly spawning, we can refine more as per your wish.
    {
        Vector3 newPosition = Vector3.zero; //i initialize to avoid compiler confusion error, because its used in a conditional while loop below.
        bool validPosition = false;

        while (!validPosition)
        {
            float randomX = Random.Range(-5f, 5f);
            float zSpawn = 20f;
            newPosition = new Vector3(randomX, 2f, zSpawn);

            validPosition = true;
            foreach (var position in spawnedPositions)
            {
                if (Vector3.Distance(newPosition, position) < minDistanceBetweenEnemies)
                {
                    validPosition = false;
                    break;
                }
            }
        }

        spawnedPositions.Add(newPosition);
        return newPosition;
    }
}