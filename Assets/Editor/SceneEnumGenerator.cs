using UnityEditor;
using System.IO;

public class SceneEnumGenerator
{
    [MenuItem("Tools/Generate Scene Enum")]
    public static void GenerateSceneEnum()
    {
        string enumName = "Scenes";
        string folderPath = "Assets/Scripts";
        string filePath = Path.Combine(folderPath, enumName + ".cs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var scenes = EditorBuildSettings.scenes;

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("// This file is auto-generated. Do not modify manually.");
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");

            for (int i = 0; i < scenes.Length; i++)
            {
                var scene = scenes[i];
                if (!scene.enabled) continue;

                string sceneName = Path.GetFileNameWithoutExtension(scene.path)
                    .Replace(" ", "")
                    .Replace("-", "_");

                writer.WriteLine($"    {sceneName}" + (i < scenes.Length - 1 ? "," : ""));
            }

            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }
}