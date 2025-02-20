using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public Transform playerStartTr;
    [SerializeField] private int objectivesToFinish;
    [SerializeField] public int levelTime;
    private int currentObjectives = 0;

    public void SetCurrentObjectivesInt(int currentObjMod)
    {
        currentObjectives += currentObjMod;
        if(currentObjectives >= objectivesToFinish)
        {
            LevelManager.Instance.OnLevelEnded();
        }
    }
}
