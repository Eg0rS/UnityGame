using System.Collections.Generic;
using System.Linq;
using RSG;
using UnityEngine;
using Adept.Logger;
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
using Tile.Descriptor;
using AppContext = IoC.AppContext;
using Object = UnityEngine.Object;

namespace Drone.Location.Service.Builder
{
    public class LocationBuilder
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationBuilder>();

        private const string PLAYER_CONTAINER_PATH = "World/pfPlayerContainer@embeded";
        private const string SPOT_PATH = "World/pfSpot@embeded";
        private const string WORLD_NAME = "location";
        private const string GAME_WORLD = "GameWorld";
        private const string SPLINE = "Spline";
        private const string PLAYER = "Player";
        private const string LEVEL = "Level";

        private readonly CreateLocationObjectService _createObjectService;
        private readonly LoadLocationObjectService _loadObjectService;

        private Transform _container;
        private GameObject _droneWorld;
        private GameObject _spline;
        private GameObject _player;
        private GameObject _level;
        private GameObject _spot;

        private LevelDescriptor _levelDescriptor;

        private Dictionary<TileDescriptor, GameObject> _tiles;
        private List<WorldTile> _worldTiles = new List<WorldTile>();
        private Dictionary<ObstacleType, Dictionary<GameObject, int>> _obstacles;

        private LocationBuilder(CreateLocationObjectService createObjectService, LoadLocationObjectService loadObjectService)
        {
            _createObjectService = createObjectService;
            _loadObjectService = loadObjectService;
        }

        [NotNull]
        public static LocationBuilder Create(CreateLocationObjectService createObjectService, LoadLocationObjectService loadObjectService)
        {
            return new LocationBuilder(createObjectService, loadObjectService);
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
            Promise promise = new Promise();
            _loadObjectService.LoadResource<GameObject>(PLAYER_CONTAINER_PATH)
                              .Then(loadObject => { Object.Instantiate(loadObject, _player.transform); })
                              .Then(() => _loadObjectService.LoadResource<GameObject>(SPOT_PATH)
                                                            .Then(loadObject => {
                                                                _spot = loadObject;
                                                                promise.Resolve();
                                                            }));
            return promise;
        }

        private void CreateGameWorld()
        {
            DroneWorld gameWorld = _droneWorld.AddComponent<DroneWorld>();
            gameWorld.CreateWorld(WORLD_NAME);

            InitControllers(gameWorld);
            InitService();
            gameWorld.Dispatch(new WorldEvent(WorldEvent.CREATED));
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
        private IPromise LoadTiles()
        {
            Promise promise = new Promise();
            _loadObjectService.LoadLevelTiles(_levelDescriptor)
                              .Then(tiles => {
                                  _tiles = tiles;
                                  promise.Resolve();
                              });
            return promise;
        }

        public void Build()
        {
            LoadPlayer().Then(LoadTiles).Then(BuildLevel).Then(CreateLevelSpline).Then(ConfigureTiles).Then(CreateGameWorld);
        }

        [NotNull]
        private IPromise ConfigureTiles()
        {
            Promise promise = new Promise();

            foreach (WorldTile worldTile in _worldTiles) {
                worldTile.Configure();
            }
            promise.Resolve();
            return promise;
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
                WorldTile worldTile = Object.Instantiate(tile.Value, _level.transform).gameObject.AddComponent<WorldTile>();
                worldTile.Init(tile.Key, _spot);
                _worldTiles.Add(worldTile);
                if (lastTile == null) {
                    lastTile = worldTile;
                    continue;
                }
                worldTile.gameObject.transform.position = lastTile.End.position - lastTile.Begin.localPosition;
                lastTile = worldTile;
            }
        }
    }
}