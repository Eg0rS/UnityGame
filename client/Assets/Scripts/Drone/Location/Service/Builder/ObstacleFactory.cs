using System.Collections.Generic;
using System.Linq;
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

        private Promise _spawnObstaclesPromise;
        private Promise _configTilePromise;

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

        public IPromise StartConfigureTiles()
        {
            _randomGenerator = new MTRandomGenerator(_seed);
            _configTilePromise = new Promise();
            return ConfigureTiles(0).Then(() => { Debug.Log("Tiles configuration completed"); });
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
                                          _configTilePromise.Resolve();
                                      }
                                  }));
            } else {
                _configTilePromise.Resolve();
            }
            return _configTilePromise;
        }

        private IPromise ConfigTile(WorldTile worldTile)
        {
            Transform parentObstacles = worldTile.gameObject.GetChildren().First(x => x.name == "Obstacle").transform;
            _spawnObstaclesPromise = new Promise();
            return SpawnObstacle(worldTile, parentObstacles);
        }

        private IPromise SpawnObstacle(WorldTile worldTile, Transform parentObstacles)
        {
            if (_lastObstaclePosition.magnitude < worldTile.End.position.magnitude) {
                Zone flag = worldTile.Descriptor.RedZones?.FirstOrDefault(x => _lastObstaclePosition.magnitude >= x.Begin
                                                                               && _lastObstaclePosition.magnitude < x.End);
                if (flag != null) {
                    _lastObstaclePosition.z += flag.End;
                    _spawnObstaclesPromise = (Promise) SpawnObstacle(worldTile, parentObstacles);
                } else {
                    string type = worldTile.Descriptor.ObstacleTypes[_randomGenerator.Range(0, worldTile.Descriptor.ObstacleTypes.Length)]
                                           .UnderscoreToCamelCase();
                    _loadObjectService.LoadObstacle(worldTile.Descriptor, type)
                                      .Then(go => {
                                          GameObject instantiate = Object.Instantiate(go, parentObstacles);
                                          instantiate.transform.position = _lastObstaclePosition;
                                          ObstacleInfo skin = instantiate.GetChildren()[_randomGenerator.Range(0, instantiate.GetChildren().Count)]
                                                                         .GetComponent<ObstacleInfo>();
                                          skin.gameObject.SetActive(true);
                                          _lastObstaclePosition += _spawnStep + new Vector3(0, 0, skin.Depth);
                                          _spawnObstaclesPromise = (Promise) SpawnObstacle(worldTile, parentObstacles);
                                      });
                }
            } else {
                _spawnObstaclesPromise.Resolve();
            }
            return _spawnObstaclesPromise;
        }

        #endregion
    }
}