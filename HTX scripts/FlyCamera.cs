using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float speed = 10.0f; // Movement speed
    public float mouseSensitivity = 100.0f; // Mouse sensitivity
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Capture Mouse Movement for rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust yaw and pitch based on mouse input
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Prevent flipping upside down

        // Rotate camera
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Calculate movement direction based on user input
        Vector3 moveDirection = Vector3.zero;

        // Horizontal movement (W/S keys control forward/backward, A/D control left/right)
        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward; // Move forward
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward; // Move backward
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;   // Move left
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;   // Move right

        // Vertical movement (E to move up, Q to move down)
        if (Input.GetKey(KeyCode.E))
            moveDirection += transform.up;      // Move up
        if (Input.GetKey(KeyCode.Q))
            moveDirection -= transform.up;      // Move down

        // Normalize direction to prevent faster diagonal movement
        moveDirection.Normalize();

        // Apply movement speed and update camera position
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Ensure the cursor remains locked when the game window is focused
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
