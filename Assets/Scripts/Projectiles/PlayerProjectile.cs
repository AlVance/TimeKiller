using UnityEngine;
using System.Collections;

public class PlayerProjectile : Projectile
{
    [SerializeField] private GameObject impactParticle;
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
            else
            {
                Vector3 rotation = this.transform.rotation.eulerAngles;
                rotation.y -= 180;
                Instantiate(impactParticle, this.transform.position, Quaternion.Euler(rotation));
                Destroy(this.gameObject);

            }

        }
    }
}