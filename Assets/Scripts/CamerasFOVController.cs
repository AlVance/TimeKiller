using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
public class CamerasFOVController : MonoBehaviour
{
    [SerializeField] private float initialFOV = 70;
    [SerializeField] private float maxFOV = 100;
    [SerializeField] private AnimationCurve FOVCurve;
    public float currentFov;
    private Rigidbody pRb;
    private List<GameObject> cams = new List<GameObject>();
    [SerializeField] private float changeSpeed;
    float currentChangeSpeed = 0;
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
        float pSpeed = new Vector3(pRb.linearVelocity.x, pRb.linearVelocity.y / 2, pRb.linearVelocity.z).sqrMagnitude;
        if (currentFov > FOVCurve.Evaluate(pSpeed)) currentChangeSpeed = changeSpeed * 4;
        else currentChangeSpeed = changeSpeed;
       currentFov = Mathf.Lerp(currentFov, FOVCurve.Evaluate(pSpeed), changeSpeed * Time.deltaTime);
        if (currentFov <= initialFOV) currentFov = initialFOV;
        if (currentFov >= maxFOV) currentFov = maxFOV;

        for (int i = 0; i < cams.Count; i++)
        {
            if(cams[i].GetComponent<CinemachineCamera>() != null) cams[i].GetComponent<CinemachineCamera>().Lens.FieldOfView = currentFov;
        }
    }

    public void GetLevelCams()
    {
        if (cams.Count > 0) cams.Clear();
        int camCount = CinemachineCore.VirtualCameraCount;

        for (int i = 0; i < camCount; i++)
        {
            cams.Add(CinemachineCore.GetVirtualCamera(i).gameObject);
            cams[i].GetComponent<CinemachineCamera>().Lens.FieldOfView = initialFOV;
        }
    }
}
