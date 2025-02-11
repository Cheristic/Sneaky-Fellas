//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Resources/InGame/Player/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputAction: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""MainPlayer"",
            ""id"": ""f1a27255-fa4d-4166-93aa-a78791a5f62f"",
            ""actions"": [
                {
                    ""name"": ""UsePrimary"",
                    ""type"": ""Button"",
                    ""id"": ""7f362412-0e96-41fa-b8da-c1af9141c219"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap(duration=0.5)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UseSecondary"",
                    ""type"": ""Button"",
                    ""id"": ""eefb6390-2723-4b48-b7f8-e365314fe615"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap(duration=0.5)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrowPrimary"",
                    ""type"": ""Button"",
                    ""id"": ""23c53aac-aec3-453c-9e9d-b2aec32c60b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""ThrowInputAction"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrowSecondary"",
                    ""type"": ""Button"",
                    ""id"": ""f78d14a3-459e-468b-9bc5-df6cd42fa2db"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""ThrowInputAction"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b644fdb3-0724-44e0-af4f-f0947e419a09"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c23a5941-e6d6-4cb5-be56-10f3d52a3453"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UsePrimary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7722b961-27ce-4b55-92d3-742912300f9c"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UsePrimary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c10a02d0-cdd3-4ae1-b651-b6ebc8e282f4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseSecondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""70b01479-00a3-4003-8ff1-b55fc9f57f83"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseSecondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""bd0904b3-9894-4713-bd14-ae67b0fd954c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""746eacf9-c3cb-45f0-a15b-f16e4c526cd1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""090cfd71-f80d-49b6-b6fe-4c043f46b8b2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6f38dc7e-9d5a-4fd8-9315-c20c1190ec36"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""93f89b49-2229-4736-91ea-0e0b1f7f2ae7"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""dd8be3d1-2a3e-4567-ba86-64e1caffb1cb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowSecondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""81d1f7fa-1a61-4eb9-a44d-3349583f26f7"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowSecondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a2ddb8e0-36ed-454a-bc94-b3b13d34d15b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowPrimary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""394174e6-dc8f-46c6-b25b-478e5c9252df"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowPrimary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MainPlayer
        m_MainPlayer = asset.FindActionMap("MainPlayer", throwIfNotFound: true);
        m_MainPlayer_UsePrimary = m_MainPlayer.FindAction("UsePrimary", throwIfNotFound: true);
        m_MainPlayer_UseSecondary = m_MainPlayer.FindAction("UseSecondary", throwIfNotFound: true);
        m_MainPlayer_ThrowPrimary = m_MainPlayer.FindAction("ThrowPrimary", throwIfNotFound: true);
        m_MainPlayer_ThrowSecondary = m_MainPlayer.FindAction("ThrowSecondary", throwIfNotFound: true);
        m_MainPlayer_Move = m_MainPlayer.FindAction("Move", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // MainPlayer
    private readonly InputActionMap m_MainPlayer;
    private List<IMainPlayerActions> m_MainPlayerActionsCallbackInterfaces = new List<IMainPlayerActions>();
    private readonly InputAction m_MainPlayer_UsePrimary;
    private readonly InputAction m_MainPlayer_UseSecondary;
    private readonly InputAction m_MainPlayer_ThrowPrimary;
    private readonly InputAction m_MainPlayer_ThrowSecondary;
    private readonly InputAction m_MainPlayer_Move;
    public struct MainPlayerActions
    {
        private @PlayerInputAction m_Wrapper;
        public MainPlayerActions(@PlayerInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @UsePrimary => m_Wrapper.m_MainPlayer_UsePrimary;
        public InputAction @UseSecondary => m_Wrapper.m_MainPlayer_UseSecondary;
        public InputAction @ThrowPrimary => m_Wrapper.m_MainPlayer_ThrowPrimary;
        public InputAction @ThrowSecondary => m_Wrapper.m_MainPlayer_ThrowSecondary;
        public InputAction @Move => m_Wrapper.m_MainPlayer_Move;
        public InputActionMap Get() { return m_Wrapper.m_MainPlayer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainPlayerActions set) { return set.Get(); }
        public void AddCallbacks(IMainPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_MainPlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MainPlayerActionsCallbackInterfaces.Add(instance);
            @UsePrimary.started += instance.OnUsePrimary;
            @UsePrimary.performed += instance.OnUsePrimary;
            @UsePrimary.canceled += instance.OnUsePrimary;
            @UseSecondary.started += instance.OnUseSecondary;
            @UseSecondary.performed += instance.OnUseSecondary;
            @UseSecondary.canceled += instance.OnUseSecondary;
            @ThrowPrimary.started += instance.OnThrowPrimary;
            @ThrowPrimary.performed += instance.OnThrowPrimary;
            @ThrowPrimary.canceled += instance.OnThrowPrimary;
            @ThrowSecondary.started += instance.OnThrowSecondary;
            @ThrowSecondary.performed += instance.OnThrowSecondary;
            @ThrowSecondary.canceled += instance.OnThrowSecondary;
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
        }

        private void UnregisterCallbacks(IMainPlayerActions instance)
        {
            @UsePrimary.started -= instance.OnUsePrimary;
            @UsePrimary.performed -= instance.OnUsePrimary;
            @UsePrimary.canceled -= instance.OnUsePrimary;
            @UseSecondary.started -= instance.OnUseSecondary;
            @UseSecondary.performed -= instance.OnUseSecondary;
            @UseSecondary.canceled -= instance.OnUseSecondary;
            @ThrowPrimary.started -= instance.OnThrowPrimary;
            @ThrowPrimary.performed -= instance.OnThrowPrimary;
            @ThrowPrimary.canceled -= instance.OnThrowPrimary;
            @ThrowSecondary.started -= instance.OnThrowSecondary;
            @ThrowSecondary.performed -= instance.OnThrowSecondary;
            @ThrowSecondary.canceled -= instance.OnThrowSecondary;
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
        }

        public void RemoveCallbacks(IMainPlayerActions instance)
        {
            if (m_Wrapper.m_MainPlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMainPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_MainPlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MainPlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MainPlayerActions @MainPlayer => new MainPlayerActions(this);
    public interface IMainPlayerActions
    {
        void OnUsePrimary(InputAction.CallbackContext context);
        void OnUseSecondary(InputAction.CallbackContext context);
        void OnThrowPrimary(InputAction.CallbackContext context);
        void OnThrowSecondary(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
    }
}
