using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    private PlayerMovement pMovement;
    private PlayerSlide pSlide;
    private PlayerShoot pShoot;
    private PlayerGetHit pGetHit;

    private void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pSlide = GetComponent<PlayerSlide>();
        pShoot = GetComponent<PlayerShoot>();
        pGetHit = GetComponent<PlayerGetHit>();
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            BlockPlayer();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            UnblockPlayer();
    }
    */

    public void BlockPlayer()
    {
        pShoot.enabled = false;
        pSlide.enabled = false;
        pMovement.enabled = false;
        pGetHit.enabled = false;
    }

    public void UnblockPlayer()
    {
        pShoot.enabled = true;
        pSlide.enabled = true;
        pMovement.enabled = true;
        pGetHit.enabled = true;
    }

}
