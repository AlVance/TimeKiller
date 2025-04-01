using UnityEngine;
using System.Collections;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private GameObject footStepPS;
    [SerializeField] private Transform leftFootTr;
    [SerializeField] private Transform rightFootTr;
    [SerializeField] private Material playerMasterMat;
    [SerializeField] public float PlayerDissolveTime;
    [SerializeField] public float playerDissolveSpeed;

    private void Start()
    {
        ChangeMaterialProperties(2, 0, 1);
    }
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
        //Realmente el parámetro puede ser su Material en vez del MR
        //Recuerda que de momento el 0 está en el pibote del broski, puede que el "0" esté en -2 y tengas que acabarlo ahí
        //No existe el setBool
        playerMasterMat.SetInt("_isDissolving", isDissolving);
        playerMasterMat.SetFloat("_Cutoff_height", height);

        //Por si quieres hacerlo de arriba a abajo o al revés
        playerMasterMat.SetInt("_isUpwards", isUpwards);
    }

    private float playerDissolveCurrentTime = 0;
    public void DissolvePlayer(int isUpwards)
    {
        StartCoroutine(_DissolvePlayer(isUpwards));
    }

    private IEnumerator _DissolvePlayer(int isUpwards)
    {
        while (playerDissolveCurrentTime < PlayerDissolveTime)
        {
            playerDissolveCurrentTime += playerDissolveSpeed * Time.deltaTime;
            Debug.Log(playerDissolveCurrentTime);
            ChangeMaterialProperties(playerDissolveCurrentTime / PlayerDissolveTime, 1, isUpwards);
            yield return new WaitForEndOfFrame();
        }
        playerDissolveCurrentTime = 0;
    }

    private void OnDisable()
    {
        ChangeMaterialProperties(2, 0, 1);
    }
}
