using System.Collections.Generic;
using JetBrains.Annotations;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.World
{
    public class WorldTile : WorldObjectExtension
    {
        public Transform Begin
        {
            get { return _begin; }
        }
        public Transform End
        {
            get { return _end; }
        }
        public TileDescriptor Descriptor
        {
            get { return _descriptor; }
        }
        
        private TileDescriptor _descriptor;
        private Transform _begin;
        private Transform _end;

        [NotNull]
        public WorldTile Init(TileDescriptor tileDescriptor, WorldTile preTile)
        {
            _descriptor = tileDescriptor;
            List<GameObject> allObjects = GetInnerObjects();
            _begin = allObjects.Find(x => x.name == "Begin").transform;
            _end = allObjects.Find(x => x.name == "End").transform;
            if (preTile != null) {
                transform.position = preTile.End.position - preTile.Begin.localPosition;
            }
            return this;
        }
    }
}