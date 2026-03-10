using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    private PlayerMovement pMovement;
    private PlayerMovement.MovementStates lastState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lastState == null || pMovement.state != lastState)AnimatePlayer();
    }

    private void AnimatePlayer()
    {
        lastState = pMovement.state;
        switch (pMovement.state)
        {
            
            case PlayerMovement.MovementStates.Idle:
                playerAnim.SetTrigger("GoIdle");
                break;
            case PlayerMovement.MovementStates.Walking:
                playerAnim.SetTrigger("GoWalk");
                break;
            case PlayerMovement.MovementStates.Air:
                playerAnim.SetTrigger("GoAirIdle");
                break;
            case PlayerMovement.MovementStates.Flying:
                playerAnim.SetTrigger("GoFly");
                break;
            case PlayerMovement.MovementStates.Sliding:
                playerAnim.SetTrigger("GoSlide");
                break;
            case PlayerMovement.MovementStates.SlidingDown:
                playerAnim.SetTrigger("GoSlide");
                break;
            default:
                playerAnim.SetTrigger("GoIdle");
                break;
        }
    }
}
