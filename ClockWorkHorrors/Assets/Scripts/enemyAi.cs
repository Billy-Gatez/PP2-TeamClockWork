// Jeremy Cahill

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, meDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float shootRate;

    float shootTimer;

    Color colorOrig;
    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color; // Store the original color
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

        if (shootTimer >= shootRate)
        {
            shoot();
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
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
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}