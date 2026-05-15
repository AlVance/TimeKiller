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

    private Vector3 lastPosition;
    private GameObject playerOnPlatform;
    [SerializeField] private BoxCollider triggerCol;
    private float triggerColDistance;

    private void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        lastPosition = transform.position;

    
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
       if(hasStarted)
        {
            Vector3 delta = transform.position - lastPosition;

            if (playerOnPlatform != null && delta != Vector3.zero)
            {
                Rigidbody playerRb = playerOnPlatform.GetComponent<Rigidbody>();
                playerRb.MovePosition(playerRb.position + delta);

                if(Vector3.Distance(this.transform.position, playerOnPlatform.transform.position) > triggerCol.bounds.size.x)
                {
                    Debug.Log("out");
                    playerOnPlatform = null;
                }
            }

            lastPosition = transform.position;
            MoveToWaypoint();
        }
    }
    private void MoveToWaypoint()
    {
        if(Vector3.Distance(this.transform.position, waypointsList[currentWaypointsIndex].position) < 0.1f)
        {
            currentWaypointsIndex += direction;
            if (currentWaypointsIndex >= waypointsList.Count - 1 || currentWaypointsIndex <= 0) direction *= -1;
        }
        rb.linearVelocity = (waypointsList[currentWaypointsIndex].position - this.transform.position).normalized * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("OnPlatform");
            playerOnPlatform = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerOnPlatform = null;
        }
    }
}
