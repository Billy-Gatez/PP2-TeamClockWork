//Mark Bennett 04/2025 & Jeremy Cahill - Full Sail University - Portfolio 2 - Game Dev - Rod Moye - Enemy AI -

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;


    public int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int shootFOV;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] int numberOfPatrolPoints = 5; 
    [SerializeField] float patrolAreaSize = 20f; 
    private Vector3[] patrolPoints; 
    private int currentPatrolIndex = 0; 
    [SerializeField] float patrolSpeed = 2f; 
    [SerializeField] float detectionRange = 10f; 

    bool playerInRange;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;


    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;


    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1, 0);
        agent.speed = patrolSpeed; 
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

        
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

        
            if (!canSeePlayer())
            {
                checkRoam();
            }
        }
        else
        {
           
            Patrol();

            
            if (agent.remainingDistance < 0.01f)
            {
                roamTimer += Time.deltaTime;
                checkRoam();
            }
        }
    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        playerDir = (gamemanager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                shootTimer += Time.deltaTime;

                if (angleToPlayer <= shootFOV && shootTimer >= shootRate)
                {
                    shoot();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
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
            agent.stoppingDistance = 0;
        }
    }

    void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[numberOfPatrolPoints];

        for (int i = 0; i < numberOfPatrolPoints; i++)
        {
            
            float x = Random.Range(-patrolAreaSize / 2, patrolAreaSize / 2);
            float z = Random.Range(-patrolAreaSize / 2, patrolAreaSize / 2);
            patrolPoints[i] = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        }
    }
    void Patrol()
    {
    
        if (patrolPoints.Length == 0)
        {

            return; 
        }

        
        Vector3 targetPosition = patrolPoints[currentPatrolIndex];

       
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
           
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

       
        float distanceToPlayer = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);
        if (distanceToPlayer <= detectionRange)
        {
            playerInRange = true; 
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

        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}