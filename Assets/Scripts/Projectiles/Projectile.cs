using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;

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
            //this.transform.localScale = Vector3.Slerp(launchSize, Vector3.zero, (timeLaunched / lifeTime));
            if (timeLaunched >= lifeTime) Destroy(this.gameObject);
        }
    }

    public void ProjectileSetUp(int _damage, float _lifeTime)
    {
        projectileDamage = _damage;
        lifeTime = _lifeTime;
    }
    public void LaunchProjectile(Vector3 direction, float speed)
    {
        this.gameObject.transform.SetParent(null);
        launchSize = this.gameObject.transform.localScale;
        launched = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * speed;
    }

    public void SetCharged()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.green;
        charged = true;
    }
}
