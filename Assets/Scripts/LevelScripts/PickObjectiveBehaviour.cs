using UnityEngine;

public class PickObjectiveBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(this.gameObject.GetComponent<Objective>() != null)
            {
                this.gameObject.GetComponent<Objective>().SetCompletedObjective();
            }
            Destroy(this.gameObject);
        }
    }
}
