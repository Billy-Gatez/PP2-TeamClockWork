using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get input

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (invertY)
        { rotX += mouseY; }
        else
        { rotX -= mouseY; }

        //clamp the camera on the x axis

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate camera on x axis for up/down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate player on y axis for left/right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
