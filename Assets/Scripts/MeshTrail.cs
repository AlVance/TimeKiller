using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{ 
    public float meshResfreshRate = 0.1f;
    public float destroyTime = 0.2f;

    public Material trailMaterial;

    private bool isTrailActive = false;
    private SkinnedMeshRenderer[] meshes;

    [SerializeField] Color[] sandColors;
    int nColor = 0;

    private void Start()
    {  
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        GameManager.Instance.currentPlayer.OnStartFlyEvent.AddListener(ActivateSandevistan);
        Debug.Log(sandColors.Length);

    }

    private void ActivateSandevistan()
    {
        StartCoroutine(ActivateTrail());
    }
    
    IEnumerator ActivateTrail()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetPositionAndRotation(meshes[i].transform.position, meshes[i].transform.rotation);
            //go.transform.localScale = (meshes[i].transform.localScale * transform.localScale.x);
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
    }
}
