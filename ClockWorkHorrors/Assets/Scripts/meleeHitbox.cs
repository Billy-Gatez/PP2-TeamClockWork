using UnityEngine;

public class meleeHitbox : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float activeTime = 0.3f;

    bool alreadyHit;

    void OnEnable()
    {
        alreadyHit = false;
        Invoke(nameof(DisableHitbox), activeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHit || other.isTrigger)
            return;

        meDamage dmg = other.GetComponent<meDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
            alreadyHit = true;
        }
    }

    void DisableHitbox()
    {
        gameObject.SetActive(false);
    }
}