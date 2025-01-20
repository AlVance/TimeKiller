using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public PlayerController PC;
    public bool launched = false;
    public bool charged = false;
    public int projectileDamage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            if (other.gameObject.tag == "Enemy" && charged)
            {
                other.gameObject.GetComponent<EnemyBehaviour>().SetHealth(-projectileDamage);
            }

            if (!launched) PC.ResetCharge();
            else Destroy(this.gameObject);
        }
    }
}
