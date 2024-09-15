using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    
    public int damage = 10;
    public float speed = 2f;           // Horizontal speed of the projectile
    public float arcHeight = 0.5f;        // Height of the arc
    private Vector3 target;             // Target position (player's position)
    private Vector3 startPosition;      // Start position of the spike ball
    private float journeyLength;        // Total distance to the target
    private float startTime;            // Time when the spike was thrown

    public void Initialize(Vector3 targetPosition, Vector3 startSpawnPosition)
    {
        target = targetPosition;
        // startPosition = transform.position; //was testing with this, but gonna use Start Spawn Point
        startPosition = startSpawnPosition;
        journeyLength = Vector3.Distance(startPosition, target); // Calculate total distance to the target
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the fraction of the journey completed based on time
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        // Calculate current position along the straight line path
        Vector3 currentPosition = Vector3.Lerp(startPosition, target, fractionOfJourney);

        // Add a parabolic arc to the y position
        float height = Mathf.Sin(Mathf.PI * fractionOfJourney) * arcHeight;
        currentPosition.y += height;

        // Set the position of the spike ball
        transform.position = currentPosition;

        // Check if it has reached the target position (player)
        if (fractionOfJourney >= 1f)
        {
            OnHitPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnHitPlayer();
        }
    }

    void OnHitPlayer()
    {
        // Apply damage to the player
        PlayerController.ins.TakeDamage(damage);

        // Destroy the spike ball after hitting the player
        Destroy(gameObject);
    }
}
