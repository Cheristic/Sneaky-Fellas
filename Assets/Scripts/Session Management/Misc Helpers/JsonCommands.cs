using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class JsonCommands : MonoBehaviour
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
}


