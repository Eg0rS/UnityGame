using Drone.Location.Model;
using Drone.Location.Model.Spawner;
using UnityEngine;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour, IWorldObjectController<SpawnerModel>
    {
        public WorldObjectType ObjectType { get; }

        public void Init(SpawnerModel model)
        {
            
        }
    }
}