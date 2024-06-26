using System.Collections.Generic;
using System.Linq;
using RSG;
using UnityEngine;
using Adept.Logger;
using BezierSolution;
using Drone.Core.Service;
using Drone.LevelDifficult.Descriptor;
using Drone.Levels.Descriptor;
using Drone.Location.Event;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.CutScene;
using Drone.Location.Model.Finish;
using Drone.Location.Model.Obstacle;
using Drone.Location.Model.Player;
using Drone.Location.Model.Spline;
using Drone.Location.Model.StartPlatform;
using Drone.Location.World;
using GameKit.World;
using JetBrains.Annotations;
using RSG.Promises;
using Tile.Descriptor;
using AppContext = IoC.AppContext;
using Object = UnityEngine.Object;

namespace Drone.Location.Service.Builder
{
    public class LocationBuilder
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationBuilder>();

        #region Const

        private const string START_CUTSCENE_PATH = "AssetObjects/CutScenes/pfCutSceneStart1@embeded";
        private const string FINISH_CUTSCENE_PATH = "AssetObjects/CutScenes/pfCutSceneFinish1@embeded";
        private const string PLAYER_CONTAINER_PATH = "World/pfPlayerContainer@embeded";
        private const string WORLD_NAME = "location";
        private const string GAME_WORLD = "GameWorld";
        private const string SPLINE = "Spline";
        private const string PLAYER = "Player";
        private const string LEVEL = "Level";
        private const string CHIPS = "Chips";
        private const string CUT_SCENES = "CutScenes";

        #endregion

        #region Util

        private readonly CreateLocationObjectService _createObjectService;
        private readonly LoadLocationObjectService _loadObjectService;

        #endregion

        #region Containers

        private Transform _container;
        private GameObject _droneWorld;
        private GameObject _spline;
        private GameObject _player;
        private GameObject _level;
        private GameObject _chips;
        private GameObject _cutScenes;

        #endregion

        #region VariablesToGenerate

        private LevelDescriptor _levelDescriptor;
        private DifficultDescriptor _difficultDescriptor;
        private Dictionary<TileDescriptor, GameObject> _tiles;
        private List<WorldTile> _worldTiles = new List<WorldTile>();
        private uint _seed;
        private StartPlatformModel _start;
        private FinishModel _finish;

        #endregion

        #region Inteface

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
        public LocationBuilder SetSeed(uint seed)
        {
            _seed = seed;
            return this;
        }

        [NotNull]
        public LocationBuilder Difficult(DifficultDescriptor difficultDescriptors)
        {
            _difficultDescriptor = difficultDescriptors;
            return this;
        }

        [NotNull]
        public LocationBuilder LevelDescriptor(LevelDescriptor levelDescriptor)
        {
            _levelDescriptor = levelDescriptor;
            return this;
        }

        [NotNull]
        public LocationBuilder GameWorldContainer()
        {
            _droneWorld = new GameObject(GAME_WORLD);
            _droneWorld.transform.SetParent(_container, false);
            CreateContainers();
            return this;
        }

        public void Build()
        {
            LoadPlayer()
                    .Then(LoadTiles)
                    .Then(LoadCutScenes)
                    .Then(CreateLevelTiles)
                    .Then(CreateLevelSpline)
                    .Then(ConfigureTiles)
                    .Then(CreateChipsPath)
                    .Then(CreateGameWorld);
        }

        #endregion

        #region Constructor

        private LocationBuilder(CreateLocationObjectService createObjectService, LoadLocationObjectService loadObjectService)
        {
            _createObjectService = createObjectService;
            _loadObjectService = loadObjectService;
        }

        #endregion

        #region Implementation

        #region LoadMethods

        private IPromise LoadPlayer()
        {
            return _loadObjectService.LoadResource<GameObject>(PLAYER_CONTAINER_PATH)
                                     .Then(loadObject => { Object.Instantiate(loadObject, _player.transform); });
        }

        [NotNull]
        private IPromise LoadTiles()
        {
            return _loadObjectService.LoadLevelTiles(_levelDescriptor).Then(tiles => { _tiles = tiles; });
        }

        private IPromise LoadCutScenes()
        {
            return _loadObjectService.LoadResources<GameObject>(new[] {START_CUTSCENE_PATH, FINISH_CUTSCENE_PATH})
                                     .Then(cutScenes => {
                                         foreach (GameObject cutScene in cutScenes) {
                                             Object.Instantiate(cutScene, _cutScenes.transform);
                                         }
                                     });
        }

        #endregion

        #region СreationMethods

        [NotNull]
        private GameObject CreateContainer<T>(string name, Transform parent)
                where T : Component
        {
            GameObject container = new GameObject(name);
            container.AddComponent<T>();
            container.transform.SetParent(parent, false);
            return container;
        }

        private void CreateContainers()
        {
            _player = CreateContainer<PlayerModel>(PLAYER, _droneWorld.transform);
            _spline = CreateContainer<SplineModel>(SPLINE, _droneWorld.transform);
            _level = CreateContainer<SplineWalkerModel>(LEVEL, _droneWorld.transform);
            _chips = CreateContainer<ChipsLineCreator>(CHIPS, _level.transform);
            _cutScenes = CreateContainer<CutScenesModel>(CUT_SCENES, _droneWorld.transform);
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

        private void CreateLevelTiles()
        {
            WorldTile lastTile = null;
            _levelDescriptor.GameData.Tiles.TilesData.Each(tileId => {
                KeyValuePair<TileDescriptor, GameObject> tile = _tiles.First(pair => pair.Key.Id == tileId.Id);
                WorldTile worldTile = Object.Instantiate(tile.Value, _level.transform).gameObject.AddComponent<WorldTile>().Init(tile.Key, lastTile);
                if (_start == null) {
                    _start = worldTile.GetComponentInChildren<StartPlatformModel>();
                }
                if (_finish == null) {
                    _finish = worldTile.GetComponentInChildren<FinishModel>();
                }
                _worldTiles.Add(worldTile);
                lastTile = worldTile;
            });
        }

        private IPromise CreateChipsPath(List<ObstacleInfo> obstacles)
        {
            ChipsLineCreator chips = _chips.GetComponent<ChipsLineCreator>();

            return chips.Init(obstacles, _seed, _levelDescriptor.ChipsForPassing, _loadObjectService, _start.transform, _finish.transform);
        }

        private void CreateGameWorld()
        {
            DroneWorld gameWorld = _droneWorld.AddComponent<DroneWorld>();
            gameWorld.CreateWorld(WORLD_NAME);
            InitControllers(gameWorld);
            InitService();
            gameWorld.Dispatch(new WorldEvent(WorldEvent.CREATED));
        }

        #endregion

        #region InitMethods

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

        #endregion

        #region ConfigureMethods

        private IPromise<List<ObstacleInfo>> ConfigureTiles()
        {
            return ObstacleFactory.Create(_loadObjectService)
                                  .SetWorldTiles(_worldTiles)
                                  .SetSeed(_seed)
                                  .SetDifficult(_difficultDescriptor)
                                  .StartConfigureTiles();
        }

        #endregion

        #endregion
    }
}