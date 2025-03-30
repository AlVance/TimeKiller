using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using MyBox;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

public class EnemyBehaviour : MonoBehaviour
{
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

    [Header("RespawnVariables")]
    [SerializeField, ConditionalField(nameof(canRespawn), false)] private float respawnTime = 5f;

    [Header("Visual variables")]
    [SerializeField] private GameObject enemyStandardModelGO;
    [SerializeField] private GameObject enemyTpOnKillModelGO;
    [SerializeField] private GameObject enemyGunGO;
    private GameObject enemyModelGO;
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

        
    }

    private void Update()
    {
        if (GameManager.Instance.levelStarted)
        {
            MoveAlongSpline();
            Shoot();
        }   
    }

    public void SetHealth(int healthModfier)
    {
        if (!isInvulnerable)
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
        this.gameObject.SetActive(false);
        if (this.GetComponent<Objective>() != null) this.GetComponent<Objective>().SetCompletedObjective();

        if(enemyBehaviourType == enemyOnHitTypes.TpOnKill)
        {
            GameManager.Instance.currentPlayer.ForcedMovement(this.transform.position);
        }

        if (canRespawn) StartCoroutine(_RespawnEnemy());
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
                enemyGunGO.transform.rotation = Quaternion.LookRotation(gunCol.objective.transform.position - enemyGunGO.transform.position);

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
                        currentProjectileGO = Instantiate(enemyProjectileGO, shootOriginTr.position, Quaternion.identity, shootOriginTr);
                        currentProjectileGO.GetComponent<EnemyProjectile>().ProjectileSetUp(0, projectileRange);
                        currentProjectileGO.GetComponent<EnemyProjectile>().hitForce = projectileHitForce;
                    }
                    currentProjectileGO.transform.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 0.01f), new Vector3(projectileSize, projectileSize, projectileSize), (currentShootCD / shootCD));
                    currentShootCD += Time.deltaTime;
                }
            }
            
            //if(currentShootCD < shootCD || gunCol.objective == null)
            //{
            //    if(currentShootCD < shootCD) currentShootCD += Time.deltaTime;
            //}

            enemyGunGO.transform.position = this.gameObject.transform.position;
        }
    }

    private IEnumerator _RespawnEnemy()
    {
        yield return new WaitForSeconds(respawnTime);
        this.gameObject.SetActive(true);
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
        
    }
   
}
