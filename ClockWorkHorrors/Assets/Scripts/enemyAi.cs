using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] GameObject meleeHitbox;
    [SerializeField] float meleeCooldown;
    [SerializeField] float meleeRange;

    float meleeTimer;

    float shootTimer;

    Color colorOrig;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
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

        // Prefer melee if close enough
        if (distToPlayer <= meleeRange && meleeTimer >= meleeCooldown)
        {
            meleeAttack();
        }
        else if (shootTimer >= shootRate)
        {
            shoot();
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashred());

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
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

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void meleeAttack()
    {
        meleeTimer = 0;
        meleeHitbox.SetActive(true);
    }
}

//TEST
}
