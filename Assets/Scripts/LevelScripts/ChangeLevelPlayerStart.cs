using UnityEngine;

public class ChangeLevelPlayerStart : MonoBehaviour
{
    private Level level;
    [SerializeField] private Transform newPlayerStartTr;

    [SerializeField] GameObject rippleGO;
    [SerializeField] GameObject playerGO;
    [SerializeField] Color matColor;
    [SerializeField] private AudioSource spawnAS;

    private void Start()
    {
        if (this.transform.parent.TryGetComponent<Level>(out Level parentLevel)) { level = parentLevel; } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && level != null && level.playerStartTr != newPlayerStartTr)
        {
            level.playerStartTr = newPlayerStartTr;

            rippleGO.GetComponent<MeshRenderer>().material.SetColor("_Wave_Color", matColor);
            playerGO.GetComponent<MeshRenderer>().material.SetColor("_Main_Color", matColor);
            playerGO.GetComponent<MeshRenderer>().material.SetColor("_Fresnel_Color", matColor);

            spawnAS.Play();
        }
    }
}
