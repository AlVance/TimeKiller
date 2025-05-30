using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]private Rigidbody rb;

    public bool launched = false;
    public bool charged = false;
    public int projectileDamage;
    public float lifeTime;
    private float timeLaunched = 0;
    private Vector3 launchSize;
    public bool isActive = false;
    public Transform spawnPos;
    private SphereCollider collidier;
    private void Awake()
    {
        collidier = this.gameObject.GetComponent<SphereCollider>();
        collidier.radius = 0;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        if (launched)
        {
            timeLaunched += Time.deltaTime;
            //this.transform.localScale = Vector3.Slerp(launchSize, Vector3.zero, (timeLaunched / lifeTime));
            if (timeLaunched >= lifeTime) SetProjectileInactive();
        }
    }

    public void ProjectileSetUp(int _damage, float _lifeTime, Transform _spawnPos)
    {
        this.gameObject.SetActive(true);
        projectileDamage = _damage;
        lifeTime = _lifeTime;
        isActive = true;
        spawnPos = _spawnPos;
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
        //this.GetComponent<MeshRenderer>().material.color = Color.green;
        collidier.radius = 0.5f;
        charged = true;
    }

    public void SetProjectileInactive()
    {
        launched = false;
        charged = false;
        collidier.radius = 0;
        if (!rb.isKinematic) rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        if(spawnPos != null)
        {
            this.transform.position = spawnPos.position;
            this.gameObject.transform.SetParent(spawnPos);           
        }
        timeLaunched = 0;
        this.gameObject.SetActive(false);

    }
}
