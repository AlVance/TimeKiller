using UnityEngine;

public class ChangeGameMode : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private bool startAsExplorationMode = true;
    private void Start()
    {
        anim = this.GetComponent<Animator>();
        GameManager.Instance.explorationMode = startAsExplorationMode;
        if(PlayerPrefs.HasKey("GameMode"))
        {
            if (PlayerPrefs.GetInt("GameMode") == 0) GameManager.Instance.explorationMode = true;
            else GameManager.Instance.explorationMode = false;
        }
        else
        {
            PlayerPrefs.SetInt("GameMode", 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            GameManager.Instance.explorationMode = !GameManager.Instance.explorationMode;
            anim.SetBool("On", true);

            if(GameManager.Instance.explorationMode) PlayerPrefs.SetInt("GameMode", 0);
            else PlayerPrefs.SetInt("GameMode", 1);
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
