using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector]public LevelManager lM;
    [SerializeField] public Transform playerStartTr;
    [SerializeField] private int objectivesToFinish;
    [SerializeField] public int levelTime;
    [SerializeField] public GameObject levelCam;
    private int currentObjectives = 0;

    public void SetCurrentObjectivesInt(int currentObjMod)
    {
        currentObjectives += currentObjMod;
        if(currentObjectives >= objectivesToFinish)
        {
            lM.OnLevelEnded();
        }
    }
}
