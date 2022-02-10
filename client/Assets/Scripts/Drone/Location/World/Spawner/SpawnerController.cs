using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using Drone.Location.Model;
using Drone.Location.Model.Spawner;
using Drone.Location.Service;
using IoC.Attribute;
using Tile.Descriptor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Drone.Location.World.Spawner
{
    public class SpawnerController : MonoBehaviour, IWorldObjectController<SpawnerModel>
    {
        private const string PRE_PATH = "AssetObjects/Location/CityZone/Obstacles/";
        private const string BUNDLE_NAME = "@embeded";
        [Inject]
        private DroneWorld _droneWorld;

        [Inject]
        private CreateLocationObjectService _createLocationObjectService;
        [Inject]
        private LoadLocationObjectService _loadLocationObjectService;
        public WorldObjectType ObjectType { get; set; }

        private Transform[] _spawnSpots;
        private TileDescriptor _descriptor;

        private Dictionary<Dif, float> _difficult = new Dictionary<Dif, float>() {
                {Dif.EASY, 0.6f},
                {Dif.NORMAL, 0.6f},
                {Dif.HARD, 0.6f},
        };

        public void Init(SpawnerModel model)
        {
            ObjectType = model.ObjectType;
            _descriptor = model.TileDescriptor;
            _spawnSpots = gameObject.GetComponentsOnlyInChildren<Transform>();
            SpawnObstacles();
        }

        private void SpawnObstacles()
        {
            foreach (Transform spawnSpot in _spawnSpots) {
                float sum = _difficult.Values.Sum();

                float random = Random.Range(0, sum);

                float step = 0;
                foreach (KeyValuePair<Dif, float> anyDif in _difficult) {
                    step += anyDif.Value;
                    if (random < step) {
                        string type = _descriptor.ObstacleTypes1[Random.Range(0, _descriptor.ObstacleTypes1.Length)].UnderscoreToCamelCase();
                        _loadLocationObjectService
                                .LoadResource<GameObject>(PRE_PATH + type + "/pf" + anyDif.Key.ToString().UnderscoreToCamelCase() + BUNDLE_NAME)
                                .Then(go => {
                                    GameObject instantiate = Instantiate(go, spawnSpot);
                                    instantiate.GetChildren()[Random.Range(0, instantiate.GetChildren().Count)].SetActive(true);
                                    
                                });
                        break;
                    }
                }
            }
        }
    }

    public enum Dif
    {
        EASY,
        NORMAL,
        HARD
    }
}