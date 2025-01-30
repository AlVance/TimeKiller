using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour
{
    private Rigidbody rb;

    public PlayerController PC;
    public bool launched = false;
    public bool charged = false;
    public int projectileDamage;
    public float lifeTime;
    private float timeLaunched = 0;
    private Vector3 launchSize;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (launched)
        {
            timeLaunched += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(launchSize, Vector3.zero, (timeLaunched / lifeTime));
            if(timeLaunched >= lifeTime) Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            if (other.gameObject.tag == "Enemy" && charged)
            {
                other.gameObject.GetComponent<EnemyBehaviour>().SetHealth(-projectileDamage);
            }

            if (!launched) 
            {
                PC.ResetCharge();
            } 
            else Destroy(this.gameObject);
        }
    }

    public void ProjectileSetUp(PlayerController _PC, int _damage, float _lifeTime)
    {
        PC = _PC;
        projectileDamage = _damage;
        lifeTime = _lifeTime;
    }
    public void LaunchProjectile(Vector3 direction, float speed)
    {
        launchSize = this.gameObject.transform.localScale;
        launched = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * speed;
    }
}
