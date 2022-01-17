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

        public Dictionary<ObstacleType, List<KeyValuePair<Obstacle, int>>> Obstacles { get; set; }

        public void Init()
        {
        }

        public IPromise LoadObstacles(List<ObstacleType> types)
        {
            Obstacles = new Dictionary<ObstacleType, List<KeyValuePair<Obstacle, int>>>();
            List<IPromise> proms = new List<IPromise>();
            foreach (ObstacleType type in types) {
                List<ObstacleDescriptor> obstacleDescriptors = _obstacleDescriptors.Obstacles.Where(x => x.Type == type).ToList();
                foreach (ObstacleDescriptor obstacleDescriptor in obstacleDescriptors) {
                    proms.Add(_resourceService.LoadPrefab(obstacleDescriptor.Prefab)
                                              .Then(loadedObject =>
                                                            Obstacles[type]
                                                                    .Add(new KeyValuePair<Obstacle,
                                                                                 int>(new Obstacle(loadedObject, obstacleDescriptor.Id), 0))));
                }
            }
            return Promise.All(proms);
        }
    }

    public class Obstacle
    {
        public GameObject Object { get; }
        public string Id { get; }

        public Obstacle(GameObject gameObject, string id)
        {
            Object = gameObject;
            Id = id;
        }
    }
}