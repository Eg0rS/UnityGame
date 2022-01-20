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
using Drone.Location.World.Spawner;
using Drone.Obstacles;
using GameKit.World;
using JetBrains.Annotations;
using Tile.Descriptor;
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

        private Dictionary<TileDescriptor, GameObject> _tiles;
        private List<WorldTile> _worldTiles = new List<WorldTile>();

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
            _tileService.LoadTilesByIds(_levelDescriptor)
                        .Then(() => {
                            _tiles = _tileService.LoadedTiles;
                            _promise.Resolve();
                        });
            return _promise;
        }

        public void Check()
        {
            LoadPlayer().Then(LoadTiles).Then(BuildLevel).Then(ConfigurateTiles).Then(CreateLevelSpline).Then(CreateGameWorld);
        }

        private IPromise ConfigurateTiles()
        {
            Promise promise = new Promise();
            List<ObstacleType> obstacleTypes = new List<ObstacleType>();
            foreach (WorldTile worldTile in _worldTiles) {
                obstacleTypes.AddRange(worldTile.Descriptor.ObstacleTypes);
            }
            _tileService.ObstaclesService.LoadObstacles(obstacleTypes.Distinct().ToList())
                        .Then(() => {
                            Dictionary<ObstacleType, Dictionary<GameObject, int>> allObstacles = _tileService.ObstaclesService.Obstacles;
                            foreach (WorldTile worldTile in _worldTiles) {
                                ConfigTile(allObstacles, worldTile);
                            }
                            promise.Resolve();
                        });

            return promise;
        }

        private void ConfigTile(Dictionary<ObstacleType, Dictionary<GameObject, int>> allObstacles, WorldTile tile)
        {
            List<ObstacleType> obstacleTypes = tile.Descriptor.ObstacleTypes.ToList();
            Dictionary<GameObject, int> obstacleOnTile = new Dictionary<GameObject, int>();
            foreach (Dictionary<GameObject, int> dictionary in allObstacles.Where(ob => obstacleTypes.Exists(x => x == ob.Key)).Select(x => x.Value)) {
                foreach (KeyValuePair<GameObject, int> o in dictionary) {
                    obstacleOnTile[o.Key] = o.Value;
                }
            }
            List<SpawnerController> spawners = tile.GetObjectComponents<SpawnerController>();
            foreach (SpawnerController spawner in spawners) {
                spawner.SpawnObstacles(ref obstacleOnTile);
            }
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
            WorldTile lastTile = null;

            List<string> orderTile = _levelDescriptor.GameData.Tiles.TilesData.Select(x => x.Id).ToList();

            foreach (string order in orderTile) {
                KeyValuePair<TileDescriptor, GameObject> tile = _tiles.First(x => x.Key.Id == order);

                GameObject instTile = Object.Instantiate(tile.Value, _level.transform);

                WorldTile worldTile = instTile.AddComponent<WorldTile>();
                worldTile.Descriptor = tile.Key;
                if (lastTile == null) {
                    lastTile = worldTile;
                } else {
                    GameObject anchors = lastTile.gameObject.GetChildren().First(x => x.name == "Anchors");
                    Transform end = anchors.GetChildren().Find(x => x.name == "End").transform;

                    GameObject anchors1 = lastTile.gameObject.GetChildren().First(x => x.name == "Anchors");
                    Transform begin = anchors1.GetChildren().Find(x => x.name == "Begin").transform;

                    instTile.transform.position = end.position - begin.localPosition;
                    lastTile = worldTile;
                }
                _worldTiles.Add(worldTile);
            }
        }
    }
}