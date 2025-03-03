using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] public Transform targetTr;
    [SerializeField] private Vector3 followOffset;

    [SerializeField] private bool followSmooth;
    [SerializeField] private float followSpeed;

    private void LateUpdate()
    {
        if (!followSmooth)
        {
            this.transform.position = targetTr.position + followOffset;
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        
        if(followSmooth)
        {
            this.transform.position += ((targetTr.position + followOffset) - this.transform.position) * followSpeed * Time.deltaTime;
        }
    }
}
