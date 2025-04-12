using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sens = 100f; // Sensitivity
    [SerializeField] private float lockVertMin = -60f, lockVertMax = 60f; // Vertical lock limits
    [SerializeField] private bool invertY = false; // Invert Y-axis

    private float rotX;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Get joystick input for camera control
        float joystickX = Input.GetAxis("RightJoystickHorizontal") * sens * Time.deltaTime;
        float joystickY = Input.GetAxis("RightJoystickVertical") * sens * Time.deltaTime;

        // Get mouse input
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