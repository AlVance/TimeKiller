using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] private int objectivePoints = 1;

    public void SetCompletedObjective()
    {
        LevelManager.Instance.currentLevelGO.GetComponent<Level>().SetCurrentObjectivesInt(objectivePoints);
    }
}
