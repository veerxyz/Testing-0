using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public bool isMelee;
    public float movementSpeed;
    public int health;
    public int attackPower;
    public GameObject enemyPrefab;
}
