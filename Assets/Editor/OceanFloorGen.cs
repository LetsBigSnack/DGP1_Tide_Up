using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(OceanFloorTile))]
public class OceanFloorTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OceanFloorTile tile = (OceanFloorTile)target;

        if (GUILayout.Button("Generate & Save Mesh"))
        {
            Mesh mesh = tile.CreateMesh();

            // Ensure directory exists
            string directory = "Assets/GeneratedMeshes";
            if (!AssetDatabase.IsValidFolder(directory))
                AssetDatabase.CreateFolder("Assets", "GeneratedMeshes");

            // Generate unique mesh path
            string meshPath = Path.Combine(directory, $"{tile.gameObject.name}_Mesh.asset");
            AssetDatabase.CreateAsset(mesh, meshPath);
            AssetDatabase.SaveAssets();

            // Assign the saved mesh
            MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            EditorUtility.SetDirty(tile);
            Debug.Log($"Mesh saved successfully at {meshPath}");
        }
    }
}