using UnityEngine;

public class shopFunctions : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopKeeper.instance.ToggleShop(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopKeeper.instance.ToggleShop(false);
        }

    }

}
