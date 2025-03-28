using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Test
{
    public int test;
}


namespace Helpers.Util
{
    public class JSONUtil
    {
        public static Test GetDialogueData()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Dialogue.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Dialogue.json not found");
            }
            
            string json = File.ReadAllText(path);
            Test jsonData = JsonConvert.DeserializeObject<Test>(json);
            return jsonData;
        }
    }
}