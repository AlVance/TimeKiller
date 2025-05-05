using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]private Transform[] waypoints;
    private int currentWaypointsIndex = 0;
    [SerializeField] private float moveSpeed;
    private int direction = 1;
    private void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.linearVelocity = (waypoints[currentWaypointsIndex].position - this.transform.position).normalized * moveSpeed;
    }

    private void FixedUpdate()
    {
        MoveToWaypoint();
    }
    private void MoveToWaypoint()
    {
        if(Vector3.Distance(this.transform.position, waypoints[currentWaypointsIndex].position) < 0.1f)
        {
            currentWaypointsIndex += direction;
            rb.linearVelocity = (waypoints[currentWaypointsIndex].position - this.transform.position).normalized * moveSpeed;
            if (currentWaypointsIndex >= waypoints.Length - 1 || currentWaypointsIndex <= 0) direction *= -1;
        }
    }
}
