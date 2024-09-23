using UnityEngine;

public class LauncherPrediction : MonoBehaviour
{
    // Constants
    public float initialVelocity = 20f; // Initial velocity in m/s
    public float launchAngle = 45f; // Angle in degrees (measured from vertical)
    public float horizontalDisplacement = 30f; // Horizontal displacement (distance traveled)
    public Transform launchPoint; // Launching point
    public GameObject projectile; // Projectile prefab

    // Launcher reference
    public Transform launcher; // Drag the launcher GameObject into this field in the Inspector

    // Internal variables
    private Vector3 velocity; // Velocity vector
    private float predictedHeight; // Predicted launch height

    // Numerical integration settings
    private float timeStep = 0.01f; // Time step for the simulation

    private float mass = 0.3f; // Mass of the bottle in kg
    private float dragCoefficient = 1f; // Drag coefficient
    private float crossSectionalArea = 0.002827f; // Cross-sectional area in m^2
    private float airDensity = 1.225f; // Air density in kg/m^3 at sea level
    private float zRotation = 0f; // Additional rotation around the z-axis in degrees
    private const float g = 9.81f; // Acceleration due to gravity (m/s^2)

    void Start()
    {
        // Calculate the initial velocity components (based on launch angle from vertical)
        float angleRadians = launchAngle * Mathf.Deg2Rad;
        velocity.x = initialVelocity * Mathf.Sin(angleRadians); // Horizontal velocity
        velocity.y = initialVelocity * Mathf.Cos(angleRadians); // Vertical velocity

        // Predict the height from which the bottle was launched
        PredictLaunchHeight();

        // Move the launcher to the predicted height and rotate it
        MoveLauncher();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        // Instantiate the projectile at the launch point's position and rotation
        GameObject _projectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);

        // Calculate the initial velocity based on the launch angle and direction
        float angleRadians = launchAngle * Mathf.Deg2Rad;
        Vector3 launchDirection = new Vector3(
            initialVelocity * Mathf.Sin(angleRadians), // x-component
            initialVelocity * Mathf.Cos(angleRadians), // y-component
            0
        );

        // Set the projectile's velocity
        _projectile.GetComponent<Rigidbody>().velocity = launchDirection;
    }

    void PredictLaunchHeight()
    {
        Vector3 position = Vector3.zero; // Starting position
        float totalTime = 0f; // Track total time elapsed

        while (position.x < horizontalDisplacement)
        {
            float speed = velocity.magnitude;
            Vector3 dragForce = -0.5f * dragCoefficient * airDensity * crossSectionalArea * speed * speed * velocity.normalized;
            Vector3 gravityForce = new Vector3(0, -mass * g, 0);
            Vector3 netForce = dragForce + gravityForce;
            Vector3 acceleration = netForce / mass;

            velocity += acceleration * timeStep;
            position += velocity * timeStep;
            totalTime += timeStep;

            if (velocity.y <= 0 && position.y <= 0)
                break;
        }

        predictedHeight = position.y;

        if (predictedHeight < 0)
        {
            Debug.LogWarning("Predicted height is negative: " + predictedHeight);
        }
    }

    void MoveLauncher()
    {
        launcher.position = new Vector3(launcher.position.x, predictedHeight, launcher.position.z);
        launcher.rotation = Quaternion.Euler(launchAngle, 0, zRotation);
    }
}
