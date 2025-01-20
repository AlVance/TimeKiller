using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform targetTr;
    [SerializeField] private Vector3 followOffset;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = targetTr.position + followOffset;
    }
}
