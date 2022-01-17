using System.Collections.Generic;
using System.Linq;
using AgkCommons.Resources;
using RSG;
using UnityEngine;
using Adept.Logger;
using AgkCommons.Extension;
using BezierSolution;
using Drone.Core.Service;
using Drone.Levels.Descriptor;
using Drone.Location.Event;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Spline;
using Drone.Location.World;
using GameKit.World;
using JetBrains.Annotations;
using Tile.Service;
using AppContext = IoC.AppContext;

namespace Drone.Location.Service.Builder
{
    public class LocationBuilder
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationBuilder>();

        private const string PLAYER_CONTAINER_PATH = "World/pfPlayerContainer@embeded";
        private const string WORLD_NAME = "location";
        private const string GAME_WORLD = "GameWorld";
        private const string SPLINE = "Spline";
        private const string PLAYER = "Player";
        private const string LEVEL = "Level";
        private readonly Vector3 _defaultPlayerPosition = new Vector3(0, 1.5f, 0);

        private readonly CreateObjectService _createObjectService;
        private readonly ResourceService _resourceService;
        private readonly TileService _tileService;

        private Promise _promise;
        private Transform _container;
        private GameObject _droneWorld;
        private GameObject _spline;
        private GameObject _player;
        private GameObject _level;

        private Promise _loadPromise;

        private LevelDescriptor _levelDescriptor;

        List<GameObject> _otherTiles = new List<GameObject>();

        private LocationBuilder(ResourceService resourceService, CreateObjectService createService, TileService tileService)
        {
            _resourceService = resourceService;
            _createObjectService = createService;
            _tileService = tileService;
        }

        [NotNull]
        public static LocationBuilder Create(ResourceService resourceService, CreateObjectService createService, TileService tileService)
        {
            return new LocationBuilder(resourceService, createService, tileService);
        }

        [NotNull]
        public LocationBuilder Container(Transform container)
        {
            _container = container;
            return this;
        }

        [NotNull]
        public LocationBuilder LevelDescriptor(LevelDescriptor levelDescriptor)
        {
            _levelDescriptor = levelDescriptor;
            return this;
        }

        private void CreateContainers()
        {
            CreatePlayerContainer();
            CreateSplineContainer();
            CreateLevelContainer();
        }

        [NotNull]
        public LocationBuilder GameWorldContainer()
        {
            _droneWorld = new GameObject(GAME_WORLD);
            _droneWorld.transform.SetParent(_container, false);
            CreateContainers();
            return this;
        }

        private void CreateSplineContainer()
        {
            _spline = new GameObject(SPLINE);
            _spline.AddComponent<SplineModel>();
            _spline.transform.SetParent(_droneWorld.transform, false);
        }

        private void CreateLevelContainer()
        {
            _level = new GameObject(LEVEL);
            _level.AddComponent<SplineWalkerModel>();
            _level.transform.SetParent(_droneWorld.transform, false);
        }

        private void CreatePlayerContainer()
        {
            _player = new GameObject(PLAYER);
            _player.transform.SetParent(_droneWorld.transform, false);
        }

        public IPromise Build()
        {
            _promise = new Promise();
            LoadPlayer().Then(LoadLevel).Then(CreateLevelSpline).Then(CreateGameWorld);
            return _promise;
        }

        private void CreateLevelSpline()
        {
            BezierSpline levelBezier = _level.GetComponentInChildren<BezierSpline>();
            List<BezierPoint> points = levelBezier.gameObject.GetComponentsInChildren<BezierPoint>().ToList();
            foreach (BezierPoint point in points) {
                point.gameObject.transform.SetParent(_spline.transform, false);
            }
        }

        private IPromise LoadPlayer()
        {
            _loadPromise = new Promise();
            _resourceService.LoadPrefab(PLAYER_CONTAINER_PATH, OnPlayerContainerLoaded);
            return _loadPromise;
        }

        private IPromise LoadLevel()
        {
            _loadPromise = new Promise();
            //_resourceService.LoadPrefab(_prefab, OnLevelLoaded);
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
            _loadPromise.Resolve();
        }

        private void CreateGameWorld()
        {
            GameWorld gameWorld = _droneWorld.AddComponent<DroneWorld>();
            gameWorld.CreateWorld(WORLD_NAME);

            InitControllers(gameWorld);
            InitService();
            gameWorld.Dispatch(new WorldEvent(WorldEvent.CREATED));

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
                _createObjectService.AttachController(prefabModel);
            }
        }

        [NotNull]
        public IPromise LoadTiles()
        {
            _promise = new Promise();
            string[] tilesIds = _levelDescriptor.GameData.Tiles.TilesData.Select(tile => tile.Id).ToArray();

            _tileService.LoadTilesByIds(tilesIds)
                        .Then(() => {
                            _otherTiles = _tileService.LoadedTiles;
                            _promise.Resolve();
                        });
            return _promise;
        }

        public void Check()
        {
            LoadPlayer().Then(LoadTiles).Then(BuildLevel).Then(CreateLevelSpline1).Then(CreateGameWorld);
        }

        private void CreateLevelSpline1()
        {
            List<BezierSpline> levelBezier = _level.GetComponentsInChildren<BezierSpline>().ToList();
            foreach (BezierSpline spline in levelBezier) {
                List<BezierPoint> points = spline.gameObject.GetComponentsInChildren<BezierPoint>().ToList();
                foreach (BezierPoint point in points) {
                    point.gameObject.transform.SetParent(_spline.transform, true);
                }
            }
        }

        private void BuildLevel()
        {
            GameObject lastTile = null;
            foreach (GameObject tile in _otherTiles) {
                GameObject instTile = Object.Instantiate(tile, _level.transform);
                if (lastTile == null) {
                    lastTile = instTile;
                } else {
                    GameObject anchors = lastTile.GetChildren().First(x => x.name == "Anchors");
                    Transform end = anchors.GetChildren().Find(x => x.name == "End").transform;

                    GameObject anchors1 = lastTile.GetChildren().First(x => x.name == "Anchors");
                    Transform begin = anchors1.GetChildren().Find(x => x.name == "Begin").transform;

                    instTile.transform.position = end.position - begin.localPosition;
                    lastTile = instTile;
                }
            }
        }
    }
}