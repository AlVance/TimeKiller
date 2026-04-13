using UnityEngine;

public class CameraChangeController : MonoBehaviour
{
    [Header("ON ENTER")]
    [SerializeField] private bool triggerOnEnter = true;
    [SerializeField] private GameObject onEnter_newCam;
    [SerializeField] private bool onEnter_returnToLevelCam = false;
    [SerializeField] private bool onEnter_returnToBaseCam = false;

    [Header("ON EXIT")]
    [SerializeField] private bool triggerOnExit = false;
    [SerializeField] private GameObject onExit_newCam;
    [SerializeField] private bool onExit_returnToLevelCam = false;
    [SerializeField] private bool onExit_returnToBaseCam = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && triggerOnEnter)
        {
            if (onEnter_returnToLevelCam && CameraManager.Instance.levelCamera != null) CameraManager.Instance.ChangeCam(CameraManager.Instance.levelCamera);
            else if(onEnter_returnToBaseCam) CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
            else CameraManager.Instance.ChangeCam(onEnter_newCam);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && triggerOnExit)
        {
            if (onExit_returnToLevelCam && CameraManager.Instance.levelCamera != null) CameraManager.Instance.ChangeCam(CameraManager.Instance.levelCamera);
            else if (onExit_returnToBaseCam) CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
            else CameraManager.Instance.ChangeCam(onExit_newCam);
        }
    }
}
