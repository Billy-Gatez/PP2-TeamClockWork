using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing, grenade }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [SerializeField] GameObject explosionEffect;
    [SerializeField] float explosionRadius;
    [SerializeField] float grenadeFuseTime;


    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == damageType.moving)
            {
                rb.linearVelocity = (gamemanager.instance.player.transform.position - transform.position).normalized * speed;
            }
        }

        if (type == damageType.grenade)
        {
            Destroy(gameObject, destroyTime);

            rb.useGravity = true;

            // Direction from grenade to player
            Vector3 dir = (gamemanager.instance.player.transform.position - transform.position).normalized;

            // Add some upward force for the arc
            Vector3 throwForce = dir * speed + Vector3.up;

            rb.AddForce(throwForce, ForceMode.VelocityChange);

            StartCoroutine(ExplodeAfterDelay());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == damageType.homing)
        {
            rb.linearVelocity = (gamemanager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && (type == damageType.stationary || type == damageType.moving || type == damageType.homing))
        {
            dmg.takeDamage(damageAmount);
        }

        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(grenadeFuseTime);

        // Show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Damage all nearby IDamage objects
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            IDamage dmg = hit.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damageAmount);
            }
        }

        Destroy(gameObject);
    }
}
