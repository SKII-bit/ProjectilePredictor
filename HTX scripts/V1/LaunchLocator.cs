using UnityEngine;

public class LaunchLocator : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public float horizontalDisplacement;  // Input horizontal displacement (meters)
    public float velocity;  // Input velocity
    public float angleInDegrees;  // Angle in degrees input from the inspector

    private float predictedHeight;
    private float gravity = 9.81f;  // Earth's gravitational acceleration
    private float airDensity = 1.225f;  // kg/m^3
    private float crossSectionalArea = 0.002827f;  // m^2
    private float Cdrag = 1.0f;  // Drag coefficient

    // The Launcher going up or down
    public GameObject Launcher;

    private float mass = 0.3f;  // Mass of the glass bottle (kg)

    // Start is called before the first frame update
    void Start()
    {
        // Convert angle to radians for calculations
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculate horizontal and vertical velocity components
        float horizontalVelocity = velocity * Mathf.Sin(angleInRadians);
        float verticalVelocity = velocity * Mathf.Cos(angleInRadians);

        // Debug the initial values
        Debug.Log($"Initial Horizontal Velocity: {horizontalVelocity}, Initial Vertical Velocity: {verticalVelocity}");

        // Predict the height at the specified horizontal displacement
        predictedHeight = PredictHeightAtDisplacement(horizontalDisplacement, horizontalVelocity, verticalVelocity, velocity);

        Debug.Log("Predicted height: " + predictedHeight + " meters");

        // Check if predicted height is valid
        if (predictedHeight <= 0)
        {
            Debug.LogError("Predicted height is negative. Check your inputs!");
            return; // Exit if height is invalid
        }

        // Set the object's position based on the prediction
        if (Launcher != null)
        {
            RotateLauncher();
            MoveObjectToPosition(predictedHeight);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //reads for user input 
        {
            var _projectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation); //makes a copy of the bottle inside launcher
            _projectile.GetComponent<Rigidbody>().velocity = velocity * launchPoint.up; //launches projectile in direction of launcher
        }
    }

    // Predict the height at the specified horizontal displacement accounting for air drag
    float PredictHeightAtDisplacement(float targetDisplacement, float horizontalVelocity, float verticalVelocity, float velocity)
    {
        float timeStep = 0.01f;  // Time increment for simulation
        float currentHeight = 0f;
        float currentZ = 0f;

        // Simulate motion
        while (currentHeight >= 0 && currentZ < targetDisplacement)
        {
            // Calculate drag forces
            float dragForceX = 0.5f * Cdrag * airDensity * crossSectionalArea * Mathf.Pow(horizontalVelocity, 2);
            float dragForceY = 0.5f * Cdrag * airDensity * crossSectionalArea * Mathf.Pow(verticalVelocity, 2);

            // Calculate accelerations
            float dragAccelerationX = dragForceX / mass;
            float dragAccelerationY = (dragForceY / mass) + gravity;  // Include gravity for vertical motion

            // Update velocities
            horizontalVelocity -= dragAccelerationX * timeStep;  // Apply drag to horizontal velocity
            verticalVelocity -= dragAccelerationY * timeStep;  // Apply drag to vertical velocity

            // Update positions
            currentZ += horizontalVelocity * timeStep;
            currentHeight += verticalVelocity * timeStep;

            // Check for negative height condition
            if (currentHeight < 0)
                break;
        }

        // Always return the height, making sure it's not negative
        return Mathf.Max(currentHeight, 0);  // Return 0 if height is negative
    }

    // Move the object to the predicted position
    void MoveObjectToPosition(float height)
    {
        Vector3 currentPosition = Launcher.transform.position;
        Launcher.transform.position = new Vector3(currentPosition.x, height, currentPosition.z);
    }

    void RotateLauncher()
    {
        // Rotate the Launcher about the x-axis based on the angleInDegrees
        Launcher.transform.rotation = Quaternion.Euler(angleInDegrees, Launcher.transform.rotation.eulerAngles.y, Launcher.transform.rotation.eulerAngles.z);
    }

}
