using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftFootStepPS;
    [SerializeField] private ParticleSystem rightFootStepPS;
    public void PlayLeftFootStepParticles()
    {
        leftFootStepPS.Play();
    }

    public void PlayRightFootStepParticles()
    {
        rightFootStepPS.Play();
    }
}
