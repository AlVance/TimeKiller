using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{ 
    public float activeTime = 2f;
    public float meshResfreshRate = 0.1f;
    public float destroyTime = 0.3f;

    public Material trailMaterial;

    private bool isTrailActive = false;
    private MeshRenderer[] meshes;

    private void Start()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshResfreshRate;

            for (int i = 0; i < meshes.Length; i++) {
                GameObject go = new GameObject();
                go.transform.SetPositionAndRotation(meshes[i].transform.position, meshes[i].transform.rotation);
                go.transform.localScale = (meshes[i].transform.localScale * transform.localScale.x);

                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                MeshFilter   mf = go.AddComponent<MeshFilter>();

                Mesh _mesh = new Mesh();
                //meshes[i].BakeMesh(_mesh);
                mf.mesh = meshes[i].GetComponent<MeshFilter>().mesh;
                mr.material = trailMaterial;

                Destroy(go, destroyTime);
            }

            yield return new WaitForSeconds(meshResfreshRate);
        }

        isTrailActive = false;
    }
}
