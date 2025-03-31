using UnityEngine;
using System.Collections;


public class OffLimitsColController : MonoBehaviour
{
    private Transform tpTr;

    private void Start()
    {
        if (this.GetComponentInParent<Level>() != null) tpTr = this.GetComponentInParent<Level>().playerStartTr;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (tpTr != null) 
        {
            if(other.gameObject.tag != "Player")other.gameObject.transform.position = tpTr.position;
            else
            {
                other.gameObject.GetComponent<PlayerController>().PlayerOffLimits(tpTr);
            }
        } 
        else other.gameObject.transform.position = Vector3.zero;
    }
}
