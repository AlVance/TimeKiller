using System;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] private int objectivePoints = 1;
    [SerializeField] private GameObject objPS;
    [SerializeField] private Transform objPSSpawnPos;
    [SerializeField] private GameObject acquiredPS;
    [SerializeField] private AudioClip objectiveAC;

    private void Start()
    {   
        if(objectivePoints > 0 && objPS != null)
        {
            GameObject objPSGO;
            if (objPSSpawnPos == null) objPSGO = Instantiate(objPS, this.transform);
            else objPSGO = Instantiate(objPS, objPSSpawnPos);
            objPSGO.transform.localScale = this.gameObject.transform.localScale;
        }
    }
    public void SetCompletedObjective()
    {
        if (GameManager.Instance.levelStarted && objectivePoints > 0)
        {
            GameManager.Instance.currentLevelGO.GetComponent<Level>().SetCurrentObjectivesInt(objectivePoints);
            GameObject GO = Instantiate(acquiredPS, this.transform.position, this.transform.rotation);
            SoundManager.Instance.PlayOneShootAudio(objectiveAC);
            Destroy(GO, 3);
        }
    }
}
