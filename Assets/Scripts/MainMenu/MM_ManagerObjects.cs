using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_ManagerObjects : MonoBehaviour
{
    public static MM_ManagerObjects Instance { get; private set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
