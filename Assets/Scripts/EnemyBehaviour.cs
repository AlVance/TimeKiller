using UnityEngine;
using UnityEngine.Splines;

public class EnemyBehaviour : MonoBehaviour
{

    [SerializeField] private int enemyHealth = 1;
    private int currentEnemyHealth;

    [Header("Movement variables")]
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private bool moveAlongSpline = false;
    [SerializeField] private SplineContainer movementSpline;
    private float distancePercentageSpline;
    private float movementSplineLength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnemyHealth = enemyHealth;
        if (moveAlongSpline)
        {
            movementSplineLength = movementSpline.CalculateLength();
            movementSpline.gameObject.transform.parent = null;
        }
        else
        {
            movementSpline.gameObject.SetActive(false);
        }
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
    }

    private void MoveAlongSpline()
    {
        if (moveAlongSpline)
        {
            if (movementSpline.Spline.Closed)
            {
                distancePercentageSpline += moveSpeed * Time.deltaTime / movementSplineLength;

                Vector3 currentPos = movementSpline.EvaluatePosition(distancePercentageSpline);
                this.transform.position = currentPos;

                if (distancePercentageSpline >= 1f)
                {
                    distancePercentageSpline = 0f;
                }
            }
        }
    }
}
