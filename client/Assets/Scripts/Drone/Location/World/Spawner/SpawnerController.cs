using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using Drone.Obstacles;
using FluffyUnderware.DevTools.Extensions;
using UnityEngine;
using Random = System.Random;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour
    {
        public void SpawnObstacles(ref Dictionary<ObstacleType, Dictionary<GameObject, int>> allObstacles, List<ObstacleType> obstacleTypesOnTile)
        {
            List<GameObject> spawnPlaces = gameObject.GetChildren();
            Dictionary<GameObject, int> tileObstacles = new Dictionary<GameObject, int>();
            foreach (Dictionary<GameObject, int> dictionary in allObstacles.Where(x => obstacleTypesOnTile.Contains(x.Key)).Select(x => x.Value)) {
                tileObstacles = tileObstacles.Union(dictionary).ToDictionary(x => x.Key, x => x.Value);
            }

            foreach (GameObject place in spawnPlaces) {
                int min = tileObstacles.Min(x => x.Value);
                List<GameObject> gameObjects = tileObstacles.Where(x => x.Value == min).Select(pair => pair.Key).ToList();
                GameObject go = gameObjects[new Random().Next(gameObjects.Count)];
                tileObstacles[go]++;
                Instantiate(go, place.transform);
            }
            allObstacles.Where(x => obstacleTypesOnTile.Contains(x.Key))
                        .Select(x => x.Value)
                        .ForEach(dictionary => tileObstacles.ForEach(matched => {
                            if (dictionary.ContainsKey(matched.Key)) {
                                dictionary[matched.Key] = matched.Value;
                            }
                        }));
        }
    }
}