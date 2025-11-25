using UnityEngine;
using FMODUnity;

public class AimHoldPitchController : MonoBehaviour
{
    public StudioEventEmitter audAimHold;
    [SerializeField] private float maxChargeTimeForPitch = 6f;
    private float audCurrentChargeTime = 0f;
    private bool isCharging = false;
    void Update()
    {
        if (isCharging)
        {
            if (audCurrentChargeTime < maxChargeTimeForPitch)
            {
                audCurrentChargeTime += Time.deltaTime;
            }
            audAimHold.SetParameter("AimPitch", audCurrentChargeTime);
        }
    }

    public void AudStartCharging()
    {
        isCharging = true;
        audCurrentChargeTime = 0f;
    }

    public void AudStopCharging()
    {
        isCharging = false;
        audAimHold.SetParameter("AimPitch", 0f);
    }
}
