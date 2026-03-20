using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
public class CamerasFOVController : MonoBehaviour
{
    [SerializeField] private float initialFOV = 70;
    [SerializeField] private float maxFOV = 100;
    private float currentFov;
    private Rigidbody pRb;
    private List<GameObject> cams = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pRb = GameManager.Instance.currentPlayer.GetComponent<Rigidbody>();
        currentFov = initialFOV;

        GetLevelCams();
    }

    // Update is called once per frame
    void Update()
    {
        currentFov = pRb.linearVelocity.sqrMagnitude;
        if (currentFov <= initialFOV) currentFov = initialFOV;
        if (currentFov >= maxFOV) currentFov = maxFOV;

        for (int i = 0; i < cams.Count; i++)
        {
            cams[i].GetComponent<CinemachineCamera>().Lens.FieldOfView = currentFov;
        }
    }

    public void GetLevelCams()
    {
        int camCount = CinemachineCore.VirtualCameraCount;

        for (int i = 0; i < camCount; i++)
        {
            cams.Add(CinemachineCore.GetVirtualCamera(i).gameObject);
        }
    }
}
