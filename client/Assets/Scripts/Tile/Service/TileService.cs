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

        private List<GameObject> _loadedTiles = new List<GameObject>();

        public void Init()
        {
        }

        public IPromise LoadTilesByIds(params string[] ids)
        {
            _loadedTiles = new List<GameObject>();
            List<IPromise> proms = ids
                                   .Select(id => _resourceService.LoadPrefab(_tileDescriptors.Tiles.First(x => x.Id == id).Prefab)
                                                                 .Then(x => _loadedTiles.Add(x)))
                                   .ToList();

            return Promise.All(proms);
        }

        public List<GameObject> LoadedTiles
        {
            get { return _loadedTiles; }
        }
    }
}