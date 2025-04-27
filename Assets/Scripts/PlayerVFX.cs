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
    private float dissolveMinValue = -0.5f;
    private float dissolveMaxValue = 2f;

    private void Start()
    {
        ChangeMaterialProperties(2, 0, 1);
    }
    public void PlayLeftFootstepParticles()
    {
        GameObject fs = Instantiate(footStepPS, leftFootTr.transform.position, Quaternion.Euler(0, this.transform.parent.gameObject.transform.rotation.eulerAngles.y,0));
        Destroy(fs, 2);
    }
    public void PlayRightFootstepParticles()
    {
        GameObject fs = Instantiate(footStepPS, rightFootTr.transform.position, Quaternion.Euler(0, this.transform.parent.gameObject.transform.rotation.eulerAngles.y, 0));
        Destroy(fs, 2);
    }

    public void ChangeMaterialProperties(float height, int isDissolving, int isUpwards)
    {
        playerMasterMat.SetInt("_isDissolving", isDissolving);
        playerMasterMat.SetFloat("_Cutoff_height", height);
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
            float currentValue = Mathf.Lerp(dissolveMinValue, dissolveMaxValue, playerDissolveCurrentTime);
            ChangeMaterialProperties(currentValue, 1, isUpwards);
            yield return new WaitForEndOfFrame();
        }
        playerDissolveCurrentTime = 0;
    }

    private void OnDisable()
    {
        ChangeMaterialProperties(2, 0, 1);
    }
}
