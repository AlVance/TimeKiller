using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] private PlayerController pC;
    [SerializeField] private GameObject shadow;
    [SerializeField] private RaycastHit hit;
    [SerializeField] private float offset;

    private void Start()
    {
        shadow.transform.SetParent(null);
    }
    private void FixedUpdate()
    {
        Ray downRay = new Ray(new Vector3(this.transform.position.x, this.transform.position.y - offset, this.transform.position.z), -Vector3.up);

        //gets the hit from the raycast and converts it unto a vector3
        Vector3 hitPosition = hit.point;
        //transofrm the shadow to the location
        shadow.transform.position = hitPosition;

        ////Cast a ray straight downwards, reads back where it lands (this is optional but reccomended)
        if (Physics.Raycast(downRay, out hit))
        {
            print(hit.transform);
        }
    }
}
