using UnityEngine;

public class TryJumpPlatform : MonoBehaviour
{
    [SerializeField] private float force;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.up * force, ForceMode.Impulse);
        }
    }
}
