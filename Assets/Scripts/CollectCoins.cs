using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoins : MonoBehaviour
{
    [Tooltip("The particles that appear after the player collects a coin.")]
    public GameObject coinParticles;

    PlayerMovement playerMovementScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerMovementScript = other.GetComponent<PlayerMovement>();
            GetHit playerGetHit = other.GetComponent<GetHit>();

            if (playerGetHit != null)
            {
                // Increase player's health by 10
                playerGetHit.IncreaseHealth(10);
            }

            playerMovementScript.soundManager.PlayCoinSound();
            ScoreManager.score += 10;
            GameObject particles = Instantiate(coinParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}