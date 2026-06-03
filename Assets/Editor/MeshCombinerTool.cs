using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MeshCombinerTool : EditorWindow
{
    [MenuItem("Tools/Mesh Combiner")]
    static void Init()
    {
        MeshCombinerTool window = GetWindow<MeshCombinerTool>();
        window.titleContent = new GUIContent("Mesh Combiner");
        window.minSize = new Vector2(350, 200);
        window.Show();
    }

    List<GameObject> selectedObjects = new List<GameObject>();
    bool saveAsAsset = true;
    string savePath = "Assets/CombinedMeshes";
    string meshName = "CombinedMesh";
    bool keepOriginal = true;
    bool combineMaterials = true;
    bool generateLightmapUVs = true;

    void OnGUI()
    {
        GUILayout.Label("Mesh Combiner Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Object selection area
        EditorGUILayout.LabelField("Select Objects to Combine:");
        if (GUILayout.Button("Add Selected Objects"))
        {
            AddSelectedObjects();
        }

        // Display selected objects
        EditorGUILayout.LabelField($"Selected Objects: {selectedObjects.Count}");
        if (selectedObjects.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var obj in selectedObjects)
            {
                EditorGUILayout.LabelField(obj.name);
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Clear Selection"))
            {
                selectedObjects.Clear();
            }
        }

        EditorGUILayout.Space();

        // Options
        keepOriginal = EditorGUILayout.Toggle("Keep Original Objects", keepOriginal);
        combineMaterials = EditorGUILayout.Toggle("Combine Materials", combineMaterials);
        generateLightmapUVs = EditorGUILayout.Toggle("Generate Lightmap UVs", generateLightmapUVs);
        saveAsAsset = EditorGUILayout.Toggle("Save as Asset", saveAsAsset);


        if (saveAsAsset)
        {
            meshName = EditorGUILayout.TextField("Mesh Name", meshName);

            EditorGUILayout.BeginHorizontal();
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string newPath = EditorUtility.SaveFolderPanel("Select Save Folder", savePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    savePath = "Assets" + newPath.Replace(Application.dataPath, "");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // Combine button
        GUI.enabled = selectedObjects.Count > 0;
        if (GUILayout.Button("Combine Meshes", GUILayout.Height(30)))
        {
            CombineMeshes();
        }
        GUI.enabled = true;
    }

    void AddSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.GetComponent<MeshFilter>() != null && !selectedObjects.Contains(obj))
            {
                selectedObjects.Add(obj);
            }
        }
    }

    void CombineMeshes()
    {
        if (selectedObjects.Count == 0)
        {
            Debug.LogWarning("No objects selected to combine!");
            return;
        }

        // Create parent object
        GameObject combinedObject = new GameObject("CombinedMeshObject");
        Undo.RegisterCreatedObjectUndo(combinedObject, "Create Combined Mesh Object");

        // Get all mesh filters
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        foreach (var obj in selectedObjects)
        {
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                meshFilters.Add(mf);
            }
        }

        if (meshFilters.Count == 0)
        {
            Debug.LogWarning("No valid meshes found in selected objects!");
            DestroyImmediate(combinedObject);
            return;
        }

        // Prepare combine instances with all available attributes
        List<CombineInstance> combine = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        List<Vector2[]> uvLists = new List<Vector2[]>();
        List<Vector2[]> uv2Lists = new List<Vector2[]>();
        List<Vector2[]> uv3Lists = new List<Vector2[]>();
        List<Vector2[]> uv4Lists = new List<Vector2[]>();
        List<Color[]> colorLists = new List<Color[]>();
        List<Vector4[]> tangentLists = new List<Vector4[]>();

        bool hasUV = false;
        bool hasUV2 = false;
        bool hasUV3 = false;
        bool hasUV4 = false;
        bool hasColors = false;
        bool hasTangents = false;

        foreach (var mf in meshFilters)
        {
            Mesh mesh = mf.sharedMesh;

            // Check which attributes exist
            if (mesh.uv.Length > 0) hasUV = true;
            if (mesh.uv2.Length > 0) hasUV2 = true;
            if (mesh.uv3.Length > 0) hasUV3 = true;
            if (mesh.uv4.Length > 0) hasUV4 = true;
            if (mesh.colors.Length > 0) hasColors = true;
            if (mesh.tangents.Length > 0) hasTangents = true;
        }

        foreach (var mf in meshFilters)
        {
            Mesh mesh = mf.sharedMesh;
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combine.Add(ci);

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                materials.AddRange(mr.sharedMaterials); // Changed to AddRange to handle multi-material objects
            }

            // Store attribute data
            if (hasUV) uvLists.Add(mesh.uv);
            if (hasUV2) uv2Lists.Add(mesh.uv2);
            if (hasUV3) uv3Lists.Add(mesh.uv3);
            if (hasUV4) uv4Lists.Add(mesh.uv4);
            if (hasColors) colorLists.Add(mesh.colors);
            if (hasTangents) tangentLists.Add(mesh.tangents);
        }

        // Create combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = meshName;

        if (combineMaterials)
        {
            // Merge into a single submesh & single material
            combinedMesh.CombineMeshes(combine.ToArray(), true); // "true" merges submeshes
        }
        else
        {
            // Keep submeshes & materials separate
            combinedMesh.CombineMeshes(combine.ToArray(), false);
        }

        // Generate lightmap UVs if requested
        if (generateLightmapUVs)
        {
            Unwrapping.GenerateSecondaryUVSet(combinedMesh);
        }

        // Reapply UV and other attributes if they existed in any source mesh
        if (hasUV) combinedMesh.uv = CombineArrays(uvLists);
        if (hasUV2) combinedMesh.uv2 = CombineArrays(uv2Lists);
        if (hasUV3) combinedMesh.uv3 = CombineArrays(uv3Lists);
        if (hasUV4) combinedMesh.uv4 = CombineArrays(uv4Lists);
        if (hasColors) combinedMesh.colors = CombineArrays(colorLists);
        if (hasTangents) combinedMesh.tangents = CombineArrays(tangentLists);

        // Set up combined object
        MeshFilter combinedMF = combinedObject.AddComponent<MeshFilter>();
        combinedMF.sharedMesh = combinedMesh;

        MeshRenderer combinedMR = combinedObject.AddComponent<MeshRenderer>();

        if (combineMaterials)
        {
            combinedMR.sharedMaterial = materials.Count > 0 ? materials[0] : null;
        }
        else
        {
            combinedMR.sharedMaterials = materials.ToArray();
        }

        // Position at center of selection
        Vector3 center = Vector3.zero;
        foreach (var obj in selectedObjects)
        {
            center += obj.transform.position;
        }
        center /= selectedObjects.Count;
        combinedObject.transform.position = center;

        // Save as asset if requested
        if (saveAsAsset)
        {
            SaveMeshAsAsset(combinedMesh);
        }

        // Delete original objects if requested
        if (!keepOriginal)
        {
            foreach (var obj in selectedObjects)
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }

        Debug.Log($"Successfully combined {meshFilters.Count} meshes!");
    }

    void SaveMeshAsAsset(Mesh mesh)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string assetPath = Path.Combine(savePath, meshName + ".asset");
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        Mesh meshToSave = Instantiate(mesh);
        AssetDatabase.CreateAsset(meshToSave, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Mesh saved as: {assetPath}");
        EditorUtility.FocusProjectWindow();
    }

    T[] CombineArrays<T>(List<T[]> arrays)
    {
        int totalLength = 0;
        foreach (var array in arrays)
        {
            totalLength += array.Length;
        }

        T[] combined = new T[totalLength];
        int offset = 0;
        foreach (var array in arrays)
        {
            System.Array.Copy(array, 0, combined, offset, array.Length);
            offset += array.Length;
        }
        return combined;
    }
}