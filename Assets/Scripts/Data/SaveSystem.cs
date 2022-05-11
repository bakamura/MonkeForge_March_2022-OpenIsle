﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {

    public static void SaveProgress() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/progress.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(PlayerData.Instance);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static SaveData LoadProgress() {
        string path = Application.persistentDataPath + "/progress.data";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else {
            Debug.Log("Save file could'nt be found in" + path);
            return null;
        }
    }

    public static void EraseProgress() {
        string path = Application.persistentDataPath + "/progress.data";
        if (File.Exists(path)) File.Delete(path);
        else Debug.Log("Couldn't find anything in" + path);
    }
}
