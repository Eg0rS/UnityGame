// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Drone/Location/World/Drone/InputControl.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputControl : IInputActionCollection, IDisposable
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
                    ""type"": ""Value"",
                    ""id"": ""1576babe-0ffa-41b8-8fd6-f8e8f7424902"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Gesture"",
                    ""type"": ""Button"",
                    ""id"": ""291faeab-efeb-4fd3-ad82-fce356aa4b46"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GestureStartPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d3123e77-21fd-4739-87dd-3288c4e6b77e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GesturePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e1c9ec61-81e1-4e2d-a2ab-fe01662013e6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GestureTime"",
                    ""type"": ""PassThrough"",
                    ""id"": ""98234dc0-879e-49cc-9172-bc9b65b01673"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bb3747ed-838e-418c-9f0c-5e02e4b55d20"",
                    ""path"": ""<Touchscreen>/touch0/startPosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gesture"",
                    ""action"": ""GestureStartPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a995f488-5bd6-474b-a84b-dc7271ef9a35"",
                    ""path"": ""<Touchscreen>/touch0/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GesturePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d122b5b2-2a07-40a8-9c6e-3b1b34d741cd"",
                    ""path"": ""<Touchscreen>/touch0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Touch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""884117ef-2d7e-4fc6-8198-324da04aacdc"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Gesture"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49a75e94-2a91-4e3a-97dd-db1828a9ff9f"",
                    ""path"": ""<Touchscreen>/touch0/startTime"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GestureTime"",
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
        m_Player_Gesture = m_Player.FindAction("Gesture", throwIfNotFound: true);
        m_Player_GestureStartPosition = m_Player.FindAction("GestureStartPosition", throwIfNotFound: true);
        m_Player_GesturePosition = m_Player.FindAction("GesturePosition", throwIfNotFound: true);
        m_Player_GestureTime = m_Player.FindAction("GestureTime", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Touch;
    private readonly InputAction m_Player_Gesture;
    private readonly InputAction m_Player_GestureStartPosition;
    private readonly InputAction m_Player_GesturePosition;
    private readonly InputAction m_Player_GestureTime;
    public struct PlayerActions
    {
        private @InputControl m_Wrapper;
        public PlayerActions(@InputControl wrapper) { m_Wrapper = wrapper; }
        public InputAction @Touch => m_Wrapper.m_Player_Touch;
        public InputAction @Gesture => m_Wrapper.m_Player_Gesture;
        public InputAction @GestureStartPosition => m_Wrapper.m_Player_GestureStartPosition;
        public InputAction @GesturePosition => m_Wrapper.m_Player_GesturePosition;
        public InputAction @GestureTime => m_Wrapper.m_Player_GestureTime;
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
                @Gesture.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGesture;
                @Gesture.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGesture;
                @Gesture.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGesture;
                @GestureStartPosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGestureStartPosition;
                @GestureStartPosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGestureStartPosition;
                @GestureStartPosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGestureStartPosition;
                @GesturePosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGesturePosition;
                @GesturePosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGesturePosition;
                @GesturePosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGesturePosition;
                @GestureTime.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGestureTime;
                @GestureTime.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGestureTime;
                @GestureTime.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGestureTime;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Touch.started += instance.OnTouch;
                @Touch.performed += instance.OnTouch;
                @Touch.canceled += instance.OnTouch;
                @Gesture.started += instance.OnGesture;
                @Gesture.performed += instance.OnGesture;
                @Gesture.canceled += instance.OnGesture;
                @GestureStartPosition.started += instance.OnGestureStartPosition;
                @GestureStartPosition.performed += instance.OnGestureStartPosition;
                @GestureStartPosition.canceled += instance.OnGestureStartPosition;
                @GesturePosition.started += instance.OnGesturePosition;
                @GesturePosition.performed += instance.OnGesturePosition;
                @GesturePosition.canceled += instance.OnGesturePosition;
                @GestureTime.started += instance.OnGestureTime;
                @GestureTime.performed += instance.OnGestureTime;
                @GestureTime.canceled += instance.OnGestureTime;
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
        void OnGesture(InputAction.CallbackContext context);
        void OnGestureStartPosition(InputAction.CallbackContext context);
        void OnGesturePosition(InputAction.CallbackContext context);
        void OnGestureTime(InputAction.CallbackContext context);
    }
}
