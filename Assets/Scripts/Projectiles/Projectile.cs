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
    [SerializeField] private AudioSource shootAS;
    [SerializeField] private AudioClip chargeShootAC, holdShootAC, setChargeAC, shootAC;

    private void Awake()
    {
        collidier = this.gameObject.GetComponent<SphereCollider>();
        collidier.radius = 0;
    }


    private void Update()
    {
        if (launched)
        {
            timeLaunched += Time.deltaTime;
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

        shootAS.loop = false;
        shootAS.clip = chargeShootAC;
        shootAS.Play();
    }
    public void LaunchProjectile(Vector3 direction, float speed)
    {
        this.gameObject.transform.SetParent(null);
        launchSize = this.gameObject.transform.localScale;
        launched = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * speed;


        shootAS.Stop();
        shootAS.loop = false;
        shootAS.PlayOneShot(shootAC);
    }

    public void SetCharged()
    {
        //this.GetComponent<MeshRenderer>().material.color = Color.green;
        collidier.radius = 0.5f;
        charged = true;

        shootAS.Stop();
        shootAS.loop = true;
        shootAS.clip = holdShootAC;
        shootAS.Play();

        shootAS.PlayOneShot(setChargeAC);
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

        shootAS.loop = false;
        shootAS.Stop();

        this.gameObject.SetActive(false);

    }
}
