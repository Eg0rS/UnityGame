using System;
using Drone.Location.World.Dron;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Drone.Location.Service
{
    public class ControllService : MonoBehaviour
    {
        private DroneMovement _droneMovement;

        private void Awake()
        {
            _droneMovement = new DroneMovement();
        }

        private void OnEnable()
        {
            _droneMovement.Enable();
        }

        private void OnDisable()
        {
            _droneMovement.Disable();
        }

        private void Start()
        {
            _droneMovement.Touch.TouchPress.started += context => StartTouch(context);
            _droneMovement.Touch.TouchPress.canceled += context => EndTouch(context);
            
            
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            Debug.Log("start");
        }
        
        private void EndTouch(InputAction.CallbackContext context)
        {
            Debug.Log("end");
        }

        
    }
}