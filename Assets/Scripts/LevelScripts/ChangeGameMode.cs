using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeGameMode : MonoBehaviour
{
    private Animator anim;
   

    private void Start()
    {
        anim = this.GetComponent<Animator>();        
        if(PlayerPrefs.HasKey("CD_GameMode"))
        {
            if (PlayerPrefs.GetInt("CD_GameMode") == 0) GameManager.Instance.explorationMode = true;
            else GameManager.Instance.explorationMode = false;
        }
        else
        {
            PlayerPrefs.SetInt("CD_GameMode", 0);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            GameManager.Instance.ChangeGameMode();
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
