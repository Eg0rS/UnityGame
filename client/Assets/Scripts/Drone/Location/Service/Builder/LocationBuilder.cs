using System;
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
using Drone.Location.Model.Player;
using Drone.Location.Model.Spline;
using Drone.Location.World;
using Drone.Obstacles;
using GameKit.World;
using JetBrains.Annotations;
using Tile.Service;
using AppContext = IoC.AppContext;
using Object = UnityEngine.Object;

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

        Dictionary<string, GameObject> _otherTiles;

        public Dictionary<ObstacleType, List<KeyValuePair<GameObject, int>>> Obstacles { get; set; }

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
            CreateContainer<PlayerModel>(PLAYER, ref _player);
            CreateContainer<SplineModel>(SPLINE, ref _spline);
            CreateContainer<SplineWalkerModel>(LEVEL, ref _level);
            // CreatePlayerContainer();
            // CreateSplineContainer();
            // CreateLevelContainer();
        }

        [NotNull]
        public LocationBuilder GameWorldContainer()
        {
            _droneWorld = new GameObject(GAME_WORLD);
            _droneWorld.transform.SetParent(_container, false);
            CreateContainers();
            return this;
        }

        private void CreateContainer<T>(string name, ref GameObject container)
                where T : Component
        {
            container = new GameObject(name);
            container.AddComponent<T>();
            container.transform.SetParent(_droneWorld.transform, false);
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

        private IPromise LoadPlayer()
        {
            _loadPromise = new Promise();
            _resourceService.LoadPrefab(PLAYER_CONTAINER_PATH, OnPlayerContainerLoaded);
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
            DroneWorld gameWorld = _droneWorld.AddComponent<DroneWorld>();
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
            LoadPlayer().Then(LoadTiles).Then(LoadObstacles).Then(BuildLevel).Then(CreateLevelSpline).Then(CreateGameWorld);
        }

        private void CreateLevelSpline()
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

            List<string> orderTile = _levelDescriptor.GameData.Tiles.TilesData.Select(x => x.Id).ToList();

            foreach (string order in orderTile) {
                GameObject instTile = Object.Instantiate(_otherTiles[order], _level.transform);

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

        private IPromise LoadObstacles()
        {
            _promise = new Promise();
            List<string> tilesIds = _levelDescriptor.GameData.Tiles.TilesData.Select(tile => tile.Id).Distinct().ToList();
            List<ObstacleType> types = new List<ObstacleType>();

            List<ObstacleType[]> t = _tileService.TileDescriptors.Tiles.Where(tile => tilesIds.Exists(tId => tId.Equals(tile.Id)))
                                                 .Select(x => x.ObstacleTypes)
                                                 .ToList();
            foreach (ObstacleType[] anyType in t) {
                foreach (ObstacleType type in anyType) {
                    if (!types.Exists(x => x.Equals(type))) {
                        types.Add(type);
                    }
                }
            }
            _tileService.ObstaclesService.LoadObstacles(types)
                        .Then(() => {
                            Obstacles = _tileService.ObstaclesService.Obstacles;
                            _promise.Resolve();
                        });
            return _promise;
        }
    }
}