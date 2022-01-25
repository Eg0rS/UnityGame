using System.Collections.Generic;
using System.Linq;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Obstacles.Descriptor;
using IoC.Attribute;
using RSG;
using UnityEngine;

namespace Drone.Obstacles.Service
{
    public class ObstaclesService : IWorldServiceInitiable
    {
        [Inject]
        private ObstacleDescriptors _obstacleDescriptors;
        [Inject]
        private ResourceService _resourceService;

        public Dictionary<ObstacleType, Dictionary<GameObject, int>> Obstacles { get; set; }

        public void Init()
        {
        }

        public IPromise LoadObstacles(List<ObstacleType> types)
        {
            List<IPromise> proms = new List<IPromise>();
            Obstacles = new Dictionary<ObstacleType, Dictionary<GameObject, int>>();
            foreach (ObstacleType type in types) {
                Obstacles[type] = new Dictionary<GameObject, int>();
                List<ObstacleDescriptor> obstacleDescriptors = _obstacleDescriptors.Obstacles.Where(x => x.Type == type).ToList();
                foreach (ObstacleDescriptor obstacleDescriptor in obstacleDescriptors) {
                    proms.Add(_resourceService.LoadPrefab(obstacleDescriptor.Prefab).Then(go => Obstacles[type].Add(go, 0)));
                }
            }
            return Promise.All(proms);
        }
    }
}