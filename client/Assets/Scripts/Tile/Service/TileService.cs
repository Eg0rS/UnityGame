using System.Collections.Generic;
using System.Linq;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
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
        public List<GameObject> LoadedTiles { get; set; }

        public void Init()
        {
        }

        public IPromise LoadTilesByIds(params string[] ids)
        {
            LoadedTiles = new List<GameObject>();
            List<IPromise> proms = new List<IPromise>();

            foreach (string id in ids) {
                proms.Add(_resourceService.LoadPrefab(_tileDescriptors.Tiles.First(x => x.Id == id).Prefab).Then(go => LoadedTiles.Add(go)));
            }
            return Promise.All(proms);
        }
    }
}