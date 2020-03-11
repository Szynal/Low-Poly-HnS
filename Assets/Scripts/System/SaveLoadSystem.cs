using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LowPolyHnS
{
    public static class SaveLoadSystem
    {
        public static void SaveGame(FE_SaveFile save, string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create($"{Application.persistentDataPath}/{fileName}");
            formatter.Serialize(file, save);
            file.Close();
        }

        public static FE_SaveFile GetSavedFile(string fileName)
        {
            if (!File.Exists(Application.persistentDataPath + "/" + fileName))
            {
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{fileName}", FileMode.Open);
            FE_SaveFile retFile = (FE_SaveFile) formatter.Deserialize(file);
            file.Close();

            return retFile;
        }
    }
}