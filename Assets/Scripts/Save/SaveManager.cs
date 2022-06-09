using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static void SaveData<T>(T data)
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/" + typeof(T) + ".data";
        var stream = new FileStream(path, FileMode.Create);
        
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static T LoadData<T>() where T : new()
    {
        var path = Application.persistentDataPath + "/" + typeof(T) + ".data";

        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found in " + path);
            return new T();
        }

        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Open);

        var data = (T)formatter.Deserialize(stream);
        stream.Close();
        
        return data;
    }
}