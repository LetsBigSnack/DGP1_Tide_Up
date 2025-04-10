using UnityEngine;
using UnityEditor;
using Data;

[CustomEditor(typeof(UpcycleTester))]
public class UpcycleTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpcycleTester tester = (UpcycleTester)target;

        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add Material to Upcycler", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Plastic"))
                tester.AddMaterial(TrashMaterialType.Plastic);

            if (GUILayout.Button("Add Metal"))
                tester.AddMaterial(TrashMaterialType.Metal);

            if (GUILayout.Button("Add Glass"))
                tester.AddMaterial(TrashMaterialType.Glass);
        
            if (GUILayout.Button("Add Paper"))
                tester.AddMaterial(TrashMaterialType.Paper);
            
            if (GUILayout.Button("Add Wood"))
                tester.AddMaterial(TrashMaterialType.Wood);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Upcycle Action", EditorStyles.boldLabel);

            if (GUILayout.Button("Upcycle Now"))
                tester.Upcycle();
            
            if (GUILayout.Button("Remove All"))
                tester.RemoveAll();
        }
    }
}