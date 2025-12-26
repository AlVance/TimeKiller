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
            if(other.gameObject.tag == "Player") other.gameObject.GetComponent<PlayerController>().PlayerOffLimits(currentLevel.playerStartTr);
        } 
        else other.gameObject.transform.position = Vector3.zero;
    }
}
