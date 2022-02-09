using System;
using System.Collections.Generic;
using System.Linq;
using Adept.Logger;
using Drone.Location.Event;
using Drone.Location.Model.BaseModel;
using Drone.Location.World.Player;
using GameKit.World;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World
{
    public class DroneWorld : GameWorld
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DroneWorld>();
        private Dictionary<string, PrefabModel> _controllers;
        private PlayerController _playerController;
        private float _currentTimeScale;
        private bool _isPauseWorld = false;
        private Dictionary<string, GameObject> _loadedCache = new Dictionary<string, GameObject>();

        [PublicAPI]
        public void Pause()
        {
            _isPauseWorld = true;
            _currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
            Dispatch(new WorldEvent(WorldEvent.PAUSED));
        }

        [PublicAPI]
        public void Resume()
        {
            _isPauseWorld = false;
            Time.timeScale = _currentTimeScale;
            Dispatch(new WorldEvent(WorldEvent.UNPAUSED));
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

        [NotNull]
        public PlayerController Player
        {
            get
            {
                if (_playerController == null) {
                    _playerController = FindComponent<PlayerController>();
                }
                return _playerController;
            }
        }
        public bool IsPauseWorld
        {
            get { return _isPauseWorld; }
        }
    }
}