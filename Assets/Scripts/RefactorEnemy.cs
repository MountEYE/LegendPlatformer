using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefactorEnemy : MonoBehaviour
{
    public Stats enemyStats;
    public Transform sight;
    public Transform[] patrolPoints;
    public GameObject enemyExplosionParticles;

    private int currentPatrolPoint = 0;
    private bool slipping = false;
    private float facing;
    private Rigidbody rb;
    private GameObject player;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (enemyStats.idle)
        {
            Patrol();
        }
        else
        {
            ChasePlayer();
        }

        if (slipping)
        {
            StopFollowingPlayer();
        }
    }

    private void Patrol()
    {
        Vector3 moveToPoint = patrolPoints[currentPatrolPoint].position;
        transform.position = Vector3.MoveTowards(transform.position, moveToPoint, enemyStats.walkSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveToPoint) < 0.01f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(sight);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);

        if (Vector3.Distance(transform.position, player.transform.position) < enemyStats.explodeDist)
        {
            StartCoroutine(Explode());
            enemyStats.idle = true;
        }
    }

    private void StopFollowingPlayer()
    {
        transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision other)
    {
        slipping = (other.gameObject.layer == 9);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            enemyStats.idle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyStats.idle = true;
        }
    }

    private IEnumerator Explode()
    {
        GameObject particles = Instantiate(enemyExplosionParticles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Destroy(transform.parent.gameObject);
    }

    [System.Serializable]
    public struct Stats
    {
        public float walkSpeed;
        public float rotateSpeed;
        public float chaseSpeed;
        public bool idle;
        public float explodeDist;
    }
}