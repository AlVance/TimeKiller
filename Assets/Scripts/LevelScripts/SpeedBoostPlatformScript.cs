using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class SpeedBoostPlatformScript : MonoBehaviour
{
    [SerializeField] private float speedBoostForce;

    private Rigidbody playerRb;
    private PlayerMovement pMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRb = other.GetComponent<Rigidbody>();
            pMovement = other.GetComponent<PlayerMovement>();

            Vector3 boostDir = pMovement.moveDirection == Vector3.zero ? transform.forward : pMovement.moveDirection;
            playerRb.AddForce(boostDir * speedBoostForce, ForceMode.Impulse);

            float newVel = playerRb.linearVelocity.magnitude + speedBoostForce;
            pMovement.ApplyBoost(newVel, pMovement.desiredMoveSpeed);
        }
    }
}


