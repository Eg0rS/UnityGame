using System.Collections.Generic;
using Drone.Core.Service;
using UnityEngine;

namespace Drone.Obstacles.Service
{
    public class ObstaclesService : IConfigurable
    {
        private List<GameObject> _loadedObstacles;

        public void Configure()
        {
        }
    }
}