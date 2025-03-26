using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector]public LevelManager lM;
    [SerializeField] public Transform playerStartTr;
    [SerializeField] public int objectivesToFinish;
    [SerializeField] public int levelTime;
    [SerializeField] public GameObject levelCam;
    [HideInInspector]public int currentObjectives = 0;

    public void SetCurrentObjectivesInt(int currentObjMod)
    {
        currentObjectives += currentObjMod;
        UIManager.Instance.SetObjectivesValueText(currentObjectives, objectivesToFinish);
        if(currentObjectives >= objectivesToFinish)
        {
            lM.OnLevelEnded();
        }
    }
}
