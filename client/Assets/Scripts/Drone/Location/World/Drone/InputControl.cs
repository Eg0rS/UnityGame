//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Scripts/Drone/Location/World/Drone/InputControl.inputactions
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

public partial class @InputControl : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControl()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControl"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f88bde9c-f6e6-4c0a-befa-8f6db052881f"",
            ""actions"": [
                {
                    ""name"": ""Touch"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1576babe-0ffa-41b8-8fd6-f8e8f7424902"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""touch1"",
                    ""type"": ""Button"",
                    ""id"": ""5739546c-e85e-435f-91af-e437fcbb853a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""touch2"",
                    ""type"": ""Value"",
                    ""id"": ""f5cf2f0e-8606-42ec-9041-3424f467f06c"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""touch3"",
                    ""type"": ""Value"",
                    ""id"": ""77d32012-979d-462b-bc8a-c892d657cce1"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d122b5b2-2a07-40a8-9c6e-3b1b34d741cd"",
                    ""path"": ""<Touchscreen>/primaryTouch"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gesture"",
                    ""action"": ""Touch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""048e9013-62da-4fa2-b723-67cd832f4612"",
                    ""path"": ""<Touchscreen>/primaryTouch/tap"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""touch1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""049932af-82b1-498e-89ec-f88f27bf5a8c"",
                    ""path"": ""<Touchscreen>/primaryTouch"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""touch2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f2e7cee-22df-48ea-bc85-7b8e14b7b53e"",
                    ""path"": ""<Touchscreen>/primaryTouch"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""touch3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gesture"",
            ""bindingGroup"": ""Gesture"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Touch = m_Player.FindAction("Touch", throwIfNotFound: true);
        m_Player_touch1 = m_Player.FindAction("touch1", throwIfNotFound: true);
        m_Player_touch2 = m_Player.FindAction("touch2", throwIfNotFound: true);
        m_Player_touch3 = m_Player.FindAction("touch3", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Touch;
    private readonly InputAction m_Player_touch1;
    private readonly InputAction m_Player_touch2;
    private readonly InputAction m_Player_touch3;
    public struct PlayerActions
    {
        private @InputControl m_Wrapper;
        public PlayerActions(@InputControl wrapper) { m_Wrapper = wrapper; }
        public InputAction @Touch => m_Wrapper.m_Player_Touch;
        public InputAction @touch1 => m_Wrapper.m_Player_touch1;
        public InputAction @touch2 => m_Wrapper.m_Player_touch2;
        public InputAction @touch3 => m_Wrapper.m_Player_touch3;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Touch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch;
                @Touch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch;
                @Touch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch;
                @touch1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch1;
                @touch1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch1;
                @touch1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch1;
                @touch2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch2;
                @touch2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch2;
                @touch2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch2;
                @touch3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch3;
                @touch3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch3;
                @touch3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouch3;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Touch.started += instance.OnTouch;
                @Touch.performed += instance.OnTouch;
                @Touch.canceled += instance.OnTouch;
                @touch1.started += instance.OnTouch1;
                @touch1.performed += instance.OnTouch1;
                @touch1.canceled += instance.OnTouch1;
                @touch2.started += instance.OnTouch2;
                @touch2.performed += instance.OnTouch2;
                @touch2.canceled += instance.OnTouch2;
                @touch3.started += instance.OnTouch3;
                @touch3.performed += instance.OnTouch3;
                @touch3.canceled += instance.OnTouch3;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_GestureSchemeIndex = -1;
    public InputControlScheme GestureScheme
    {
        get
        {
            if (m_GestureSchemeIndex == -1) m_GestureSchemeIndex = asset.FindControlSchemeIndex("Gesture");
            return asset.controlSchemes[m_GestureSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnTouch(InputAction.CallbackContext context);
        void OnTouch1(InputAction.CallbackContext context);
        void OnTouch2(InputAction.CallbackContext context);
        void OnTouch3(InputAction.CallbackContext context);
    }
}
