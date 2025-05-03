using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Xml.XPath;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;

    [Header("---Stats---")]
    [SerializeField] int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTranSpeed;

    [Header("---Weapons---")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootFOV;
    [SerializeField] float shootRate;

    [Header("---Audio---")]
    [SerializeField] AudioClip[] audMove;
    [Range(0, 1)][SerializeField] float audMoveVol;
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 1)][SerializeField] float audShootVol;

    bool playerInRange;
    bool isPlayingStep;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDisOrig;

    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        //gamemanager.instance.updateGameGoal(1, 0);
        startingPos = transform.position;
        stoppingDisOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();

        //anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        if (!isPlayingStep && agent.velocity.magnitude > 0.2f)
        {
            StartCoroutine(playMoveSound());
        }

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

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    IEnumerator playMoveSound()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audMove[Random.Range(0, audMove.Length)], audMoveVol);
        yield return new WaitForSeconds(0.5f);
        isPlayingStep = false;
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
                agent.stoppingDistance = stoppingDisOrig;
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
            agent.stoppingDistance = 0;
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
        anim.SetTrigger("Shoot");
        aud.PlayOneShot(audShoot[Random.Range(0, audShoot.Length)], audShootVol);
    }
    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
