using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;


public class PlayerShoot : MonoBehaviour
{
    private PlayerInputs pInputs;
    private PlayerMovement pMovement;

    private bool canAim = true;

    [SerializeField] private GameObject playerProjectileGO;
    [SerializeField] private Transform projectileSpawnPos;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject weaponModel;
    [SerializeField] private Transform backSocket;
    [SerializeField] private Transform handSocket;

    [Header("Shoot variables")]
    [SerializeField] private float shootCDTime;
    public bool shootOnCD = false;
    public bool didShoot = false;

    [SerializeField] private float projectileSpeed;
    private GameObject currentProjectileGO;

    [SerializeField] private float projectileRange;
    [SerializeField] private int projectileDamage;
    [SerializeField] private float moveDirShootInertia;

    [SerializeField] private GameObject aimGuideGO;

    [SerializeField] private MMF_Player enterAimFeedBack;
    [SerializeField] private MMF_Player endAimFeedBack;


    private void Start()
    {
        pInputs = GetComponent<PlayerInputs>();
        pMovement = GetComponent<PlayerMovement>();

        weaponModel.transform.parent = backSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);

        ProjectilePooling();

        aimGuideGO.SetActive(false);
    }

    private void Update()
    {
        canAim = !pMovement.isSliding;

        if (pInputs.shootPressed && !pMovement.isAiming && canAim) StartAim();
        if(pMovement.isAiming)Aim();
        if (pMovement.isAiming && (!pInputs.shootPressed || !canAim)) EndAim();
    }
    Vector3 aimDir;
    private void StartAim()
    {
        pMovement.isAiming = true;
        aimDir = this.transform.forward;

        weaponModel.transform.parent = handSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);

        aimGuideGO.SetActive(true);

        enterAimFeedBack.PlayFeedbacks();
    }

    private void Aim()
    {
        if(pInputs.aimDirRelativeToCam != Vector3.zero)
        {
            aimDir = pInputs.aimDirRelativeToCam;

            playerModel.transform.rotation = Quaternion.LookRotation(aimDir);
        }

    }

    private void EndAim()
    {
        Shoot();
        pMovement.isAiming = false;
        weaponModel.transform.parent = backSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);

        aimGuideGO.SetActive(false);

        endAimFeedBack.PlayFeedbacks();
    }

    private void Shoot()
    {
        if (!shootOnCD)
        {
            currentProjectileGO = projectilePool[currentProjectilePooled];
            if (currentProjectilePooled < projectilePool.Count - 1) ++currentProjectilePooled;
            else currentProjectilePooled = 0;

            PlayerProjectile proj = currentProjectileGO.GetComponent<PlayerProjectile>();
            proj.ProjectileSetUp(projectileDamage, projectileRange, projectileSpawnPos);
            proj.SetCharged();

            currentProjectileGO.transform.parent = null;
            Vector3 shootDir;
            shootDir = aimDir;
            //if (pInputs.lastAimDirRelativeToCam == Vector3.zero) shootDir = this.transform.forward;
            //else shootDir = pInputs.lastAimDirRelativeToCam;
            proj.LaunchProjectile(shootDir + pInputs.moveDirRelativeToCam * moveDirShootInertia, projectileSpeed);
            currentProjectileGO = null;

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
        aimGuideGO.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(shootCDTime);
        weaponModel.SetActive(true);
        aimGuideGO.transform.parent.gameObject.SetActive(true);
        shootOnCD = false;
    }

    private List<GameObject> projectilePool = new List<GameObject>();
    private int currentProjectilePooled = 0;
    private void ProjectilePooling()
    {
        for (int i = 0; i < (projectileRange / shootCDTime) + 1; i++)
        {
            GameObject newProj = Instantiate(playerProjectileGO, projectileSpawnPos.position, Quaternion.identity, projectileSpawnPos);
            projectilePool.Add(newProj);
            newProj.GetComponent<Projectile>().spawnPos = projectileSpawnPos;
            newProj.GetComponent<Projectile>().SetProjectileInactive();
        }
    }

    private void OnDisable()
    {
        pMovement.isAiming = false;
        weaponModel.transform.parent = backSocket;
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
        endAimFeedBack.PlayFeedbacks();
        aimGuideGO.SetActive(false);

        //EndAim();
    }
}
