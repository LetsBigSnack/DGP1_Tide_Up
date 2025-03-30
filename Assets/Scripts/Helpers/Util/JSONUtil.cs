using System;
using System.IO;
using Data;
using UnityEngine;
using Newtonsoft.Json;

namespace Helpers.Util
{
    public class JSONUtil
    {
        public static DialogJsonData GetDialogueData()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Dialogue.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Dialogue.json not found");
            }
            
            string json = File.ReadAllText(path);
            DialogJsonData jsonData = JsonConvert.DeserializeObject<DialogJsonData>(json);
            return jsonData;
        }
    }
}