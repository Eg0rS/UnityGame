using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using Drone.Levels.Descriptor;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Spawner;
using Drone.Location.Service;
using IoC.Attribute;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour, IWorldObjectController<SpawnerModel>
    {
        private const float ROLLING_INTEREST = 20.0f;
        [Inject]
        private CreateLocationObjectService _createLocationObjectService;
        [Inject]
        private LoadLocationObjectService _loadLocationObjectService;
        public WorldObjectType ObjectType { get; set; }

        private Transform[] _spawnSpots;
        private TileDescriptor _descriptor;

        private Dictionary<LevelType, float> _difficult = new Dictionary<LevelType, float>();

        public void Init(SpawnerModel model)
        {
            _difficult.Add(LevelType.EASY, model.Diffcult.EasySpawnChance);
            _difficult.Add(LevelType.NORMAL, model.Diffcult.NormalSpawnChance);
            _difficult.Add(LevelType.HARD, model.Diffcult.HardSpawnChance);

            ObjectType = model.ObjectType;
            _descriptor = model.TileDescriptor;
            _spawnSpots = gameObject.GetComponentsOnlyInChildren<Transform>();
            SpawnObstacles();
        }

        private void SpawnObstacles()
        {
            foreach (Transform spawnSpot in _spawnSpots) {
                float sum = _difficult.Values.Sum();

                float random = UnityEngine.Random.Range(0, sum);

                float step = 0;
                foreach (KeyValuePair<LevelType, float> anyDif in _difficult) {
                    step += anyDif.Value;
                    if (!(random < step)) {
                        continue;
                    }
                    string type = _descriptor.ObstacleTypes[UnityEngine.Random.Range(0, _descriptor.ObstacleTypes.Length)].UnderscoreToCamelCase();
                    _loadLocationObjectService.LoadObstacle(_descriptor, type, anyDif.Key)
                                              .Then(go => {
                                                  GameObject instantiate = Instantiate(go, spawnSpot);
                                                  instantiate.GetChildren()[UnityEngine.Random.Range(0, instantiate.GetChildren().Count)]
                                                             .SetActive(true);
                                                  _createLocationObjectService.AttachController(instantiate.GetComponent<PrefabModel>());
                                              });
                    break;
                }
            }
        }
    }
}