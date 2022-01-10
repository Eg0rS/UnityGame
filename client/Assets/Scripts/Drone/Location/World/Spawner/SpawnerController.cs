using AgkCommons.Resources;
using Drone.Location.Model;
using Drone.Location.Model.Spawner;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour, IWorldObjectController<SpawnerModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private ResourceService _resourceService;

        private const string S = "AssetObjects/Location/CityZone/CityZoneObstacles/pfObColumn3@embeded";

        public void Init(SpawnerModel model)
        {
            ObjectType = model.ObjectType;
            _resourceService.LoadPrefab(S, OnPlayerContainerLoaded);
        }

        private void OnPlayerContainerLoaded(GameObject loadedObject, object[] loadparameters)
        {
            Instantiate(loadedObject, transform); 
        }
    }
}