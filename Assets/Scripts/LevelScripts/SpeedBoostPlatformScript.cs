using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class SpeedBoostPlatformScript : MonoBehaviour
{
    [SerializeField] private float speedBoostForce;
    [SerializeField] private float speedBoostTime;
    private float currentTime = 0;

    private Rigidbody playerRb;
    private PlayerMovement pMovement;

    private void FixedUpdate()
    {
        if(currentTime > 0)
        {
            playerRb.AddForce(pMovement.moveDirection * speedBoostForce, ForceMode.Force);
            if(pMovement.currentMoveSpeed < (pMovement.moveDirection * speedBoostForce).magnitude) pMovement.currentMoveSpeed = (pMovement.moveDirection * speedBoostForce).magnitude;
            currentTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRb = other.GetComponent<Rigidbody>();
            pMovement = other.GetComponent<PlayerMovement>();

            Vector3 boostDir = pMovement.moveDirection == Vector3.zero ? transform.forward : pMovement.moveDirection;
            pMovement.ApplyExternalImpulse(boostDir, speedBoostForce / 10);

            currentTime = speedBoostTime;

        }    
    }
}


