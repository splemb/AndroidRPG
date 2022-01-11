using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    // States of enemy behaviour
    enum EnemyState { Patrolling, Chasing, Attacking, Hurt, Dead }
    [SerializeField] EnemyState enemyState;
    bool sightActive;
    float evasionCount = 0f;

    // Stats
    [SerializeField] float maxHealth = 10f;
    float health = 10f;
    [SerializeField] float atk = 5f;
    [SerializeField] Vector3 patrolDestination;

    //References
    [SerializeField] NavMeshAgent agent;
    Transform playerTransform;
    Ray toPlayer;
    [SerializeField] LayerMask sightMask;
    [SerializeField] LayerMask attackMask;
    EnemyManager manager;
    [SerializeField] Animator animator;

    //Array of possible drops upon death
    [SerializeField] GameObject[] drops;
    [SerializeField] int dropChance = 5;
    [SerializeField] ParticleSystem deathParticles;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        health = maxHealth;
        manager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }

    private void Update()
    {
        //Only check to see player within a certain range
        toPlayer = new Ray(transform.position, new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z) - transform.position);
        if (Vector3.Distance(transform.position, playerTransform.position) < 10f) sightActive = true;
        else sightActive = false;

        //Different behaviour dependent on state
        switch (enemyState)
        {
            case EnemyState.Patrolling:
                //Moves from waypoint to waypoint
                Patrol();
                if (sightActive) Sight();
                evasionCount = 0f;
                break;
            case EnemyState.Chasing:
                //Follows player
                agent.destination = playerTransform.position;
                EvasionCounter();
                if (Vector3.Distance(playerTransform.position, transform.position) < 2 * transform.localScale.y) Attack();
                break;
            case EnemyState.Hurt:
                //Stops moving
                agent.destination = transform.position;
                break;
            case EnemyState.Dead:
                agent.destination = transform.position;
                break;
        }
    }

    

    void Patrol()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, patrolDestination)) < 5f || patrolDestination == Vector3.zero)
        {
            patrolDestination = manager.RequestDestination();
            
        }

        agent.destination = patrolDestination;
    }

    //Reduces hp by the given amount, with optional paramater to ignore temporary invincibility in the hurt state
    public void Damage(float amt = 1f, bool overrideInvinciblity = false)
    {
        if (enemyState == EnemyState.Hurt && !overrideInvinciblity) return;
        GetComponent<AudioSource>().Stop();
        enemyState = EnemyState.Hurt;
        animator.SetBool("Squish", true);

        health -= amt;
        if (health <= 0) { StopAllCoroutines(); StartCoroutine(WaitToRespawn(5f)); return; }
        else StartCoroutine(StateTimer(.8f, EnemyState.Chasing));
        
    }

    //Stops moving and attacks in front of it when in range of the player, not guaranteed to hit
    void Attack()
    {
        if (enemyState == EnemyState.Dead) return;
        enemyState = EnemyState.Attacking;
        animator.SetTrigger("Attack");
        StartCoroutine(StateTimer(2f, EnemyState.Chasing));
        agent.destination = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position - (Vector3.up * transform.localScale.y / 2), toPlayer.direction, out hit, 3f * transform.localScale.y, attackMask))
        {
            playerTransform.GetComponent<TouchMovement>().Hurt(atk);
        }
    }

    //Creates a cone of vision in which the player can be detected by the enemy
    void Sight()
    {
        
        Debug.DrawRay(transform.position, new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z) - transform.position);
        float dotProduct = Vector3.Dot(toPlayer.direction, transform.forward);

        if (dotProduct > 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(toPlayer, out hit, 10f, sightMask))
            {
                //Debug.Log(hit.transform.tag);
                if (hit.transform.tag == "Player")
                {
                    enemyState = EnemyState.Chasing;
                    evasionCount = 0f;
                    Debug.DrawRay(transform.position, new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z) - transform.position);
                }
            }
        }

        
    }

    //Counts up whenever the player is not in view and stops chasing when it reaches its limit, resets everytime player is spotted again
    void EvasionCounter()
    {
        RaycastHit hit;
        if (Physics.Raycast(toPlayer, out hit, 10f, sightMask))
        {
            //Debug.Log(hit.transform.tag);
            if (hit.transform.tag == "Player")
            {
                evasionCount = 0;
                return;
            }
        }
        evasionCount += Time.deltaTime;
        if (evasionCount > 3f) enemyState = EnemyState.Patrolling;
    }

    //Delays the change to the next state by a given amount of time
    IEnumerator StateTimer(float waitTime, EnemyState newState)
    {
        yield return new WaitForSeconds(waitTime);
        enemyState = newState;
        animator.SetBool("Squish", false);
        GetComponent<AudioSource>().Play();
    }

    //Enemies use pooling to avoid instantiating too many objects
    //Upon death, they reset all values and respawn at the furthest waypoint from the player

    IEnumerator WaitToRespawn(float waitTime)
    {
        
        evasionCount = 0f;
        sightActive = false;
        enemyState = EnemyState.Dead;
        deathParticles.Play();
        if (Random.Range(0, dropChance) == 0) Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
        animator.gameObject.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(waitTime);
        Respawn();
    }

    private void Respawn()
    {
        animator.gameObject.SetActive(true);
        GetComponent<CapsuleCollider>().enabled = true;
        transform.position = manager.RequestRespawn();
        health = maxHealth;
        evasionCount = 0f;
        enemyState = EnemyState.Patrolling;
        animator.SetBool("Squish", false);
    }
}
