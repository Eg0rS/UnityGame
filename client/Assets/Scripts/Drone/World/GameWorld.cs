using System;
using System.Collections.Generic;
using System.Linq;
using AgkCommons.Event;
using AgkCommons.Extension;
using Cinemachine;
using Drone.Location.World.Drone.Model;
using Drone.World.Event;
using JetBrains.Annotations;
using UnityEngine;
using static CsPreconditions.Preconditions;

namespace Drone.World
{
    [PublicAPI]
    public class GameWorld : GameEventDispatcher
    {
        [PublicAPI]
        public string WorldId { get; private set; }

        private string _drone = "DroneCube";

        public void CreateWorld(string worldId)
        {
            WorldId = worldId;
        }

        public void AddGameObject(GameObject go, [CanBeNull] GameObject container = null, bool worldPositionStays = false)
        {
            Transform parentContainer = container == null ? transform : container.transform;

            go.transform.SetParent(parentContainer, worldPositionStays);

            go.GetOrCreateComponent<GameEventDispatcher>().Dispatch(new WorldEvent(WorldEvent.ADDED, go));
        }

        public void RemoveGameObject(string id)
        {
            GameObject go = GetGameObjectByName(id);
            if (go != null) {
                DestroyImmediate(go);
            }
        }

        [CanBeNull]
        public GameObject GetDroneCube()
        {
            return GetGameObjectByName(_drone);
        }

        [CanBeNull]
        public CinemachineVirtualCamera GetDroneCamera()
        {
            return GetGameObjectByName("CM vcam1")?.GetComponent<CinemachineVirtualCamera>();
        }

        [CanBeNull]
        public Animator GetDroneAnimator()
        {
            return GetGameObjectByName("pfDroneModelContainer")?.GetComponentInChildren<Animator>();
        }

        #region GameObject control API

        [CanBeNull]
        public GameObject GetGameObjectByName(string objectName)
        {
            return GetSceneObjects().FirstOrDefault(o => o.name == objectName);
        }

        public GameObject RequireGameObjectByName(string objectName)
        {
            return GetSceneObjects().First(o => o.name == objectName);
        }

        [NotNull]
        public List<GameObject> GetSceneObjects()
        {
            return gameObject.GetComponentsOnlyInChildren<Transform>(true).ToList().Select(t => t.gameObject).ToList();
        }

        [CanBeNull]
        public T FindComponent<T>()
        {
            foreach (GameObject sceneObject in GetSceneObjects()) {
                T component = sceneObject.GetComponentInChildren<T>();
                if (component != null) {
                    return component;
                }
            }
            return default;
        }

        [Obsolete]
        public T RequireObjectComponent<T>()
        {
            foreach (GameObject sceneObject in GetSceneObjects()) {
                T component = sceneObject.GetComponentInChildren<T>();
                if (component != null) {
                    return component;
                }
            }
            throw new NullReferenceException();
        }

        public List<T> GetObjectComponents<T>()
        {
            return GetSceneObjects().Where(go => go.GetComponent<T>() != null).Select(go => go.GetComponent<T>()).ToList();
        }

        public T RequireObjectComponent<T>(string objectName)
                where T : Component
        {
            return (T) CheckNotNull(GetObjectComponent(typeof(T), objectName));
        }

        [CanBeNull]
        public T GetObjectComponent<T>(string objectName)
                where T : Component
        {
            return (T) GetObjectComponent(typeof(T), objectName);
        }

        [CanBeNull]
        public Component GetObjectComponent(Type type, string objectName)
        {
            GameObject childObject = GetGameObjectByName(objectName);
            if (childObject == null) {
                return default;
            }
            Component component = childObject.GetComponent(type);
            return component != null ? component : default;
        }

        #endregion
    }
}