using UnityEngine;

public class ChangeGameMode : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private bool startAsExplorationMode = true;
    private void Start()
    {
        anim = this.GetComponent<Animator>();
        GameManager.Instance.explorationMode = startAsExplorationMode;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            GameManager.Instance.explorationMode = !GameManager.Instance.explorationMode;
            anim.SetBool("On", true);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.SetBool("On", false);
        }

    }
}
