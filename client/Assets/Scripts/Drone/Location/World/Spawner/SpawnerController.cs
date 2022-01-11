using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using AgkCommons.Resources;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Spawner;
using Drone.Location.Service;
using IoC.Attribute;
using RSG;
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

        private Promise _promise;

        public void Init(SpawnerModel model)
        {
            ObjectType = model.ObjectType;

            _spawnPlaces = gameObject.GetChildren();
            LoadObstaclesPrefabs().Then(InitControllers);
        }

        private void InitControllers()
        {
            List<PrefabModel> objectComponents = GetObjectComponents<PrefabModel>();
            foreach (PrefabModel prefabModel in objectComponents) {
                _createObjectService.AttachController(prefabModel);
            }
        }

        private IPromise LoadObstaclesPrefabs()
        {
            _promise = new Promise();
            foreach (GameObject spawnPlace in _spawnPlaces) {
                _resourceService.LoadPrefab(S).Then(go => Instantiate(go, spawnPlace.transform)).Done();
            }
            _promise.Resolve();
            return _promise;
        }

        private void OnPlayerContainerLoaded(GameObject loadedObject, object[] loadparameters)
        {
            Instantiate(loadedObject, transform);
            List<PrefabModel> objectComponents = GetObjectComponents<PrefabModel>();
            foreach (PrefabModel prefabModel in objectComponents) {
                _createObjectService.AttachController(prefabModel);
            }
        }

        public List<GameObject> GetSceneObjects()
        {
            return gameObject.GetComponentsOnlyInChildren<Transform>(true).ToList().Select(t => t.gameObject).ToList();
        }

        public List<T> GetObjectComponents<T>()
        {
            return GetSceneObjects().Where(go => go.GetComponent<T>() != null).Select(go => go.GetComponent<T>()).ToList();
        }
    }
}