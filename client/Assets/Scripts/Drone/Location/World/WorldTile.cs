using System.Collections.Generic;
using System.Linq;
using Drone.Location.World.Spawner;
using Drone.Obstacles;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.World
{
    public class WorldTile : WorldObjectExtension
    {
        public TileDescriptor Descriptor { get; set; }

        public WorldTile(TileDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public void Configurate(ref Dictionary<ObstacleType, Dictionary<GameObject, int>> allObstacles)
        {
            List<ObstacleType> obstacleTypesOnTile = Descriptor.ObstacleTypes.ToList();
            List<SpawnerController> spawners = GetObjectComponents<SpawnerController>();
            foreach (SpawnerController spawner in spawners) {
                spawner.SpawnObstacles(ref allObstacles, obstacleTypesOnTile);
            }
        }
    }
}