using UnityEngine;
using System.Collections;

public class PlayerProjectile : Projectile
{
    [SerializeField] private GameObject impactParticle;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyHurtBox" || other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (other.gameObject.tag == "EnemyHurtBox" && charged)
            {
                other.gameObject.GetComponentInParent<EnemyBehaviour>().SetHealth(-projectileDamage);
                
                Vector3 rotation = this.transform.rotation.eulerAngles;
                rotation.y -= 180;
                Destroy(Instantiate(impactParticle, this.transform.position, Quaternion.Euler(rotation)), 2);
                if(!launched)GameManager.Instance.currentPlayer.ResetCharge();
                else SetProjectileInactive();
            }

        }
    }
}