//Mark Bennett

using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] int sens;
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

        // Get joystick input for camera control
        float joystickX = Input.GetAxis("RightJoystickHorizontal") * sens * Time.deltaTime;
        float joystickY = Input.GetAxis("RightJoystickVertical") * sens * Time.deltaTime;


        // get input 
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;


        // Combine mouse and joystick input
        float combinedMouseX = mouseX + joystickX;
        float combinedMouseY = (invertY ? joystickY : -joystickY) + (invertY ? mouseY : -mouseY);

        // Handle vertical rotation
        rotX += combinedMouseY;
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the camera on the x-axis to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the camera on the y-axis to look left and right
        transform.parent.Rotate(Vector3.up * combinedMouseX);
    }
}