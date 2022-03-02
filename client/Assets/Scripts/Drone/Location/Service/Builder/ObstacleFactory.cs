using System.Collections.Generic;
using System.Linq;
using Adept.Logger;
using AgkCommons.Extension;
using Drone.LevelDifficult.Descriptor;
using Drone.Location.Model.Obstacle;
using Drone.Location.World;
using Drone.Random.MersenneTwister;
using JetBrains.Annotations;
using RSG;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class ObstacleFactory
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ObstacleFactory>();

        #region Util

        private MTRandomGenerator _randomGenerator;
        private readonly LoadLocationObjectService _loadObjectService;

        #endregion

        #region VariablesToGenerate

        private Vector3 _lastObstaclePosition = Vector3.zero;
        private List<WorldTile> _tiles;
        private uint _seed;
        private DifficultDescriptor _difficultDescriptor;
        private Vector3 _spawnStep;

        #endregion

        #region Promises

        private Promise _allObstaclesSpawnPromise;
        private Promise _configTilePromise;

        #endregion

        #region ReturnValues

        private List<ObstacleInfo> _obstacleInfos;

        #endregion

        #region Interface

        [NotNull]
        public static ObstacleFactory Create(LoadLocationObjectService loadObjectService)
        {
            return new ObstacleFactory(loadObjectService);
        }

        [NotNull]
        public ObstacleFactory SetWorldTiles(List<WorldTile> tiles)
        {
            _tiles = tiles;
            return this;
        }

        [NotNull]
        public ObstacleFactory SetSeed(uint seed)
        {
            _seed = seed;
            return this;
        }

        [NotNull]
        public ObstacleFactory SetDifficult(DifficultDescriptor difficultDescriptor)
        {
            _difficultDescriptor = difficultDescriptor;
            _spawnStep = new Vector3(0, 0, _difficultDescriptor.SpawnStep);
            return this;
        }

        public IPromise<List<ObstacleInfo>> StartConfigureTiles()
        {
            _obstacleInfos = new List<ObstacleInfo>();
            _randomGenerator = new MTRandomGenerator(_seed);
            _configTilePromise = new Promise();
            return Promise.All(ConfigureTiles(0)).Then((() => Promise<List<ObstacleInfo>>.Resolved(_obstacleInfos)));
        }

        #endregion

        #region Constructor

        private ObstacleFactory(LoadLocationObjectService loadObjectService)
        {
            _loadObjectService = loadObjectService;
        }

        #endregion

        #region Implementaton

        private IPromise ConfigureTiles(int i)
        {
            if (i < _tiles.Count) {
                ConfigTile(_tiles[i])
                        .Then((() => {
                                      if (i + 1 < _tiles.Count) {
                                          _configTilePromise = (Promise) ConfigureTiles(i + 1);
                                      } else {
                                          _logger.Debug("Tiles configuration completed");
                                          _configTilePromise.Resolve();
                                      }
                                  }));
            } else {
                _logger.Debug("Tiles configuration completed");
                _configTilePromise.Resolve();
            }
            return _configTilePromise;
        }

        private IPromise ConfigTile(WorldTile worldTile)
        {
            Transform parentObstacles = worldTile.gameObject.GetChildren().First(x => x.name == "Obstacle").transform;
            _allObstaclesSpawnPromise = new Promise();
            return SpawnObstacle(worldTile, parentObstacles);
        }

        private IPromise SpawnObstacle(WorldTile worldTile, Transform parentObstacles)
        {
            if (_lastObstaclePosition.magnitude < worldTile.End.position.magnitude) {
                Zone redZone = worldTile.Descriptor.RedZones?.FirstOrDefault(x => _lastObstaclePosition.magnitude >= x.Begin
                                                                                  && _lastObstaclePosition.magnitude < x.End);
                if (redZone == null) {
                    string type = ChoiceObstacleType(worldTile.Descriptor.ObstacleTypes);
                    _loadObjectService.LoadObstacle(worldTile.Descriptor, type)
                                      .Then(go => {
                                          InitObstacle(Object.Instantiate(go, parentObstacles));
                                          _allObstaclesSpawnPromise = (Promise) SpawnObstacle(worldTile, parentObstacles);
                                      });
                } else {
                    _lastObstaclePosition.z += redZone.End;
                    _allObstaclesSpawnPromise = (Promise) SpawnObstacle(worldTile, parentObstacles);
                }
            } else {
                _allObstaclesSpawnPromise.Resolve();
            }
            return _allObstaclesSpawnPromise;
        }

        private void InitObstacle(GameObject obstacle)
        {
            obstacle.transform.position = _lastObstaclePosition;
            ObstacleInfo skin = obstacle.GetChildren()[_randomGenerator.Range(0, obstacle.GetChildren().Count)].GetComponent<ObstacleInfo>();
            skin.gameObject.SetActive(true);
            _obstacleInfos.Add(skin);
            _lastObstaclePosition += _spawnStep + new Vector3(0, 0, skin.Depth);
        }

        private string ChoiceObstacleType(string[] obstacleTypes)
        {
            return obstacleTypes[_randomGenerator.Range(0, obstacleTypes.Length)].UnderscoreToCamelCase();
        }

        #endregion
    }
}