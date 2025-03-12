using UnityEngine;

public class EnemyGunCollider : MonoBehaviour
{
    public GameObject objective;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            objective = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            objective = null;
        }
    }
}
