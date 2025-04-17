using System.Data;
using UnityEngine;

public class activeTrap : MonoBehaviour
{
    enum trapType {Ram, Pitfall, Spike};
    [SerializeField] trapType type;
    [SerializeField] float rotationSpeed;
    [SerializeField] GameObject Spikes;

    private void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        activateTrap();
    }

    private void OnTriggerExit(Collider other)
    {
        deactivateTrap();
    }

    void activateTrap()
    {
        if (type == trapType.Ram)
        {
            transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime * 10);
        }
        else if(type == trapType.Pitfall)
        {
            Destroy(gameObject);
        }else if(type == trapType.Spike)
        {
            Spikes.SetActive(true);
        }
    }

    void deactivateTrap()
    {
        if (type == trapType.Spike)
        {
            Spikes.SetActive(false);
        }
    }
}
