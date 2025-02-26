using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using MyBox;
using UnityEditor.Callbacks;

public class EnemyBehaviour : MonoBehaviour
{
    private enum enemyMovementTypes { Static, Movable };
    [SerializeField] private enemyMovementTypes enemyMovementType = enemyMovementTypes.Static;

    private enum enemyBehaviourTypes { Standard, TpOnKill};
    [SerializeField] private enemyBehaviourTypes enemyBehaviourType = enemyBehaviourTypes.Standard;


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

    [Header("Visual variables")]
    private GameObject enemyModelGO;
    [SerializeField] private GameObject enemyStandardModelGO;
    [SerializeField] private GameObject enemyTpOnKillModelGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnemyHealth = enemyHealth;
        if (enemyMovementType == enemyMovementTypes.Movable)
        {
            movementSplineLength = MovementSpline.CalculateLength();
            MovementSpline.gameObject.transform.parent = this.transform.parent;
        }
        else
        {
            MovementSpline.gameObject.SetActive(false);
        }

        EnemyBehaviourTypeSetter();
        EnemyMovementTypeSetter();
    }

    private void Update()
    {
        MoveAlongSpline();
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

        if(enemyBehaviourType == enemyBehaviourTypes.TpOnKill)
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
        EnemyBehaviourTypeSetter();
        
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
    private void EnemyBehaviourTypeSetter()
    {
        if (enemyModelGO != null) enemyModelGO.SetActive(false);
        if (enemyBehaviourType == enemyBehaviourTypes.Standard)
        {            
            enemyModelGO = enemyStandardModelGO;
            if (enemyModelGO != null) enemyModelGO.SetActive(true);
        }
        if (enemyBehaviourType == enemyBehaviourTypes.TpOnKill)
        {
            enemyModelGO = enemyTpOnKillModelGO;
            if (enemyModelGO != null) enemyModelGO.SetActive(true);
        }
    }
}
