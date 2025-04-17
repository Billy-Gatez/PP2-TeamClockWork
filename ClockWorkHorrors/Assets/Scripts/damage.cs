using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, overtime, trap}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [Range(1,10)] [SerializeField] int damageAmount;
    [Range(.25f, 1f)][SerializeField] float damageTime;
    [Range(10,45)][SerializeField] int speed;
    [Range(1,4)] [SerializeField] int destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == damageType.moving)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if(dmg != null && (type==damageType.stationary || type==damageType.moving || type==damageType.trap))
        {
            dmg.takeDamage(damageAmount);
        }

        if(type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.overtime)
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
        yield return new WaitForSeconds(damageTime);

        isDamaging = false;
    }

}
