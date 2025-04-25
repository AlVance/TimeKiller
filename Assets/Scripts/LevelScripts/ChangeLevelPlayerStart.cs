using UnityEngine;

public class ChangeLevelPlayerStart : MonoBehaviour
{
    private Level level;
    [SerializeField] private Transform newPlayerStartTr;

    private void Start()
    {
        if(this.transform.parent.TryGetComponent<Level>(out Level parentLevel))
        {
            level = parentLevel;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && level != null && level.playerStartTr != newPlayerStartTr)
        {
            level.playerStartTr = newPlayerStartTr;
        }
    }
}
