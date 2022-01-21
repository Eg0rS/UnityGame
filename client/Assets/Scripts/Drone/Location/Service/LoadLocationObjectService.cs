using System.Collections.Generic;
using System.Linq;
using AgkCommons.CodeStyle;
using AgkCommons.Resources;
using Drone.Descriptor;
using Drone.Levels.Descriptor;
using IoC.Attribute;
using RSG;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.Service
{
    [Injectable]
    public class LoadLocationObjectService
    {
        [Inject]
        private TileDescriptors _tileDescriptors;

        [Inject]
        private ObstacleDescriptors _obstacleDescriptors;

        [Inject]
        private ResourceService _resourceService;

        private const string BUNDLE_NAME = "@embeded";

        private List<TileDescriptor> GetTileDescriptors(LevelDescriptor descriptor)
        {
            return _tileDescriptors.Tiles.Where(tileDescriptor =>
                                                        descriptor.GameData.Tiles.TilesData.Distinct()
                                                                  .Select(tile => tile.Id)
                                                                  .ToList()
                                                                  .Exists(id => id == tileDescriptor.Id))
                                   .ToList();
        }

        private IPromise<List<T>> LoadResources<T>(string[] resoursePaths)
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

        private IPromise<T> LoadPrefab<T>(string prefabPath)
                where T : class
        {
            return _resourceService.LoadResource<T>(prefabPath);
        }
    }
}