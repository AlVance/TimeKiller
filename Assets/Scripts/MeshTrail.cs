using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MeshTrail : MonoBehaviour
{ 
    public float meshResfreshRate = 0.1f;
    public float destroyTime = 0.2f;

    public Material trailMaterial;

    private SkinnedMeshRenderer[] meshes;

    [SerializeField] Color[] sandColors;
    int nColor = 0;

    [SerializeField] VisualEffect[] smoke_VFX;

    private void Start()
    {  
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public void ActivateSandevistan()
    {
        StartCoroutine(ActivateTrail());
    }
    
    IEnumerator ActivateTrail()
    {
        smoke_VFX[0].Play();
        smoke_VFX[1].Play();

        for (int i = 0; i < meshes.Length; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetPositionAndRotation(meshes[i].transform.position, meshes[i].transform.rotation);

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();

            Mesh _mesh = new Mesh();
            meshes[i].BakeMesh(_mesh);
            mf.mesh = _mesh;
            mr.material = trailMaterial;

            Color sandColor = sandColors[nColor];

            mr.material.SetColor("_Emission_color", sandColor);
            Destroy(go, destroyTime);
        }
        yield return new WaitForSeconds(meshResfreshRate);
        nColor = nColor >= sandColors.Length - 1 ? 0 : ++nColor;

        if (GameManager.Instance.currentPlayer.isFlying) StartCoroutine(ActivateTrail());
        else
        {
            smoke_VFX[0].Stop();
            smoke_VFX[1].Stop();

        }
    }
}
