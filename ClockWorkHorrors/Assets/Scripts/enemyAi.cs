using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;


    [SerializeField] GameObject meleeHitbox;
    [SerializeField] float meleeCooldown;
    [SerializeField] float meleeRange;

    bool playerInRange;


    bool playerInRange;

    float shootTimer;
    float meleeTimer;

    Color colorOrig;
    Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1, 0); // track enemy count, no XP gain at spawn
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
            meleeTimer += Time.deltaTime;

            float distToPlayer = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

            if (distToPlayer <= meleeRange && meleeTimer >= meleeCooldown)
            {
                meleeAttack();
            }
            else if (shootTimer >= shootRate)
            {
                shoot();
            }
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

        StartCoroutine(flashred());

        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1, XP); // remove from goal and give XP
            Destroy(gameObject);
        }
    }

    IEnumerator flashred()
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

    void meleeAttack()
    {
        meleeTimer = 0;
        meleeHitbox.SetActive(true); // assume it deactivates itself via animation or script
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0f, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}

