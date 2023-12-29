using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent_SO", menuName = "Scriptable Object/Game Event")]
public class GameEvent_SO : ScriptableObject
{
    public List<GameEventListener_SO> listeners = new List<GameEventListener_SO>();

    // Raise this game event object
    public void Raise(Component sender, object data)
    {
        foreach (GameEventListener_SO listener in listeners)
        {
            listener.OnEventRaised(sender, data);
        }
    }

    // Add/Remove Listeners
    public void RegisterListener(GameEventListener_SO listener)
    {
        if (!listeners.Contains(listener)) listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener_SO listener)
    {
        if (listeners.Contains(listener)) listeners.Remove(listener);
    }
}
