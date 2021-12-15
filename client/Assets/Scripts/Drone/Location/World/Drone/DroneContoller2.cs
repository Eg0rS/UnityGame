using Drone.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service.Control.Drone
{
    public class DroneContoller2 : MonoBehaviour
    {
        [Inject]
        private GameWorld _gameWorld;
    }
}