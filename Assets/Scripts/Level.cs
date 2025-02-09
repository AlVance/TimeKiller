using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform playerStartTr;
    [SerializeField] private int objectivesToFinish;
    private int currentObjectives = 0;

    private void Start()
    {
        GameManager.Instance.currentPlayer.transform.position = playerStartTr.position;
    }

    public void SetCurrentObjectivesInt(int currentObjMod)
    {
        currentObjectives += currentObjMod;
        if(currentObjectives >= objectivesToFinish)
        {
            LevelManager.Instance.SetNextLevel();
        }
    }
}
