using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    private PlayerMovement pMovement;
    private PlayerMovement.MovementStates lastState;

    private PlayerShoot pShoot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pShoot = GetComponent<PlayerShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatePlayer();
    }

    private void AnimatePlayer()
    {
        //if (lastState == null || pMovement.state != lastState)
        //{
        //    lastState = pMovement.state;
        //    switch (pMovement.state)
        //    {

        //        case PlayerMovement.MovementStates.Idle:
        //            playerAnim.SetTrigger("GoIdle");
        //            break;
        //        case PlayerMovement.MovementStates.Walking:
        //            playerAnim.SetTrigger("GoWalk");
        //            break;
        //        case PlayerMovement.MovementStates.Air:
        //            playerAnim.SetTrigger("GoAirIdle");
        //            break;
        //        case PlayerMovement.MovementStates.Flying:
        //            playerAnim.SetTrigger("GoFly");
        //            break;
        //        case PlayerMovement.MovementStates.Sliding:
        //            playerAnim.SetTrigger("GoSlide");
        //            break;
        //        case PlayerMovement.MovementStates.SlidingDown:
        //            playerAnim.SetTrigger("GoSlide");
        //            break;
        //        default:
        //            playerAnim.SetTrigger("GoIdle");
        //            break;
        //    }
        //}

        switch (pMovement.state)
        {
            case PlayerMovement.MovementStates.Idle:
                if(!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) playerAnim.SetTrigger("GoIdle");
                break;
            case PlayerMovement.MovementStates.Walking:
                if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Move")) playerAnim.SetTrigger("GoWalk");
                break;
            case PlayerMovement.MovementStates.Air:
                if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AirIdle")) playerAnim.SetTrigger("GoAirIdle");
                break;
            case PlayerMovement.MovementStates.Flying:
                if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Fly")) playerAnim.SetTrigger("GoFly");
                break;
            case PlayerMovement.MovementStates.Sliding:
                if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Drift")) playerAnim.SetTrigger("GoSlide");
                break;
            case PlayerMovement.MovementStates.SlidingDown:
                if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Drift")) playerAnim.SetTrigger("GoSlide");
                break;
        }

        playerAnim.SetBool("IsAiming", pMovement.isAiming);
        if(pShoot.didShoot)
        {
            playerAnim.SetTrigger("GoShoot");
        }
    }
}
