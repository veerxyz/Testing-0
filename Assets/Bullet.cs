using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float ricochetChance = 0.2f; // 20% chance to ricochet
    public float lifetime = 5f; // Time before the bullet is destroyed

    private EnemyController currentTarget;
    private bool hasRicocheted = false;
    // Start is called before the first frame update
     void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the bullet after a certain time
    }

    public void SetTarget(EnemyController target, int damage, float ricochetChance)
    {
        this.currentTarget = target;
        this.damage = damage;
        this.ricochetChance = ricochetChance;
    }


    // Update is called once per frame
   void Update()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(currentTarget.transform);

            // Check if the bullet hits the target
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < 0.1f)
            {
                HitTarget();
            }
        }
    }
    private void HitTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(damage); //applying bullet damage to the enemy it touches
            // Check for ricochet
            if (!hasRicocheted && Random.value <= ricochetChance)
            {
                Ricochet();
            }
            else
            {
                Destroy(gameObject); // Destroy the bullet if it doesn't ricochet
            }
        }
    }

    private void Ricochet()
    {
        hasRicocheted = true;
        EnemyController newTarget = FindNewTarget();
        if (newTarget != null)
        {
            currentTarget = newTarget;
            // Change the bullet direction to the new target
            transform.LookAt(currentTarget.transform);
        }
        else
        {
            Destroy(gameObject); // Destroy the bullet if no new target found
        }
    }

    private EnemyController FindNewTarget()
    {
        // Find all enemies except the current target
        List<EnemyController> enemies = FindObjectOfType<GameManager>().enemies; //using GameManager instance would be preferred instead of finding object but just just one gamemanager so typing it as it flows for now.
        enemies.Remove(currentTarget);

        if (enemies.Count > 0)
        {
            return enemies[Random.Range(0, enemies.Count)];
        }

        return null; // No new target found
    }

}
