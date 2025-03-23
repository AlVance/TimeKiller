using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] public GameObject basePlayerCam;
    public GameObject currentCam;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void ChangeCam(GameObject newCam)
    {
        newCam.SetActive(true);
        if(newCam.GetComponent<FollowObject>() != null && newCam.GetComponent<FollowObject>().followPlayer) newCam.GetComponent<FollowObject>().targetTr = GameManager.Instance.currentPlayer.gameObject.transform;
        if(currentCam != null)currentCam.SetActive(false);
        currentCam = newCam;
    }
}
