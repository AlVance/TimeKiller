using UnityEngine;

public class EnemyProjectile : Projectile
{
    public float hitForce;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (other.gameObject.tag == "Player" && charged)
            {
                other.gameObject.GetComponent<PlayerController>().GetHit(this.transform.position, hitForce);
            }
            SetProjectileInactive();
        }
    }
}
