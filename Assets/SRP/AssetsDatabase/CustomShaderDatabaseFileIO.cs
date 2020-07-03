using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenRT {
    public class CustomShaderDatabaseFileIO : IShaderDatabaseFileIO {
        public const string JSON_FILE_PATH = "Assets/SRP/AssetsDatabase/Resources/RayTracingFramework/CustomShaderDatabase.json";

        public CustomShaderDatabaseFile ReadDatabaseFromFile() {

            if (!File.Exists(JSON_FILE_PATH)) {
                // Return a brand new Custom Shader Database File
                return new CustomShaderDatabaseFile();
            }

            StreamReader reader = new StreamReader(JSON_FILE_PATH);
            var db = JsonConvert.DeserializeObject<CustomShaderDatabaseFile>(reader.ReadToEnd());
            reader.Close();
            return db;
        }

        public void WriteDatabaseToFile(CustomShaderDatabaseFile file) {

            StreamWriter writer = new StreamWriter(JSON_FILE_PATH, false);
            writer.WriteLine(JsonConvert.SerializeObject(file, formatting : Formatting.Indented));
            writer.Close();
        }
    }
}