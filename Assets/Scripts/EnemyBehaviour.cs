using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    [SerializeField] private int enemyHealth = 1;
    private int currentEnemyHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnemyHealth = enemyHealth;
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
}
