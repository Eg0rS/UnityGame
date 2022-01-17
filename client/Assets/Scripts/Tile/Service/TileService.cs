using System.Collections.Generic;
using System.Linq;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Obstacles.Service;
using IoC.Attribute;
using RSG;
using UnityEngine;

namespace Tile.Service
{
    public class TileService : IWorldServiceInitiable
    {
        [Inject]
        private TileDescriptors _tileDescriptors;
        [Inject]
        private ResourceService _resourceService;
        public Dictionary<string, GameObject> LoadedTiles { get; set; }

        [Inject]
        public ObstaclesService ObstaclesService;

        public void Init()
        {
        }

        public IPromise LoadTilesByIds(params string[] ids)
        {
            ids = ids.Distinct().ToArray();
            LoadedTiles = new Dictionary<string, GameObject>();
            List<IPromise> proms = new List<IPromise>();

            foreach (string id in ids) {
                proms.Add(_resourceService.LoadPrefab(_tileDescriptors.Tiles.First(x => x.Id == id).Prefab).Then(go => LoadedTiles.Add(id, go)));
            }
            return Promise.All(proms);
        }

        public TileDescriptors TileDescriptors
        {
            get { return _tileDescriptors; }
        }
    }
}