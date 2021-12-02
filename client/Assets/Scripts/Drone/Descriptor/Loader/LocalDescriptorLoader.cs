using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Resources;
using Drone.Descriptor.Service;
using IoC.Attribute;
using RSG;
using UnityEngine;
using static CsPreconditions.Preconditions;
namespace Drone.Descriptor.Loader
{
    public class LocalDescriptorLoader : AbstactDescriptorLoader
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocalDescriptorLoader>();

        private const string DESCRIPTORS_PATH = "Descriptor/";

        [Inject]
        private ResourceService _resourceService;

        public override IPromise Load()
        {
            DescriptorRegistry registry = DescriptorRegistry.Instance;
            return LoadDescriptors(registry).Then(() => LoadCollections(registry));
        }

        private IPromise LoadDescriptors(DescriptorRegistry registry)
        {
            registry.ClearSingleDescriptors();
            List<IPromise> promises = new List<IPromise>();
            foreach (KeyValuePair<string, Type> descriptor in Descriptors) {
                promises.Add(LoadTextAsset(descriptor.Key)
                             .Then((asset => registry.AddSingleDescriptor(CreateSingleDescriptor(asset.text, descriptor.Value))))
                             .Catch(e => {
                                 _logger.Error($"Can't parse descriptor {descriptor.Key}", e);
                                 throw e;
                             }));
            }
            return Promise.All(promises);
        }

        private IPromise LoadCollections(DescriptorRegistry registry)
        {
            List<IPromise> promises = new List<IPromise>();
            foreach (KeyValuePair<string, Type> collection in Collections) {
                string collectionName = collection.Key;
                promises.Add(LoadTextAsset(collectionName)
                             .Then((asset => FillCollection(asset.text, collection.Value,
                                                            CheckNotNull(registry.GetCollection(collectionName)))))
                             .Catch(e => {
                                 _logger.Error($"Descriptor {collection.Key} can't be parsed", e);
                                 throw e;
                             }));
            }
            return Promise.All(promises);
        }

        private IPromise<TextAsset> LoadTextAsset(string name)
        {
            string path = DESCRIPTORS_PATH + name + "@embeded";
            IPromise<TextAsset> promise = _resourceService.LoadResource<TextAsset>(path);
            return promise;
        }
    }
}