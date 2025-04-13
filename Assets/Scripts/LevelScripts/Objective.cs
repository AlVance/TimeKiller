using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] private int objectivePoints = 1;
    [SerializeField] private GameObject objPS;

    private void Start()
    {
        if(objectivePoints > 0 && objPS != null)
        {
            Instantiate(objPS, this.transform);
        }
    }
    public void SetCompletedObjective()
    {
        LevelManager.Instance.currentLevelGO.GetComponent<Level>().SetCurrentObjectivesInt(objectivePoints);
    }
}
