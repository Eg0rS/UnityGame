using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using UnityEngine;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour
    {
        public void SpawnObstacles(ref Dictionary<GameObject, int> obstacles)
        {
            List<GameObject> spawnPlaces = gameObject.GetChildren();

            foreach (GameObject place in spawnPlaces) {
                int min = obstacles.Min(x => x.Value);
                GameObject go = obstacles.First(x => x.Value == min).Key;
                obstacles[go]++;
                Instantiate(go, place.transform);
            }
        }
    }
}