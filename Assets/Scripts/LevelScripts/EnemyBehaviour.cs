using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using MyBox;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject enemyStuff;
    private enum enemyMovementTypes { Static, Movable };
    [SerializeField] private enemyMovementTypes enemyMovementType = enemyMovementTypes.Static;

    private enum enemyOnHitTypes { Standard, TpOnKill};
    [SerializeField] private enemyOnHitTypes enemyBehaviourType = enemyOnHitTypes.Standard;

    [Header("Enemy abilities")]
    [SerializeField] private bool canShoot = false;
    [SerializeField] private bool canRespawn = false;
    [SerializeField] private bool isInvulnerable = false;


    [SerializeField] private int enemyHealth = 1;
    private int currentEnemyHealth;

    [Header("Movement variables")]
    [SerializeField, ConditionalField(nameof(enemyMovementType), false, enemyMovementTypes.Movable)] 
    private float moveSpeed = 1;
    [SerializeField]
    private SplineContainer movementSpline;
    private float distancePercentageSpline;
    private float movementSplineLength;
    public SplineContainer MovementSpline { get => movementSpline; set => movementSpline = value; }

    [Header("Shoot variables")]
    [SerializeField, ConditionalField(nameof(canShoot), false)] private float shootCD;
    [SerializeField, ConditionalField(nameof(canShoot), false)] private float projectileRange;
    [SerializeField, ConditionalField(nameof(canShoot), false)] private float projectileSize;
    [SerializeField, ConditionalField(nameof(canShoot), false)] private float projectileSpeed;
    [SerializeField, ConditionalField(nameof(canShoot), false)] private float projectileHitForce;
    private float currentShootCD;
    [SerializeField]private EnemyGunCollider gunCol;
    [SerializeField] private GameObject enemyProjectileGO;
    [SerializeField] private Transform shootOriginTr;
    [SerializeField] private LineRenderer lR;

    [Header("RespawnVariables")]
    [SerializeField, ConditionalField(nameof(canRespawn), false)] private float respawnTime = 5f;
    private bool isAlive = true;

    [Header("Visual variables")]
    [SerializeField] private GameObject enemyStandardModelGO;
    [SerializeField] private GameObject enemyTpOnKillModelGO;
    [SerializeField] private GameObject enemyGunGO;
    [SerializeField] private GameObject shieldGO;
    private GameObject enemyModelGO;

    private Vector3 gunTr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnemyHealth = enemyHealth;
        if (enemyMovementType == enemyMovementTypes.Movable)
        {
            movementSplineLength = MovementSpline.CalculateLength();
            MovementSpline.gameObject.transform.parent = this.transform.parent;
            this.transform.position = MovementSpline.EvaluatePosition(0);
        }
        else
        {
            MovementSpline.gameObject.SetActive(false);
        }

        EnemyVisualSetter();
        EnemyMovementTypeSetter();

        gunTr = enemyGunGO.transform.localPosition;
        if (canShoot) 
        {
            ProjectilePooling();
            lR.SetPosition(0, shootOriginTr.position);
            lR.SetPosition(1, lR.GetPosition(0));

        }
    }

    private void Update()
    {
        if(enemyGunGO != null) enemyGunGO.transform.localPosition = gunTr;
        if (GameManager.Instance.levelStarted && isAlive)
        {
            MoveAlongSpline();
            Shoot();
        }   
    }

    public void SetHealth(int healthModfier)
    {
        if (!isInvulnerable && GameManager.Instance.levelStarted)
        {
            currentEnemyHealth += healthModfier;
            if (currentEnemyHealth <= 0)
            {
                EnemyDeath();
            }
        }
    }

    private void EnemyDeath()
    {
        isAlive = false;
        if (canRespawn) StartCoroutine(_RespawnEnemy());
        else
        {
            this.gameObject.SetActive(false);
        }
        if (this.GetComponent<Objective>() != null) this.GetComponent<Objective>().SetCompletedObjective();

        if(enemyBehaviourType == enemyOnHitTypes.TpOnKill)
        {
            GameManager.Instance.currentPlayer.ForcedMovement(this.transform.position);
        }  
    }

    private void MoveAlongSpline()
    {
        if (enemyMovementType == enemyMovementTypes.Movable)
        {
            if (MovementSpline.Spline.Closed)
            {
                distancePercentageSpline += moveSpeed * Time.deltaTime / movementSplineLength;

                Vector3 currentPos = MovementSpline.EvaluatePosition(distancePercentageSpline);
                this.transform.position = currentPos;

                if (distancePercentageSpline >= 1f)
                {
                    distancePercentageSpline = 0f;
                }
            }
        }
    }

    private GameObject currentProjectileGO;
    private void Shoot()
    {
        if (canShoot)
        {
            if (gunCol.objective != null)
            {
                //enemyGunGO.transform.rotation = Quaternion.LookRotation(gunCol.objective.transform.position - enemyGunGO.transform.position);
                lR.SetPosition(0, shootOriginTr.position);
                lR.SetPosition(1, gunCol.objective.transform.position);
                if (currentShootCD >= shootCD)
                {
                    //Shoot!
                    currentProjectileGO.GetComponent<EnemyProjectile>().SetCharged();
                    currentProjectileGO.GetComponent<EnemyProjectile>().LaunchProjectile((gunCol.objective.transform.position - enemyGunGO.transform.position), projectileSpeed);
                    currentProjectileGO = null;
                    currentShootCD = 0;
                }
                else
                {
                    if (currentProjectileGO == null) 
                    {
                        currentProjectileGO = projectilePool[currentProjectilePooled];
                        if (currentProjectilePooled < projectilePool.Count - 1) ++currentProjectilePooled;
                        else currentProjectilePooled = 0;
                        currentProjectileGO.GetComponent<EnemyProjectile>().ProjectileSetUp(0, projectileRange, shootOriginTr);
                        currentProjectileGO.GetComponent<EnemyProjectile>().hitForce = projectileHitForce;
                    }
                    currentProjectileGO.transform.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 0.01f), new Vector3(projectileSize, projectileSize, projectileSize), (currentShootCD / shootCD));
                    currentShootCD += Time.deltaTime;
                }
            }
            else
            {
                currentShootCD = 0;
                if (currentProjectileGO != null) 
                {
                    lR.SetPosition(0, shootOriginTr.position);
                    lR.SetPosition(1, lR.GetPosition(0));
                    currentProjectileGO.GetComponent<Projectile>().SetProjectileInactive();
                    currentProjectileGO = null;
                } 

            }


        }
    }

    private List<GameObject> projectilePool = new List<GameObject>();
    private int currentProjectilePooled = 0;
    private void ProjectilePooling()
    {
        for (int i = 0; i < (projectileRange / shootCD) + 2; i++)
        {
            GameObject newProj = Instantiate(enemyProjectileGO, shootOriginTr.position, Quaternion.identity, shootOriginTr);
            projectilePool.Add(newProj);
            newProj.GetComponent<Projectile>().spawnPos = shootOriginTr;
            newProj.GetComponent<Projectile>().SetProjectileInactive();
        }
    }

    private IEnumerator _RespawnEnemy()
    {
        enemyStuff.SetActive(false);
        currentShootCD = 0;
        if (currentProjectileGO != null) Destroy(currentProjectileGO);
        yield return new WaitForSeconds(respawnTime);
        enemyStuff.SetActive(true);
        isAlive = true;
    }
    private void OnDestroy()
    {
        if(MovementSpline != null) MovementSpline.gameObject.GetComponent<SplineInstantiate>().Clear();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += OnValidateCallBack;
    }

    private void OnValidateCallBack()
    {
        if (this == null)
        {
            UnityEditor.EditorApplication.delayCall -= OnValidateCallBack;
        }
        EnemyMovementTypeSetter();
        EnemyVisualSetter();
        
    }
#endif
    private void EnemyMovementTypeSetter()
    {
        if (enemyMovementType == enemyMovementTypes.Movable)
        {
            if (MovementSpline != null) MovementSpline.gameObject.SetActive(true);
        }
        if (enemyMovementType == enemyMovementTypes.Static)
        {
            if(MovementSpline != null)MovementSpline.gameObject.SetActive(false);
        }
    }
    private void EnemyVisualSetter()
    {
        if (enemyModelGO != null) enemyModelGO.SetActive(false);
        if (enemyBehaviourType == enemyOnHitTypes.Standard)
        {            
            enemyModelGO = enemyStandardModelGO;
            if (enemyModelGO != null) enemyModelGO.SetActive(true);
        }
        if (enemyBehaviourType == enemyOnHitTypes.TpOnKill)
        {
            enemyModelGO = enemyTpOnKillModelGO;
            if (enemyModelGO != null) enemyModelGO.SetActive(true);
        }

        if(enemyGunGO != null)
        {
            if (canShoot)
            {
                enemyGunGO.SetActive(true);
            }
            else
            {
                enemyGunGO.SetActive(false);
            }
        }

        if(shieldGO != null)
        {
            if (isInvulnerable)
            {
                shieldGO.SetActive(true);
            }
            else
            {
                shieldGO.SetActive(false);
            }
        }
        
    }
   
}
