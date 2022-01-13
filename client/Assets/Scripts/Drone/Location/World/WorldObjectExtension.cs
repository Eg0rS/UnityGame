using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World
{
    public static class WorldObjectExtension
    {
        [NotNull]
        public static List<T> GetObjectComponents<T>(GameObject gameObject)
        {
            return GetSceneObjects(gameObject).Where(o => o.GetComponent<T>() != null).Select(go => go.GetComponent<T>()).ToList();
        }

        [NotNull]
        private static List<GameObject> GetSceneObjects(GameObject gameObject)
        {
            return gameObject.GetComponentsOnlyInChildren<Transform>(true).ToList().Select(t => t.gameObject).ToList();
        }
    }
}