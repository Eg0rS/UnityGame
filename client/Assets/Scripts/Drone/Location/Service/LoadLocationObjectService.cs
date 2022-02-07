using System.Collections.Generic;
using System.Linq;
using AgkCommons.CodeStyle;
using AgkCommons.Resources;
using Drone.Descriptor;
using Drone.Levels.Descriptor;
using Drone.Obstacles;
using Drone.Obstacles.Descriptor;
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
        [Inject]
        private TileDescriptors _tileDescriptors;

        [Inject]
        private ObstacleDescriptors _obstacleDescriptors;

        [Inject]
        private ResourceService _resourceService;

        // public IPromise<Dictionary<ObstacleType, Dictionary<GameObject, int>>> LoadLevelObstacles(LevelDescriptor descriptor)
        // {
        //     Dictionary<ObstacleType, Dictionary<GameObject, int>> levelObstacles = new Dictionary<ObstacleType, Dictionary<GameObject, int>>();
        //     List<IPromise> promises = new List<IPromise>();
        //     List<ObstacleType> obstacleTypes = GetUniqueObstacleTypes(GetUniqueTileDescriptors(descriptor));
        //     foreach (ObstacleType obstacleType in obstacleTypes) {
        //         levelObstacles[obstacleType] = new Dictionary<GameObject, int>();
        //         List<ObstacleDescriptor> obstacleDescriptors =
        //                 _obstacleDescriptors.Obstacles.Where(obstacleDescriptor => obstacleDescriptor.Type == obstacleType).ToList();
        //         foreach (ObstacleDescriptor obstacleDescriptor in obstacleDescriptors) {
        //             promises.Add(LoadResource<GameObject>(obstacleDescriptor.Prefab)
        //                                  .Then(loadedObject => { levelObstacles[obstacleType].Add(loadedObject, 0); }));
        //         }
        //     }
        //     return Promise.All(promises).Then(() => Promise<Dictionary<ObstacleType, Dictionary<GameObject, int>>>.Resolved(levelObstacles));
        // }

        public IPromise<Dictionary<TileDescriptor, GameObject>> LoadLevelTiles(LevelDescriptor descriptor)
        {
            List<IPromise> promises = new List<IPromise>();
            List<TileDescriptor> tileDescriptors = GetUniqueTileDescriptors(descriptor);
            Dictionary<TileDescriptor, GameObject> tiles = new Dictionary<TileDescriptor, GameObject>();
            foreach (TileDescriptor tileDescriptor in tileDescriptors) {
                Promise loadPromise = new Promise();
                promises.Add(loadPromise);
                _resourceService.LoadResource<GameObject>(tileDescriptor.Prefab)
                                .Then((go) => {
                                    tiles[tileDescriptor] = go;
                                    loadPromise.Resolve();
                                })
                                .Catch(e => loadPromise.Reject(e))
                                .Done();
            }
            return Promise.All(promises).Then(() => Promise<Dictionary<TileDescriptor, GameObject>>.Resolved(tiles));
        }

        [NotNull]
        private List<ObstacleType> GetUniqueObstacleTypes(List<TileDescriptor> uniqueTiles)
        {
            List<ObstacleType> obstacleTypes = new List<ObstacleType>();
            foreach (TileDescriptor tileDescriptor in uniqueTiles) {
                obstacleTypes.AddRange(tileDescriptor.ObstacleTypes);
            }
            return obstacleTypes.Distinct().ToList();
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
    }
}