using UnityEngine;
using System.Collections;

public class PlayerGetHit : MonoBehaviour
{
    private PlayerBlock pBlock;
    private Rigidbody rb;

    [SerializeField] private float invulneravilityTime;
    private bool isInvulnerable = false;
    [SerializeField] private float stunnedTime;
    public bool isHitted;

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
        }
    }

    private IEnumerator _HitBlock(Vector3 _hitPos, float _hitForce)
    {
        pBlock.BlockPlayer();
        rb.AddForce((this.transform.position - _hitPos) * _hitForce, ForceMode.Force);
        yield return new WaitForSeconds(stunnedTime);
        pBlock.UnblockPlayer();
    }

    private IEnumerator _InvTime()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulneravilityTime);
        isInvulnerable = false;
    }
}
