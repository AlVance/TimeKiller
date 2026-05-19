using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;


public class PlayerGetHit : MonoBehaviour
{
    private PlayerBlock pBlock;
    private Rigidbody rb;

    [SerializeField] private float invulneravilityTime;
    private bool isInvulnerable = false;
    [SerializeField] private float stunnedTime;
    public bool isHitted;

    [SerializeField] private MMF_Player onHitFeedback;

    private void Start()
    {
        pBlock = GetComponent<PlayerBlock>();
        rb = GetComponent<Rigidbody>();
    }

    public void GetHit(Vector3 hitPos, float hitForce)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(_HitBlock(hitPos, hitForce));
            StartCoroutine(_InvTime());
            onHitFeedback.PlayFeedbacks();
        }
    }

    private IEnumerator _HitBlock(Vector3 _hitPos, float _hitForce)
    {
        isHitted = true;
        pBlock.BlockPlayer();
        rb.AddForce((this.transform.position - _hitPos) * _hitForce, ForceMode.Force);
        yield return new WaitForSeconds(stunnedTime);
        isHitted = false;
        pBlock.UnblockPlayer();
    }

    private IEnumerator _InvTime()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulneravilityTime);
        isInvulnerable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyHitBox")
        {
            GetHit(other.gameObject.transform.position, 700);
        }
    }

    private void OnDisable()
    {
        isInvulnerable = true;
    }

    private void OnEnable()
    {
        isInvulnerable = false;
    }
}
