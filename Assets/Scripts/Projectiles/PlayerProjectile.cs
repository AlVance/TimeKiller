using UnityEngine;
using System.Collections;

public class PlayerProjectile : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (other.gameObject.tag == "Enemy" && charged)
            {
                other.gameObject.GetComponent<EnemyBehaviour>().SetHealth(-projectileDamage);
            }

            if (!launched) 
            {
                GameManager.Instance.currentPlayer.ResetCharge();
            } 
            else Destroy(this.gameObject);
        }
    }
}