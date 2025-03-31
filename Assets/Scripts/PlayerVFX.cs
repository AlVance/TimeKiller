using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private GameObject footStepPS;
    [SerializeField] private Transform leftFootTr;
    [SerializeField] private Transform rightFootTr;
    [SerializeField] private Material playerMasterMat;
    [SerializeField] public float PlayerDissolveTime;
    public void PlayLeftFootstepParticles()
    {
        Instantiate(footStepPS, leftFootTr.transform.position, rightFootTr.rotation);
    }
    public void PlayRightFootstepParticles()
    {
        Instantiate(footStepPS, rightFootTr.transform.position, leftFootTr.rotation);
    }

    public void ChangeMaterialProperties(float height, int isDissolving, int isUpwards)
    {
        //Realmente el par�metro puede ser su Material en vez del MR
        //Recuerda que de momento el 0 est� en el pibote del broski, puede que el "0" est� en -2 y tengas que acabarlo ah�
        //No existe el setBool
        playerMasterMat.SetInt("_isDissolving", isDissolving);
        playerMasterMat.SetFloat("_Cutoff_height", height);

        //Por si quieres hacerlo de arriba a abajo o al rev�s
        playerMasterMat.SetInt("_isUpwards", isUpwards);
    }

    private float playerDissolveCurrentTime = 0;
    public void DissolvePlayer(int isUpwards)
    {
        while(playerDissolveCurrentTime < PlayerDissolveTime)
        {
            playerDissolveCurrentTime += Time.deltaTime;
            ChangeMaterialProperties(playerDissolveCurrentTime, 1, isUpwards);
        }
        playerDissolveCurrentTime = 0;
    }
}
