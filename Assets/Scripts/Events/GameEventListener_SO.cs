using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class GameEventListener_SO : MonoBehaviour
{
    [SerializeField] private GameEvent_SO GameEvent_SO;

    public CustomGameEvent response;

    private void OnEnable()
    {
        GameEvent_SO.RegisterListener(this);
    }

    private void OnDisable()
    {
        GameEvent_SO.UnregisterListener(this);
    }

    public void OnEventRaised(Component sender, object data)
    {
        response.Invoke(sender, data);
    }
}
