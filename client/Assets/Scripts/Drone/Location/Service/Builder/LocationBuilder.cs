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
        private const string SPLINE = "Spline";
        private const string PLAYER = "Player";
        private const string LEVEL = "Level";
        private const string PLAYER_CONTAINER_PATH = "World/pfPlayerContainer@embeded";

        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationBuilder>();
        private readonly CreateObjectService _createCreateService;
        private readonly ResourceService _resourceService;

        private Promise _promise;
        private string _prefab;
        private Transform _container;
        private GameObject _gameWorld;
        private GameObject _spline;
        private GameObject _player;
        private GameObject _level;

        private Promise _loadPromise;

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
            CreateWorldContainers();
            return _promise;
        }

        private void CreateWorldContainers()
        {
            _gameWorld = new GameObject(GAME_WORLD);
            _gameWorld.transform.SetParent(_container, false);

            _spline = new GameObject(SPLINE);
            _spline.transform.SetParent(_gameWorld.transform, false);

            CreatePlayerContainer().Then((() => CreateLevelContainer())).Then(() => _promise.Resolve());
        }

        private IPromise CreatePlayerContainer()
        {
            _loadPromise = new Promise();
            _player = new GameObject(PLAYER);
            _player.transform.SetParent(_gameWorld.transform, false);
            _resourceService.LoadPrefab(PLAYER_CONTAINER_PATH, OnPlayerContainerLoaded);
            return _loadPromise;
        }

        private IPromise CreateLevelContainer()
        {
            _loadPromise = new Promise();
            _level = new GameObject(LEVEL);
            _level.transform.SetParent(_gameWorld.transform, false);
            _resourceService.LoadPrefab(_prefab, OnLevelLoaded);
            return _loadPromise;
        }

        private void OnPlayerContainerLoaded(GameObject loadedObject, object[] loadparameters)
        {
            Object.Instantiate(loadedObject, _player.transform);
            _loadPromise.Resolve();
        }

        private void OnLevelLoaded(GameObject loadedObject, object[] loadparameters)
        {
            Object.Instantiate(loadedObject, _level.transform);
            GameWorld gameWorld = _gameWorld.AddComponent<GameWorld>();
            gameWorld.CreateWorld(WORLD_NAME);

            InitControllers(gameWorld);
            InitService();

            _loadPromise.Resolve();
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