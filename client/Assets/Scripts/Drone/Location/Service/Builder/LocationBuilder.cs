using System.Collections.Generic;
using AgkCommons.Resources;
using RSG;
using UnityEngine;
using Adept.Logger;
using Drone.Core.Service;
using Drone.Location.Model.BaseModel;
using Drone.World;
using IoC;

namespace Drone.Location.Service.Builder
{
    public class LocationBuilder
    {
        private const string WORLD_NAME = "location";
        private const string GAME_WORLD = "GameWorld";

        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationBuilder>();
        private readonly CreateObjectService _createCreateService;
        private readonly ResourceService _resourceService;

        private Promise _promise;
        private string _prefab;
        private Transform _container;
        private GameObject _gameWorld;

        private LocationBuilder(ResourceService resourceService, CreateObjectService createService)
        {
            _resourceService = resourceService;
            _createCreateService = createService;
        }

        public static LocationBuilder Create(ResourceService resourceService, CreateObjectService createService)
        {
            return new LocationBuilder(resourceService, createService);
        }

        public LocationBuilder Prefab(string prefab)
        {
            _prefab = prefab;
            return this;
        }

        public LocationBuilder Container(Transform container)
        {
            _container = container;
            return this;
        }

        public IPromise Build()
        {
            _promise = new Promise();
            _gameWorld = new GameObject(GAME_WORLD);
            _gameWorld.transform.SetParent(_container, false);
            _resourceService.LoadPrefab(_prefab, OnPrefabLoad);
            return _promise;
        }

        private void OnPrefabLoad(GameObject loadedObject, object[] loadparameters)
        {
            Object.Instantiate(loadedObject, _gameWorld.transform);
            GameWorld gameWorld = _gameWorld.AddComponent<GameWorld>();
            gameWorld.CreateWorld(WORLD_NAME);


            InitControllers(gameWorld);
            InitService();

            _promise.Resolve();
        }

        private static void InitService()
        {
            foreach (IWorldServiceInitiable serviceInitable in AppContext.ResolveCollection<IWorldServiceInitiable>()) {
                serviceInitable.Init();
            }
        }

        private void InitControllers(GameWorld gameWorld)
        {
            List<PrefabModel> objectComponents = gameWorld.GetObjectComponents<PrefabModel>();
            foreach (PrefabModel prefabModel in objectComponents) {
                _logger.Debug("attach");
                _createCreateService.AttachController(prefabModel);
            }
        }
    }
}