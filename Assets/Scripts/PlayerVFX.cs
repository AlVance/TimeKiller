using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private GameObject footStepPS;
    [SerializeField] private Transform leftFootTr;
    [SerializeField] private Transform rightFootTr;
    public void PlayLeftFootstepParticles()
    {
        Instantiate(footStepPS, leftFootTr.transform.position, rightFootTr.rotation);
    }
    public void PlayRightFootstepParticles()
    {
        Instantiate(footStepPS, rightFootTr.transform.position, leftFootTr.rotation);
    }

    public void ChangeMaterialProperties(MeshRenderer playerMR)
    {
        //Realmente el par�metro puede ser su Material en vez del MR
        //Recuerda que de momento el 0 est� en el pibote del broski, puede que el "0" est� en -2 y tengas que acabarlo ah�

        float altura = 0.0f;
        //Haces tus c�lculos pa que cambie con el tiempo
        Material material = playerMR.material;
        //No existe el setBool
        playerMR.material.SetInt("_isDissolving", 1);
        playerMR.material.SetFloat("_Cutoff_height", altura);

        //Por si quieres hacerlo de arriba a abajo o al rev�s
        playerMR.material.SetInt("_isUpwards", 1);
    }
}
