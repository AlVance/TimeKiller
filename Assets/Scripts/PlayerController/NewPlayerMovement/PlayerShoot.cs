using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShoot : MonoBehaviour
{
    private PlayerInputs pInputs;
    private PlayerMovement pMovement;

    [SerializeField] private GameObject playerModel;

    private void Start()
    {
        pInputs = GetComponent<PlayerInputs>();
        pMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (pInputs.shootPressed && !pMovement.isAiming) StartAim();
        if(pMovement.isAiming)Aim();
        if (!pInputs.shootPressed && pMovement.isAiming) EndAim();
    }

    private void StartAim()
    {
        pMovement.isAiming = true;
    }

    private void Aim()
    {
        Vector3 aimDir = pInputs.aimDirRelativeToCam;
        if(aimDir != Vector3.zero) playerModel.transform.rotation = Quaternion.LookRotation(aimDir);
    }

    private void EndAim()
    {
        pMovement.isAiming = false;
    }
}
