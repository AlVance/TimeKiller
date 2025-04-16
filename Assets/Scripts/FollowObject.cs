using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    [SerializeField] public Transform targetTr;
    [SerializeField] public Vector3 followOffset;

    [SerializeField] private bool followSmooth;
    [SerializeField] private float followSpeed;
    [SerializeField] public bool followPlayer;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        if(followPlayer && targetTr == null) targetTr = GameManager.Instance.currentPlayer.gameObject.transform;
        if(targetTr != null) this.transform.position = targetTr.position + followOffset;
    }

    private void LateUpdate()
    {
        if (!followSmooth && targetTr != null)
        {
            this.transform.position = targetTr.position + followOffset;
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        
        if(followSmooth && targetTr != null)
        {
            this.transform.position += ((targetTr.position + followOffset) - this.transform.position) * followSpeed * Time.deltaTime;
        }
    }
}
