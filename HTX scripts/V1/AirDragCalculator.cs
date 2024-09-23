using UnityEngine;

public class AirDragCalculator : MonoBehaviour
{
    public float velocity = 10f;           // Initial velocity
    public float angleInDegrees = 45f;     // Launch angle

    private float mass = 0.3f;              // Mass of the object
    private float Cdrag = 1f;               // Drag coefficient
    private float airDensity = 1.225f;      // Air density (kg/m^3 at sea level)
    private float crossSectionalArea = 0.002827f; // Cross-sectional area (m^2)
    private float gravity = 9.81f;          // Gravity constant
    private float horizontalVelocity;      // Velocity along the z-axis
    private float verticalVelocity;        // Velocity along the y-axis
    private Rigidbody rb;                  // Rigidbody of the GameObject
    private float timeStep = 0.01f;        // Simulation time step
    private float currentHeight;           // Current height (y-axis)
    private float currentZ;                // Current horizontal displacement (z-axis)
    private float accumulatedTime = 0f;    // Time accumulator for time stepping

    void Start()
    {
        // Get the Rigidbody component attached to the GameObject
        rb = GetComponent<Rigidbody>();

        // Initialize positions from the object's current position
        currentHeight = transform.position.y;
        currentZ = transform.position.z;

        // Convert angle to radians for calculations
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculate horizontal and vertical velocity components
        horizontalVelocity = velocity * Mathf.Sin(angleInRadians); // Along the z-axis
        verticalVelocity = velocity * Mathf.Cos(angleInRadians);   // Along the y-axis
    }

    void Update()
    {
        // Accumulate time
        accumulatedTime += Time.deltaTime;

        // Simulate motion every timeStep seconds
        if (accumulatedTime >= timeStep && currentHeight >= 0)
        {
            SimulateMotion();
            accumulatedTime = 0f; // Reset the accumulator after each step
        }
    }

    void SimulateMotion()
    {
        // Calculate the velocity magnitude (total velocity)
        float totalVelocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);

        // Calculate drag forces
        float dragForceZ = 0.5f * Cdrag * airDensity * crossSectionalArea * horizontalVelocity * totalVelocity;
        float dragForceY = 0.5f * Cdrag * airDensity * crossSectionalArea * verticalVelocity * totalVelocity;

        // Calculate accelerations
        float dragAccelerationZ = dragForceZ / mass;
        float dragAccelerationY = (dragForceY / mass) + gravity;  // Include gravity for vertical motion

        // Update velocities
        horizontalVelocity -= dragAccelerationZ * timeStep;  // Apply drag to horizontal velocity (z-axis)
        verticalVelocity -= dragAccelerationY * timeStep;    // Apply drag to vertical velocity (y-axis)

        // Update positions
        currentZ += horizontalVelocity * timeStep;
        currentHeight += verticalVelocity * timeStep;

        // Update the Rigidbody's velocity based on the calculated values
        Vector3 newVelocity = new Vector3(0, verticalVelocity, horizontalVelocity); // z is for horizontal movement
        rb.velocity = newVelocity;

        // Check for negative height condition
        if (currentHeight <= 0)
        {
            currentHeight = 0; // Stop simulation if object hits the ground
            rb.velocity = Vector3.zero; // Stop movement
        }
    }
}
