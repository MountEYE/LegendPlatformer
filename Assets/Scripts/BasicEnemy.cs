using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        [Header("Enemy Settings")]
        public float walkSpeed;
        public float rotateSpeed;
        public float chaseSpeed;
        public bool idle;
    }

    public Stats enemyStats;
    public Transform target;
    public AudioClip alertedSound;
    public AudioClip attackSound;

    private AudioSource audioSource;
    private bool alerted = false;
    private bool slipping = false;
    private float facing;
    private Rigidbody rb;
    private GameObject player;
    private bool canAttack = true; // Flag to control attack frequency

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check if the player GameObject is not null before updating
        if (player != null)
        {
            if (enemyStats.idle == true)
            {
                transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * enemyStats.rotateSpeed, Space.Self);
                transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * enemyStats.walkSpeed, Space.Self);
            }
            else if (enemyStats.idle == false)
            {
                target.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                transform.LookAt(target);
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);
            }

            // stops enemy from following player up the inaccessible slopes
            if (slipping == true)
            {
                transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 9)
        {
            slipping = true;
        }
        else
        {
            slipping = false;
        }

        if (other.gameObject.CompareTag("Player") && canAttack)
        {
            audioSource.PlayOneShot(attackSound);

            // Check if the player has the GetHit script attached
            GetHit playerGetHit = other.gameObject.GetComponent<GetHit>();
            if (playerGetHit != null)
            {
                playerGetHit.TakeDamage();
            }

            // Set cooldown to prevent frequent attacks
            StartCoroutine(AttackCooldown());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
            enemyStats.idle = false;

            if (alerted == false)
            {
                alerted = true;
                audioSource.PlayOneShot(alertedSound);
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.0f); // Adjust the cooldown duration as needed
        canAttack = true;
    }
}