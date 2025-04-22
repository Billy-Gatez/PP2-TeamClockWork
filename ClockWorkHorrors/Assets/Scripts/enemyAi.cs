//Mark Bennett 04/2025 & Jeremy Cahill - Full Sail University - Portfolio 2 - Game Dev - Rod Moye

using UnityEngine;
using System.Collections;
using UnityEngine.AI; 

public class enemyAI : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;


    public int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] int numberOfPatrolPoints = 5; // Number of patrol points to generate
    [SerializeField] float patrolAreaSize = 20f; // Size of the patrol area (20x20)
    private Vector3[] patrolPoints; // Array to hold patrol points
    private int currentPatrolIndex = 0; // Current patrol point index
    [SerializeField] float patrolSpeed = 2f; // Speed for patrolling
    [SerializeField] float detectionRange = 10f; // Range to detect the player

    bool playerInRange;

    float shootTimer;


    Color colorOrig;

    Vector3 playerDir;

   
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1, 0);
        agent.speed = patrolSpeed; // Set the agent speed for patrolling

        
        // Generate patrol points within a 20x20 area
        GeneratePatrolPoints();
    }
    void Update()
    {
        if (playerInRange)
        {
            playerDir = (gamemanager.instance.player.transform.position - transform.position);
            agent.SetDestination(gamemanager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            shootTimer += Time.deltaTime;

            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
        else
        {
            Patrol();
        }
    }

    void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[numberOfPatrolPoints];

        for (int i = 0; i < numberOfPatrolPoints; i++)
        {
            // Generate random patrol points within the specified area
            float x = Random.Range(-patrolAreaSize / 2, patrolAreaSize / 2);
            float z = Random.Range(-patrolAreaSize / 2, patrolAreaSize / 2);
            patrolPoints[i] = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        }
    }
    void Patrol()
    {
        // Check if there are any patrol points assigned
        if (patrolPoints.Length == 0)
        {
            
            return; // Exit the method if there are no patrol points
        }

        // Move towards the current patrol point
        Vector3 targetPosition = patrolPoints[currentPatrolIndex];

        // Check if we have reached the patrol point
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Move to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        // Check distance to player to see if they should engage
        float distanceToPlayer = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);
        if (distanceToPlayer <= detectionRange)
        {
            playerInRange = true; // Engage player if within range
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }

    }


   public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1, XP);
            gamemanager.instance.updateCurrency(XP); 
            Destroy(gameObject); 
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    void faceTarget()
    {

        Quaternion rot = Quaternion.LookRotation(new Vector3 (playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
