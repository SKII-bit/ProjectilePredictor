using UnityEngine;

public class ProjectileHeightCalculator : MonoBehaviour
{
    public GameObject bottle; // Assign the GameObject (bottle) in the Inspector
    public float horizontalDisplacement;  // Known horizontal distance along the z-axis (set in Inspector)
    public float initialVelocity;   //Initial velocity set by user
    public float angleOfElevation = 10f;  //Angle of elevation from vertical set by user
    public float dragCoefficient;          // Drag coefficient (set in Inspector)
    public float airDensity;               // Air density (set in Inspector)
    public float crossSectionalArea;      // Cross-sectional area (set in Inspector)

    private float initialHeightGuess;     // Variable to find the initial height
    private float initialVelocityX;        // Initial horizontal velocity 
    private float initialVelocityY;        // Initial vertical velocity  
    private float bottleMass = 0.3f;               // Mass of the bottle (set in Inspector)
    private float gravity = 9.81f;         // Acceleration due to gravity
    void Start()
    {
        // Start with an initial guess for the height
        initialHeightGuess = 10f; // Start guessing from a height of 10 meters
        float angleInRadians = angleOfElevation * Mathf.Deg2Rad; // Convert angle of elevation from degrees to radians

        initialVelocityX = initialVelocity * Mathf.Sin(angleInRadians); // Horizontal component
        initialVelocityY = initialVelocity * Mathf.Cos(angleInRadians); // Vertical component

        // Tolerance for the time of flight comparison
        float tolerance = 0.01f;

        // Find the initial height
        FindInitialHeight(tolerance);
    }

    void FindInitialHeight(float tolerance)
    {
        int maxIterations = 100; // Limit iterations to avoid infinite loop
        int iterations = 0;

        while (iterations < maxIterations)
        {
            float timeOfFlight = Simulate(initialHeightGuess);
            float expectedTime = horizontalDisplacement / initialVelocityX;

            // Check if the calculated time matches the expected time within the tolerance
            if (Mathf.Abs(timeOfFlight - expectedTime) < tolerance)
            {
                Debug.Log($"The initial height is approximately: {initialHeightGuess} meters");

                // Move the bottle GameObject to the calculated height
                bottle.transform.position = new Vector3(bottle.transform.position.x, initialHeightGuess, bottle.transform.position.z);

                // Rotate the bottle about the x-axis for the angle of elevation
                bottle.transform.rotation = Quaternion.Euler(angleOfElevation, 0, 0);
                break;
            }

            // Adjust the guess for height (increase or decrease)
            initialHeightGuess += 0.1f; // Increment height guess
            iterations++; // Increment iteration count
        }

        if (iterations == maxIterations)
        {
            Debug.LogWarning("Max iterations reached without finding a valid height.");
        }
    }


    float Simulate(float initialHeight)
    {
        float y = initialHeight;
        float vX = initialVelocityX;
        float vY = initialVelocityY;
        float dt = 0.01f; // Time step
        float time = 0f;

        while (y > 0)
        {
            float v = Mathf.Sqrt(vX * vX + vY * vY); // Calculate total velocity
            float dragForce = 0.5f * dragCoefficient * airDensity * crossSectionalArea * v * v;

            // Update vertical velocity with air drag
            float dvY = -gravity - (dragForce / bottleMass) * (vY / v);
            vY += dvY * dt; // Update vertical velocity
            y += vY * dt; // Update height
            time += dt; // Increment time
        }

        return time; // Return time of flight
    }
}
