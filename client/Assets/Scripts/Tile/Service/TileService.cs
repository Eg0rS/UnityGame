using System.Collections.Generic;
using System.Linq;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Levels.Descriptor;
using Drone.Obstacles.Service;
using IoC.Attribute;
using JetBrains.Annotations;
using RSG;
using Tile.Descriptor;
using UnityEngine;

namespace Tile.Service
{
    public class TileService : IWorldServiceInitiable
    {
        [Inject]
        private TileDescriptors _tileDescriptors;
        [Inject]
        private ResourceService _resourceService;
        
        public Dictionary<TileDescriptor, GameObject> LoadedTiles { get; set; }

        [Inject]
        public ObstaclesService ObstaclesService;

        public void Init()
        {
        }

        public IPromise LoadTilesByIds(LevelDescriptor descriptor)
        {
            List<TileDescriptor> tilesDescriptors = GetTileDescriptors(descriptor);
            LoadedTiles = new Dictionary<TileDescriptor, GameObject>();
            List<IPromise> proms = new List<IPromise>();
            foreach (TileDescriptor tileDescriptor in tilesDescriptors) {
                proms.Add(_resourceService.LoadPrefab(tileDescriptor.Prefab)
                                          .Then(tileObject => LoadedTiles.Add(tileDescriptor, tileObject)));
            }
            return Promise.All(proms);
        }

        [NotNull]
        private List<TileDescriptor> GetTileDescriptors(LevelDescriptor descriptor)
        {
            List<TileDescriptor> tilesDescriptor = new List<TileDescriptor>();
            List<string> tileIds = descriptor.GameData.Tiles.TilesData.Select(tile => tile.Id).Distinct().ToList();
            tileIds.ForEach(id => tilesDescriptor.Add(_tileDescriptors.Tiles.First(x => x.Id == id)));
            return tilesDescriptor;
        }
    }
}