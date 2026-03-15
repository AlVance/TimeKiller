using UnityEngine;

public class EnemyProjectile : Projectile
{
    public float hitForce;
    private void OnTriggerEnter(Collider other)
    {
       
        if ((other.gameObject.tag == "Player" || other.gameObject.layer == LayerMask.NameToLayer("Floor")) && charged)
        {   
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponentInParent<PlayerGetHit>().GetHit(this.transform.position, hitForce);
            }
            
            SetProjectileInactive();
        }
    }
}
