using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LowPolyHnS
{
    public static class SaveLoadSystem
    {
        public static void SaveGame(SaveFile save, string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create($"{Application.persistentDataPath}/{fileName}");
            formatter.Serialize(file, save);
            file.Close();
        }

        public static SaveFile GetSavedFile(string fileName)
        {
            if (!File.Exists(Application.persistentDataPath + "/" + fileName))
            {
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{fileName}", FileMode.Open);
            SaveFile retFile = (SaveFile) formatter.Deserialize(file);
            file.Close();

            return retFile;
        }
    }
}