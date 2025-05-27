using UnityEngine;
using System.Collections;


public class OffLimitsColController : MonoBehaviour
{
    private Level currentLevel;
    private void Start()
    {
        if (this.transform.parent.TryGetComponent<Level>(out Level parentLevel))
        {
            currentLevel = parentLevel;
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
