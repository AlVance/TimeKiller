using System.Collections;
using System.Collections.Generic;
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

    private List<GameObject> GOmeshes;
    [SerializeField] int poolNumber = 4;
    int index = 0;


    private void Start()
    {
        CreatePool();
    }

    public void ActivateSandevistan()
    {
        _ActivateSandevistan();
    }
    
    private void SpawnMesh()
    {
        this.gameObject.transform.SetParent(null);
    }

    private void _ActivateSandevistan()
    {
        smoke_VFX[0].Play();
        smoke_VFX[1].Play();

        for (int i = 0; i < meshes.Length; i++)
        {
            GOmeshes[index].transform.GetChild(i).transform.SetPositionAndRotation(meshes[i].transform.position, meshes[i].transform.rotation);

            Mesh _mesh = new Mesh();
            meshes[i].BakeMesh(_mesh);
            GOmeshes[index].transform.GetChild(i).GetComponent<MeshFilter>().mesh = _mesh;
            GOmeshes[index].transform.GetChild(i).GetComponent<MeshRenderer>().material = trailMaterial;

            Color sandColor = sandColors[nColor];

            GOmeshes[index].transform.GetChild(i).GetComponent<MeshRenderer>().material.SetColor("_Emission_color", sandColor);
        }

        GOmeshes[index].SetActive(true);
        StartCoroutine(SetActiveFalse(GOmeshes[index]));
        index = index >= poolNumber - 1 ? 0 : ++index;
        nColor = nColor >= sandColors.Length - 1 ? 0 : ++nColor;

        StartCoroutine(_ActivateTrail());
    }
    private IEnumerator _ActivateTrail()
    {
        yield return new WaitForSeconds(meshResfreshRate);

        if (GameManager.Instance.currentPlayer.isFlying) _ActivateSandevistan();
        else
        {
            smoke_VFX[0].Stop();
            smoke_VFX[1].Stop();
        }
    }

    IEnumerator SetActiveFalse(GameObject go)
    {
        yield return new WaitForSeconds(destroyTime);
        go.SetActive(false);
    }

    private void CreatePool()
    {
        GameObject trailsParent = new GameObject("Trails");
        GOmeshes = new List<GameObject>();
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < poolNumber; i++)
        {
            GameObject BigGO = new GameObject();
            for (int t = 0; t < meshes.Length; t++)
            {
                GameObject go = new GameObject();
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                MeshFilter mf = go.AddComponent<MeshFilter>();
                go.transform.parent = BigGO.transform;
            }
            GOmeshes.Add(BigGO);
            BigGO.transform.parent = trailsParent.transform;
        }
    }
}
