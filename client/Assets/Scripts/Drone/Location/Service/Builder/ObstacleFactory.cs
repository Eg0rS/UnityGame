using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using Drone.LevelDifficult.Descriptor;
using Drone.Location.Event;
using Drone.Location.Model.Obstacle;
using Drone.Location.World;
using Drone.Random.MersenneTwister;
using JetBrains.Annotations;
using RSG;
using RSG.Promises;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class ObstacleFactory
    {
        private Vector3 _lastObstaclePosition = Vector3.zero;
        private MTRandomGenerator _randomGenerator;
        private readonly LoadLocationObjectService _loadObjectService;
        private List<WorldTile> _tiles;
        private uint _seed;
        private DifficultDescriptor _difficultDescriptor;

        #region Interface

        [NotNull]
        public ObstacleFactory Create(LoadLocationObjectService loadObjectService)
        {
            return new ObstacleFactory(loadObjectService);
        }

        [NotNull]
        public ObstacleFactory WorldTiles(List<WorldTile> tiles)
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
            return this;
        }

        public void StartConfigureTiles()
        {
            _randomGenerator = new MTRandomGenerator(_seed);
            ConfigureTiles();
        }

        #endregion

        #region Constructor

        private ObstacleFactory(LoadLocationObjectService loadObjectService)
        {
            _loadObjectService = loadObjectService;
        }

        #endregion

        #region Implementaton

        private IPromise ConfigureTiles()
        {
            List<IPromise> promises = new List<IPromise>();
            _tiles.Each(tile => promises.Add(ConfigureTile(tile)));
            return Promise.All(promises);
        }

        private IPromise ConfigureTile(WorldTile tile)
        {
            List<IPromise> promises = new List<IPromise>();
            Transform obstacles = tile.gameObject.GetChildren().First(x => x.name == "Obstacle").transform;
            Vector3 step = new Vector3(0, 0, _difficultDescriptor.SpawnStep);
            Vector3 position = _lastObstaclePosition;
            while (position.magnitude <= tile.End.position.magnitude) {
                Zone flag = tile.Descriptor.RedZones?.FirstOrDefault(x => position.magnitude >= x.Begin && position.magnitude <= x.End);
                if (flag != null) {
                    position.z += flag.End;
                }
                if (position.magnitude <= tile.End.position.magnitude || flag == null) {
                    CreateObstacle(obstacles, position, tile.Descriptor);
                }
                position += step;
            }
            _lastObstaclePosition = position;
            
        }

        private IPromise CreateObstacle(Transform parent, Vector3 position, TileDescriptor tileDescriptor)
        {
            Promise promise = new Promise();
            string type = tileDescriptor.ObstacleTypes[_randomGenerator.Range(0, tileDescriptor.ObstacleTypes.Length)].UnderscoreToCamelCase();
            _loadObjectService.LoadObstacle(tileDescriptor, type)
                              .Then(go => {
                                  GameObject instantiate = Object.Instantiate(go, parent);
                                  instantiate.transform.position = position;
                                  ObstacleInfo skin = instantiate.GetChildren()[_randomGenerator.Range(0, instantiate.GetChildren().Count)]
                                                                 .GetComponent<ObstacleInfo>();
                                  skin.gameObject.SetActive(true);
                                  position.z += skin.Depth;
                                  promise.Resolve();
                              });
            return promise;
        }

        #endregion
    }
}