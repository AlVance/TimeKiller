using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] private int objectivePoints = 1;
    [SerializeField] private GameObject objPS;

    private void Start()
    {
        if(objectivePoints > 0 && objPS != null)
        {
            GameObject objPSGO = Instantiate(objPS, this.transform);
            objPSGO.transform.localScale = this.gameObject.transform.localScale;
        }
    }
    public void SetCompletedObjective()
    {
        GameManager.Instance.currentLevelGO.GetComponent<Level>().SetCurrentObjectivesInt(objectivePoints);
    }
}
