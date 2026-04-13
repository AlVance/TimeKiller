using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject shadowParent;
    [SerializeField] private RaycastHit hit;
    [SerializeField] private float rayOffset;
    [SerializeField] private float hitOffset;
    Vector3 hitPosition;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private Vector2 minMaxSize;
    [SerializeField] private Vector2 minMaxDistance;
    [SerializeField] private LayerMask groundLayer;
    private void Start()
    {
        shadowParent.transform.SetParent(null);
    }
    float a = 1;
    private void Update()
    {
        shadowParent.transform.position = new Vector3(this.transform.position.x, hitPosition.y + hitOffset, this.transform.position.z);
        
        Vector3 groundHitAngle = Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles;
        a += rotateSpeed * Time.deltaTime;
        shadowParent.transform.rotation = Quaternion.Euler(new Vector3(groundHitAngle.x, 0, groundHitAngle.z));
        shadow.transform.rotation = Quaternion.Euler(0, a, 0);
        //shadow.transform.rotation = Quaternion.Euler(new Vector3(shadow.transform.rotation.eulerAngles.x, shadow.transform.rotation.eulerAngles.y + 1 * rotateSpeed * Time.deltaTime, shadow.transform.rotation.eulerAngles.z));

    }
    private void FixedUpdate()
    {
        Ray downRay = new Ray(new Vector3(this.transform.position.x, this.transform.position.y + rayOffset, this.transform.position.z), -Vector3.up * 10000f);
       
        if (Physics.Raycast(downRay, out hit, 1000000f, groundLayer))
        {
            hitPosition = hit.point;
            SetBlobShadowSize();
        }
    }

    private void SetBlobShadowSize()
    {
        float distance = Vector3.Distance(this.transform.position, hitPosition);
        if (distance < minMaxDistance.x) shadowParent.transform.localScale = new Vector3(minMaxSize.x, 0, minMaxSize.x);
        else if (distance > minMaxDistance.y) shadowParent.transform.localScale = new Vector3(minMaxSize.y, 0, minMaxSize.y);
        else
        {
            float cDis = distance / minMaxDistance.y;
            float scaleValue = minMaxSize.x + (cDis * (minMaxSize.y - minMaxSize.x));
            shadowParent.transform.localScale = new Vector3(scaleValue, 0, scaleValue);
        }
    }
}
