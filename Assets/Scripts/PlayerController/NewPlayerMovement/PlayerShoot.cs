using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    private PlayerInputs pInputs;
    private PlayerMovement pMovement;

    private bool canAim = true;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject weaponModel;
    [SerializeField] private Transform backSocket;
    [SerializeField] private Transform handSocket;

    [Header("Shoot variables")]
    [SerializeField] private float shootCDTime;
    public bool shootOnCD = false;
    public bool didShoot = false;

    private void Start()
    {
        pInputs = GetComponent<PlayerInputs>();
        pMovement = GetComponent<PlayerMovement>();

        weaponModel.transform.parent = backSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void Update()
    {
        canAim = !pMovement.isSliding;

        if (pInputs.shootPressed && !pMovement.isAiming && canAim) StartAim();
        if(pMovement.isAiming)Aim();
        if (pMovement.isAiming && (!pInputs.shootPressed || !canAim)) EndAim();
    }

    private void StartAim()
    {
        pMovement.isAiming = true;
        weaponModel.transform.parent = handSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void Aim()
    {
        Vector3 aimDir = pInputs.aimDirRelativeToCam;
        if(aimDir != Vector3.zero) playerModel.transform.rotation = Quaternion.LookRotation(aimDir);
    }

    private void EndAim()
    {
        Shoot();
        pMovement.isAiming = false;
        weaponModel.transform.parent = backSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void Shoot()
    {
        if (!shootOnCD)
        {
            Debug.Log("SHOOT!");
            StartCoroutine(_ShootCD());
        }
    }

    private IEnumerator _ShootCD()
    {
        didShoot = true;
        yield return null;
        didShoot = false;
        shootOnCD = true;
        weaponModel.SetActive(false);
        yield return new WaitForSeconds(shootCDTime);
        weaponModel.SetActive(true);
        shootOnCD = false;
    }
}
