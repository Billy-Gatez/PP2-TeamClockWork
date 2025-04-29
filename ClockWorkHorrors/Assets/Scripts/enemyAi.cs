using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [Header("----- Stats -----")]
    [SerializeField] int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTranSpeed;

    [Header("----- Weapons -----")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootFOV;
    [SerializeField] float shootRate;
    [SerializeField] Collider swordCol;
    [SerializeField] GameObject meleeHitbox;
    [SerializeField] float meleeCooldown;
    [SerializeField] float meleeRange;

    bool playerInRange;
    float shootTimer;
    float meleeTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    Color colorOrig;
    Vector3 playerDir;
    Vector3 startingPos;

    void Start()
    {
        colorOrig = model.material.color;
        // gamemanager.instance.updateGameGoal(1, 0); // Left just in case
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    void Update()
    {
        setAnimLocomotion();

        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
    }

    void setAnimLocomotion()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTranSpeed));
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

        Vector3 ranPos = Random.insideUnitSphere * roamDist + startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                    faceTarget();

                shootTimer += Time.deltaTime;
                meleeTimer += Time.deltaTime;

                float distToPlayer = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

                if (distToPlayer <= meleeRange && meleeTimer >= meleeCooldown)
                {
                    meleeAttack();
                }
                else if (angleToPlayer <= shootFOV && shootTimer >= shootRate)
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

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0f, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void shoot()
    {
        shootTimer = 0;
        anim.SetTrigger("Shoot");
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void meleeAttack()
    {
        meleeTimer = 0;
        meleeHitbox.SetActive(true);
        // Trigger a melee animation here
    }

    public void swordColOn()
    {
        swordCol.enabled = true;
    }

    public void swordColOff()
    {
        swordCol.enabled = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashred());

        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1, XP);
            Destroy(gameObject);
        }
    }

    IEnumerator flashred()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
}


