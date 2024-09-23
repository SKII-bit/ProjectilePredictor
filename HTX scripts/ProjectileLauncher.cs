using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public float Velocity = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var _projectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);
            _projectile.GetComponent<Rigidbody>().velocity = Velocity * launchPoint.up;
        }
    }
}