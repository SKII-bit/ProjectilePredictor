using UnityEngine;

public class BottleMotionV2 : MonoBehaviour
{
    //User inputs
    public float initialVelocity = 20f; // Initial velocity in m/s
    public float launchAngle = 45f; // Angle in degrees, from the vertical

    // Variables for position and velocity
    private Vector3 velocity; // 3D velocity vector
    private Vector3 position; // 3D position vector
    private Vector3 acceleration; // 3D acceleration vector

    private float mass = 0.3f; // Mass of the bottle in kg
    private float dragCoefficient = 1f; // Drag coefficient (for a bottle)
    private float crossSectionalArea = 0.002827f; // Cross-sectional area in m^2
    private float airDensity = 1.225f; // Air density in kg/m^3 at sea level
    private const float g = 9.81f; // Acceleration due to gravity (m/s^2)

    // Time step for numerical integration
    private float timeStep = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        // Convert launch angle from degrees to radians
        float angleRadians = launchAngle * Mathf.Deg2Rad;

        // Set initial velocity components (adjusted for angle from vertical)
        velocity.x = initialVelocity * Mathf.Sin(angleRadians); // Horizontal velocity
        velocity.y = initialVelocity * Mathf.Cos(angleRadians); // Vertical velocity

        // Initialize position (assuming starting from origin at height h)
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Call the Euler integration method to update the bottle's position
        EulerIntegration();

        // Update the GameObject position in Unity
        transform.position = position;

        // Stop when the object hits the ground
        if (position.y <= 0)
        {
            enabled = false; // Stop the simulation
        }
    }

    void EulerIntegration()
    {
        // Calculate the speed (magnitude of velocity)
        float speed = velocity.magnitude;

        // Calculate the drag force
        Vector3 dragForce = -0.5f * dragCoefficient * airDensity * crossSectionalArea * speed * speed * velocity.normalized;

        // Calculate net forces (gravity + drag)
        Vector3 gravityForce = new Vector3(0, -mass * g, 0); // Gravity only acts vertically
        Vector3 netForce = dragForce + gravityForce;

        // Newton's Second Law: F = m * a -> a = F / m
        acceleration = netForce / mass;

        // Update velocity using Euler's method: v(t + dt) = v(t) + a(t) * dt
        velocity += acceleration * timeStep;

        // Update position using Euler's method: p(t + dt) = p(t) + v(t) * dt
        position += velocity * timeStep;
    }
}
