using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World
{
    public abstract class WorldObjectExtension : MonoBehaviour
    {
        [NotNull]
        public List<T> GetObjectComponents<T>()
        {
            return GetSceneObjects().Where(o => o.GetComponent<T>() != null).Select(go => go.GetComponent<T>()).ToList();
        }

        [NotNull]
        private List<GameObject> GetSceneObjects()
        {
            return gameObject.GetComponentsOnlyInChildren<Transform>(true).ToList().Select(t => t.gameObject).ToList();
        }
    }
}