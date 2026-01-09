using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private RaycastHit hit;
    [SerializeField] private float rayOffset;
    [SerializeField] private float hitOffset;
    Vector3 hitPosition;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private Vector2 minMaxSize;
    [SerializeField] private Vector2 minMaxDistance;
    private void Start()
    {
        shadow.transform.SetParent(null);
    }

    private void Update()
    {
        shadow.transform.position = new Vector3(this.transform.position.x, hitPosition.y + hitOffset, this.transform.position.z);
        shadow.transform.Rotate(new Vector3(0, 1, 0) * rotateSpeed * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        Ray downRay = new Ray(new Vector3(this.transform.position.x, this.transform.position.y + rayOffset, this.transform.position.z), -Vector3.up * 10000f);
       
        if (Physics.Raycast(downRay, out hit))
        {
            //print(hit.transform);
            hitPosition = hit.point;
            SetBlobShadowSize();
        }
    }

    private void SetBlobShadowSize()
    {
        float distance = Vector3.Distance(this.transform.position, hitPosition);
        if (distance < minMaxDistance.x) shadow.transform.localScale = new Vector3(minMaxSize.x, 0, minMaxSize.x);
        else if (distance > minMaxDistance.y) shadow.transform.localScale = new Vector3(minMaxSize.y, 0, minMaxSize.y);
        else
        {
            float cDis = distance / minMaxDistance.y;
            float scaleValue = minMaxSize.x + (cDis * (minMaxSize.y - minMaxSize.x));
            shadow.transform.localScale = new Vector3(scaleValue, 0, scaleValue);
        }
    }
}
