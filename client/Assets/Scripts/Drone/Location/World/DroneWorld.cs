using System;
using System.Collections.Generic;
using System.Linq;
using Adept.Logger;
using Drone.Location.Event;
using Drone.Location.Model.BaseModel;
using Drone.Location.World.Drone;
using GameKit.World;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World
{
    public class DroneWorld : GameWorld
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DroneWorld>();
        private Dictionary<string, PrefabModel> _controllers;
        private DroneController _droneController;
        private float _currentTimeScale;

        [PublicAPI]
        public void Pause()
        {
            _currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
            Dispatch(new WorldEvent(WorldEvent.PAUSED));
        }

        [PublicAPI]
        public void Resume()
        {
            Time.timeScale = _currentTimeScale;
            Dispatch(new WorldEvent(WorldEvent.UNPAUSED));
        }

        [NotNull]
        public DroneController Drone
        {
            get
            {
                if (_droneController == null) {
                    _droneController = FindComponent<DroneController>();
                }
                return _droneController;
            }
        }

        private Dictionary<string, PrefabModel> Controllers
        {
            get
            {
                if (_controllers != null) {
                    return _controllers;
                }
                _controllers = new Dictionary<string, PrefabModel>();
                foreach (PrefabModel controller in GetObjectComponents<PrefabModel>()) {
                    _controllers.Add(controller.name, controller);
                }
                return _controllers;
            }
        }

        [NotNull]
        public List<T> GetControllers<T>()
        {
            return Controllers.Select(controller => controller.Value.GetComponent<T>()).Where(controller => controller != null).ToList();
        }

        public void RemoveController(string controllerName)
        {
            if (!_controllers.ContainsKey(controllerName)) {
                return;
            }
            _controllers.Remove(controllerName);
        }

        public GameObject GetObjectByName(string objectName)
        {
            return GetObjectByPrefabName(GetSceneObjects(), objectName);
        }

        [CanBeNull]
        private GameObject GetObjectByPrefabName(List<GameObject> gameObjects, string objectName)
        {
            foreach (GameObject sceneObject in gameObjects) {
                if (sceneObject.name.Replace("(Clone)", "") == objectName) {
                    return sceneObject;
                }
                if (sceneObject.gameObject.name.LastIndexOf("-", StringComparison.Ordinal) == -1) {
                    continue;
                }
                string replaceName = GetGameObjectNameWithOutSalt(sceneObject.gameObject.name).Replace("(Clone)", " ");
                if (replaceName == objectName) {
                    return sceneObject;
                }
            }
            return null;
        }

        [PublicAPI]
        public string GetGameObjectNameWithOutSalt(string gameObjectName)
        {
            int position = gameObjectName.LastIndexOf("-", StringComparison.Ordinal);
            return position == -1 ? gameObjectName : gameObjectName.Substring(0, position);
        }
        
        public void OnDestroy()
        {
            _logger.Debug("Destroy HorrorWorld");
            Dispatch(new WorldEvent(WorldEvent.DESTROYED));
        }

        public bool WorldEnabled
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
                Dispatch(new WorldEvent(WorldEvent.ENABLED, value));
            }
        }
    }
}