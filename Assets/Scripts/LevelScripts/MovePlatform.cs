using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] private float delayMoveTime = 0;
    private Rigidbody rb;
    //[SerializeField]private Transform[] waypoints;
    private List<Transform> waypointsList = new List<Transform>();
    [SerializeField] private GameObject waypointsParent;
    private int currentWaypointsIndex = 0;
    [SerializeField] private float moveSpeed;
    private int direction = -1;
    private bool hasStarted = false;

    private void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delayMoveTime);
        foreach (Transform item in waypointsParent.transform)
        {
            waypointsList.Add(item);
        }
        this.transform.position = waypointsList[currentWaypointsIndex].position;
        ++currentWaypointsIndex;
        rb.linearVelocity = (waypointsList[currentWaypointsIndex].position - this.transform.position).normalized * moveSpeed;

        hasStarted = true;
    }

    private void FixedUpdate()
    {
       if(hasStarted) MoveToWaypoint();
    }
    private void MoveToWaypoint()
    {
        if(Vector3.Distance(this.transform.position, waypointsList[currentWaypointsIndex].position) < 0.1f)
        {
            currentWaypointsIndex += direction;
            rb.linearVelocity = (waypointsList[currentWaypointsIndex].position - this.transform.position).normalized * moveSpeed;
            if (currentWaypointsIndex >= waypointsList.Count - 1 || currentWaypointsIndex <= 0) direction *= -1;
        }
    }
}
