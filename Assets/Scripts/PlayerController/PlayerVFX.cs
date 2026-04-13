using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

public class PlayerVFX : MonoBehaviour
{
    private List<GameObject> footstepsList = new List<GameObject>();

    [SerializeField] private GameObject footStepPS;
    [SerializeField] private Transform leftFootTr;
    [SerializeField] private Transform rightFootTr;

    [SerializeField] private Material playerMasterMat;
    [SerializeField] public float PlayerDissolveTime;
    [SerializeField] public float playerDissolveSpeed;
    [SerializeField] int poolNumberTotal = 10;

    private float dissolveMinValue = -0.5f;
    private float dissolveMaxValue = 2f;
    private int index = 0;

    private void Start()
    {
        ChangeMaterialProperties(2, 0, 1); 
        
        GameObject list = new GameObject("FootstepsPool");
        for (int i = 0; i < poolNumberTotal; i++)
        {
            GameObject a = Instantiate(footStepPS);
            footstepsList.Add(a);
            footstepsList[i].transform.parent = list.transform;
            footstepsList[i].SetActive(false);
        }
    }
    public void PlayLeftFootstepParticles()
    {
        footstepsList[index].transform.position = leftFootTr.transform.position;
        footstepsList[index].transform.rotation = Quaternion.Euler(0, transform.parent.gameObject.transform.rotation.eulerAngles.y, 0);
        footstepsList[index].SetActive(true);
        footstepsList[index].GetComponentInChildren<ParticleSystem>().Play();

        StartCoroutine(DeactivateFootstep(footstepsList[index]));
        index = index >= poolNumberTotal - 1 ? 0 : ++index;
    }
    public void PlayRightFootstepParticles()
    {
        footstepsList[index].transform.position = rightFootTr.transform.position;
        footstepsList[index].transform.rotation = Quaternion.Euler(0, transform.parent.gameObject.transform.rotation.eulerAngles.y, 0);
        footstepsList[index].SetActive(true);
        footstepsList[index].GetComponentInChildren<ParticleSystem>().Play();

        StartCoroutine(DeactivateFootstep(footstepsList[index]));
        index = index >= poolNumberTotal -1 ? 0 : ++index;

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

    IEnumerator DeactivateFootstep(GameObject footstep)
    {
        yield return new WaitForSeconds(1f);

        footstep.SetActive(false);
    }
}
