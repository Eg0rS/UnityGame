using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Descriptor.Loader.Interfaces;
using JetBrains.Annotations;

namespace Drone.Descriptor.Service
{
    public class DescriptorRegistry
    {
        private static readonly Lazy<DescriptorRegistry> _instance = new Lazy<DescriptorRegistry>(() => new DescriptorRegistry());

        private readonly Dictionary<string, object> _collections = new Dictionary<string, object>();
        private readonly List<object> _descriptors = new List<object>();

        private DescriptorRegistry()
        {
        }

        public T RequireSingleDescriptor<T>()
                where T : class
        {
            T descriptor = (T) GetSingleDescriptor(typeof(T));
            if (descriptor == null) {
                throw new NullReferenceException("descriptor " + typeof(T) + " not found");
            }
            return descriptor;
        }

        [CanBeNull]
        public T GetSingleDescriptor<T>()
                where T : class
        {
            return (T) GetSingleDescriptor(typeof(T));
        }

        [CanBeNull]
        [PublicAPI]
        public object GetSingleDescriptor(Type t)
        {
            return _descriptors.FirstOrDefault(t.IsInstanceOfType);
        }

        [PublicAPI]
        public bool ContainsSingleDescriptor<T>()
                where T : class
        {
            return ContainsSingleDescriptor(typeof(T));
        }

        [PublicAPI]
        public bool ContainsSingleDescriptor(Type t)
        {
            return GetSingleDescriptor(t) != null;
        }

        public DescriptorCollection<T> CreateCollection<T>(string collectionName)
        {
            DescriptorCollection<T> collection = new DescriptorCollection<T>(collectionName);
            _collections.Add(collectionName, collection);
            return collection;
        }

        public DescriptorCollection<T> RequireCollection<T>(string collectionName)
        {
            if (!ContainsCollection(collectionName)) {
                throw new NullReferenceException("Collection with name=" + collectionName + " not found.");
            }
            return (DescriptorCollection<T>) _collections[collectionName];
        }

        [CanBeNull]
        public IDescriptorCollection GetCollection(string collectionName)
        {
            if (!_collections.ContainsKey(collectionName)) {
                return null;
            }

            return (IDescriptorCollection) _collections[collectionName];
        }

        [PublicAPI]
        public bool ContainsCollection(string collectionName)
        {
            return _collections.ContainsKey(collectionName);
        }

        public void AddSingleDescriptor(object descriptor)
        {
            if (descriptor == null) {
                throw new ArgumentNullException(nameof(descriptor));
            }
            if (ContainsSingleDescriptor(descriptor.GetType())) {
                throw new ArgumentException("descriptor " + descriptor.GetType().Name + " already added");
            }
            _descriptors.Add(descriptor);
        }

        public void ClearSingleDescriptors()
        {
            _descriptors.Clear();
        }

        public static DescriptorRegistry Instance
        {
            get { return _instance.Value; }
        }

        [PublicAPI]
        public void Clear()
        {
            _descriptors.Clear();
            _collections.Clear();
        }
    }
}