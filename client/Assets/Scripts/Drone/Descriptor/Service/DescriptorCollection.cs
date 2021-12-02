using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drone.Descriptor.Loader.Interfaces;
using JetBrains.Annotations;

namespace Drone.Descriptor.Service
{
    public class DescriptorCollection<T> : IEnumerable, IDescriptorCollection
    {
        
        [PublicAPI]
        public string Name { get; }

        private readonly Dictionary<string, T> _map;

        public DescriptorCollection(string name)
        {
            Name = name;
            _map = new Dictionary<string, T>();
        }

        [PublicAPI]
        public T Require(string id)
        {
            if (!_map.ContainsKey(id)) {
                throw new NullReferenceException($"Descriptor with id={id} of collection={Name} not found");
            }
            return _map[id];
        }

        [CanBeNull]
        [PublicAPI]
        public T Get(string id)
        {
            return _map.ContainsKey(id) ? _map[id] : default;
        }

        [PublicAPI]
        public List<T> Values
        {
            get { return _map.Values.ToList(); }
        }

        [PublicAPI]
        public bool Contains(string id)
        {
            return _map.ContainsKey(id);
        }

        [PublicAPI]
        public int Size
        {
            get { return _map.Count; }
        }

        [PublicAPI]
        public void Put(string id, T descriptor)
        {
            if (_map.ContainsKey(id)) {
                throw new ArgumentException("Descriptor with id=" + id + " in collection=" + Name + " already exists");
            }
            _map[id] = descriptor;
        }

        [PublicAPI]
        public void PutAll(Dictionary<string, T> map)
        {
            foreach (KeyValuePair<string, T> pair in map) {
                Put(pair.Key, pair.Value);
            }
        }

        public void PutAll(Dictionary<string, object> map)
        {
            foreach (KeyValuePair<string, object> pair in map) {
                Put(pair.Key, (T) pair.Value);
            }
        }

        public void Clear()
        {
            _map.Clear();
        }

        // IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            return _map.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}