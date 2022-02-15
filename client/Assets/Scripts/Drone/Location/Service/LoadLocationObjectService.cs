using System.Collections.Generic;
using System.Linq;
using AgkCommons.CodeStyle;
using AgkCommons.Extension;
using AgkCommons.Resources;
using Drone.Descriptor;
using Drone.Levels.Descriptor;
using Drone.Location.World.Spawner;
using IoC.Attribute;
using JetBrains.Annotations;
using RSG;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.Service
{
    [Injectable]
    public class LoadLocationObjectService
    {
        private const string PRE_PATH = "AssetObjects/Location/Zones/";
        private const string BUNDLE_NAME = "@embeded";
        private const string TILE = "/Tile/";
        private const string OBSTACLE = "/Obstacle/";

        [Inject]
        private TileDescriptors _tileDescriptors;

        [Inject]
        private ResourceService _resourceService;

        private Dictionary<string, GameObject> _loadedCache = new Dictionary<string, GameObject>();

        public IPromise<Dictionary<TileDescriptor, GameObject>> LoadLevelTiles(LevelDescriptor descriptor)
        {
            List<IPromise> promises = new List<IPromise>();
            List<TileDescriptor> tileDescriptors = GetUniqueTileDescriptors(descriptor);
            Dictionary<TileDescriptor, GameObject> tiles = new Dictionary<TileDescriptor, GameObject>();
            foreach (TileDescriptor tileDescriptor in tileDescriptors) {
                Promise loadPromise = new Promise();
                promises.Add(loadPromise);
                string path = PRE_PATH + tileDescriptor.Zone.ToString().UnderscoreToCamelCase() + TILE + tileDescriptor.Prefab + BUNDLE_NAME;
                LoadGameObject(path)
                        .Then(go => {
                            tiles[tileDescriptor] = go;
                            loadPromise.Resolve();
                        })
                        .Catch(e => loadPromise.Reject(e))
                        .Done();
            }
            return Promise.All(promises).Then(() => Promise<Dictionary<TileDescriptor, GameObject>>.Resolved(tiles));
        }

        [NotNull]
        private List<TileDescriptor> GetUniqueTileDescriptors(LevelDescriptor descriptor)
        {
            return _tileDescriptors.Tiles.Where(tileDescriptor =>
                                                        descriptor.GameData.Tiles.TilesData.Distinct()
                                                                  .Select(tile => tile.Id)
                                                                  .ToList()
                                                                  .Exists(id => id == tileDescriptor.Id))
                                   .ToList();
        }

        public IPromise<List<T>> LoadResources<T>(string[] resoursePaths)
                where T : class

        {
            List<T> loadResources = new List<T>();
            List<IPromise> promises = new List<IPromise>();
            foreach (string prefabPath in resoursePaths) {
                Promise loadPromise = new Promise();
                promises.Add(loadPromise);
                _resourceService.LoadResource<T>(prefabPath)
                                .Then((resource) => {
                                    loadResources.Add(resource);
                                    loadPromise.Resolve();
                                })
                                .Catch(e => loadPromise.Reject(e))
                                .Done();
            }
            return Promise.All(promises).Then(() => Promise<List<T>>.Resolved(loadResources));
        }

        public IPromise<T> LoadResource<T>(string prefabPath)
                where T : class
        {
            return _resourceService.LoadResource<T>(prefabPath);
        }

        public IPromise<GameObject> LoadGameObject(string prefabPath)
        {
            Promise<GameObject> promise = new Promise<GameObject>();
            if (_loadedCache.ContainsKey(prefabPath)) {
                promise.Resolve(_loadedCache[prefabPath]);
                return promise;
            }
            return _resourceService.LoadResource<GameObject>(prefabPath).Then(go => _loadedCache[prefabPath] = go);
        }

        public IPromise<GameObject> LoadObstacle(TileDescriptor tileDescriptor, string obstacleType, LevelType obstacleDifficult)
        {
            string path = PRE_PATH + tileDescriptor.Zone.ToString().UnderscoreToCamelCase() + OBSTACLE + obstacleType + "/pf"
                          + obstacleDifficult.ToString().UnderscoreToCamelCase() + BUNDLE_NAME;
            return LoadGameObject(path);
        }
    }
}