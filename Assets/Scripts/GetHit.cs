using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GetHit : MonoBehaviour
{
    [Tooltip("Determines when the player is taking damage.")]
    public bool hurt = false;
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthSlider; // Reference to the UI health slider
    public Text healthText; // Reference to the UI text displaying health

    private bool slipping = false;
    private PlayerMovement playerMovementScript;
    private Rigidbody rb;
    private Transform enemy;

    private void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        // Initialize UI health slider and text
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            UpdateHealthText();
        }
    }

    private void FixedUpdate()
    {
        // stops the player from running up the slopes and skipping platforms
        if (slipping)
        {
            transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
            playerMovementScript.playerStats.canMove = false;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!hurt)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                enemy = other.gameObject.transform;
                rb.AddForce(enemy.forward * 1000);
                rb.AddForce(transform.up * 500);
                TakeDamage();
            }
            else if (other.gameObject.CompareTag("Trap"))
            {
                rb.AddForce(transform.forward * -1000);
                rb.AddForce(transform.up * 500);
                TakeDamage();
            }
        }

        if (other.gameObject.layer == 9)
        {
            slipping = true;
        }

        if (other.gameObject.layer != 9)
        {
            if (slipping)
            {
                slipping = false;
                playerMovementScript.playerStats.canMove = true;
            }
        }
    }

    public void TakeDamage()
    {
        hurt = true;
        playerMovementScript.playerStats.canMove = false;
        playerMovementScript.soundManager.PlayHitSound();

        // Decrease health
        currentHealth -= 20; // Adjust the damage value as needed

        // Update UI health bar and text
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            UpdateHealthText();
        }

        if (currentHealth <= 0)
        {
            // Reset the current scene when health reaches 0
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(Invincibility());
            StartCoroutine(Recover());
        }
    }

    // New method to increase health
    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;

        // Ensure health does not exceed the maximum
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // Update UI health bar and text
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            UpdateHealthText();
        }
    }

    private void UpdateHealthText()
    {
        // Update the text displaying health
        healthText.text = "Health: " + currentHealth.ToString();
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(2f); // 2 seconds invincibility
        hurt = false;
        playerMovementScript.playerStats.canMove = true;
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.75f);
        hurt = false;
        playerMovementScript.playerStats.canMove = true;
    }
}