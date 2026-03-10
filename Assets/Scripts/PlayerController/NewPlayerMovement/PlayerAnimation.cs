using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    private PlayerMovement pMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatePlayer();
    }

    private void AnimatePlayer()
    {
        switch (pMovement.state)
        {
            case PlayerMovement.MovementStates.Idle:
                break;
            case PlayerMovement.MovementStates.Walking:
                break;
            case PlayerMovement.MovementStates.Air:
                break;
            case PlayerMovement.MovementStates.Flying:
                break;
            case PlayerMovement.MovementStates.Sliding:
                break;
            case PlayerMovement.MovementStates.SlidingDown:
                break;
            default:
                break;
        }
    }
}
