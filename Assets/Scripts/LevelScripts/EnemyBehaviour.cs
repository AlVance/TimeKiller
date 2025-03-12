using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using MyBox;
using UnityEditor.Callbacks;

public class EnemyBehaviour : MonoBehaviour
{
    private enum enemyMovementTypes { Static, Movable };
    [SerializeField] private enemyMovementTypes enemyMovementType = enemyMovementTypes.Static;

    private enum enemyOnHitTypes { Standard, TpOnKill};
    [SerializeField] private enemyOnHitTypes enemyBehaviourType = enemyOnHitTypes.Standard;

    [Header("Enemy abilities")]
    [SerializeField] private bool canShoot = false;


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
    private float currentShootCD;
    [SerializeField]private EnemyGunCollider gunCol;

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
        if (canShoot)
        {
            enemyGunGO.transform.position = this.gameObject.transform.position;
        }
    }

    public void SetHealth(int healthModfier)
    {
        currentEnemyHealth += healthModfier;
        if(currentEnemyHealth <= 0)
        {
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        Destroy(this.gameObject);
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


    private void Shoot()
    {
        if (canShoot)
        {
            if (gunCol.objective != null)
            {
                enemyGunGO.transform.rotation = Quaternion.LookRotation(gunCol.objective.transform.position - enemyGunGO.transform.position);

                if (currentShootCD >= shootCD)
                {
                    Debug.Log("Shoot player");
                    currentShootCD = 0;
                }
            }
            
            if(currentShootCD < shootCD || gunCol.objective == null)
            {
                if(currentShootCD < shootCD) currentShootCD += Time.deltaTime;
            }
        }
    }
    private void OnDestroy()
    {
        if(MovementSpline != null) MovementSpline.gameObject.GetComponent<SplineInstantiate>().Clear();
    }

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
