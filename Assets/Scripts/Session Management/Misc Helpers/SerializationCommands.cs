using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SerializationCommands : MonoBehaviour
{
    // https://medium.com/@defuncart/json-serialization-in-unity-9420abbce30b

    [Serializable]
    public class PlayerSessionDataWrapperForJson
    {
        public List<PlayerSessionData> array;
    }
    public static string PlayerDataToJson(List<PlayerSessionData> array)
    {
        PlayerSessionDataWrapperForJson p = new() { array = array };
        return JsonUtility.ToJson(p);
    }

    public static List<PlayerSessionData> PlayerDataFromJson(string json)
    {
        PlayerSessionDataWrapperForJson p = JsonUtility.FromJson<PlayerSessionDataWrapperForJson>(json);
        return p.array;
    }

    public static byte[] ToBytes(System.Object obj)
    {
        if (obj == null) return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    public static System.Object ToObject(byte[] bytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(bytes, 0, bytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        System.Object obj = (System.Object)binForm.Deserialize(memStream);

        return obj;
    }
}


