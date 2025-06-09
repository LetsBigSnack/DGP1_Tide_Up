using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class CreateBounds : EditorWindow
{
    private float expandAmount = 10f;
    private string subfolderName = "ExpandedMeshes";

    [MenuItem("Tools/Expand Mesh Bounds in Scene")]
    public static void ShowWindow()
    {
        GetWindow<CreateBounds>("Expand Mesh Bounds");
    }

    void OnGUI()
    {
        GUILayout.Label("Mesh Bounds Expander", EditorStyles.boldLabel);
        expandAmount = EditorGUILayout.FloatField("Expansion Amount", expandAmount);
        subfolderName = EditorGUILayout.TextField("Subfolder (in Assets/3D/)", subfolderName);

        if (GUILayout.Button("Expand All Mesh Bounds in Scene"))
        {
            ExpandAllBounds(expandAmount, subfolderName);
        }
    }

    private static void ExpandAllBounds(float amount, string subfolder)
    {
        string baseFolder = $"Assets/3D/{subfolder}";
        if (!AssetDatabase.IsValidFolder(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
            AssetDatabase.Refresh();
        }

        int count = 0;

        foreach (var mf in GameObject.FindObjectsOfType<MeshFilter>())
        {
            Mesh sharedMesh = mf.sharedMesh;
            if (sharedMesh == null) continue;

            string path = $"{baseFolder}/{sharedMesh.name}_Expanded_{amount}.asset";

            Mesh expandedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (expandedMesh == null)
            {
                expandedMesh = Instantiate(sharedMesh);
                Bounds bounds = expandedMesh.bounds;
                bounds.Expand(Vector3.one * amount);
                expandedMesh.bounds = bounds;

                AssetDatabase.CreateAsset(expandedMesh, path);
                AssetDatabase.SaveAssets();
                Debug.Log($"Created expanded mesh: {path}");
            }

            mf.sharedMesh = expandedMesh;
            count++;
        }
    }
}
#endif