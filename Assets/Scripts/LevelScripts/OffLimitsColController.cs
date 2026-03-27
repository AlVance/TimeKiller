using UnityEngine;
using System.Collections;


public class OffLimitsColController : MonoBehaviour
{
    private Level currentLevel;
    private void Start()
    {
        if (this.transform.GetComponentInParent<Level>() != null)
        {
            currentLevel = this.transform.GetComponentInParent<Level>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentLevel != null) 
        {
            if(other.gameObject.tag == "Player")
            {
                GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
                GameManager.Instance.currentPlayer.transform.position = GameManager.Instance.currentLevelGO.GetComponent<Level>().playerStartTr.position;
                GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().UnblockPlayer();
            }
        } 
        else other.gameObject.transform.position = Vector3.zero;
    }
}
