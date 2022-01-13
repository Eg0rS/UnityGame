using System.Collections.Generic;
using AgkCommons.Extension;
using AgkCommons.Resources;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Spawner;
using Drone.Location.Service;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour, IWorldObjectController<SpawnerModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private CreateObjectService _createObjectService;

        private List<GameObject> _spawnPlaces;

        private const string S = "AssetObjects/Location/CityZone/CityZoneObstacles/pfObColumn3@embeded";

        public void Init(SpawnerModel model)
        {
            ObjectType = model.ObjectType;

            _spawnPlaces = gameObject.GetChildren();
            LoadObstaclesPrefabs();
        }

        private void LoadObstaclesPrefabs()
        {
            foreach (GameObject spawnPlace in _spawnPlaces) {
                _resourceService.LoadPrefab(S)
                                .Then(gameObject => Instantiate(gameObject, spawnPlace.transform))
                                .Done((x) => {
                                    List<PrefabModel> objectComponents = WorldObjectExtension.GetObjectComponents<PrefabModel>(x);
                                    objectComponents.ForEach(pfModel => _createObjectService.AttachController(pfModel));
                                });
            }
        }
    }
}