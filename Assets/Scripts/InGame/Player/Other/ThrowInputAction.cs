using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
[InitializeOnLoad]
#else
[RuntimeInitializeOnLoadMethod]
#endif
public class ThrowInputAction : IInputInteraction
{
    [SerializeField] private float duration = 0.5f; // Timeout time

    static ThrowInputAction()
    {
        InputSystem.RegisterInteraction<ThrowInputAction>();
    }

    public void Process(ref InputInteractionContext context)
    {
        // Button has been pressed for longer than duration, initiate throw
        if (context.timerHasExpired)
        {
            context.Started();
            return;
        }

        // Begin press and hold
        if(context.ControlIsActuated())
        {
            context.SetTimeout(duration);
        } else // Released
        {
            // Too soon
            if (!context.isStarted)
            {
                context.Canceled();
            } else // After duration
            {
                context.Performed();
            }
        }

    }

    public void Reset() { }
}