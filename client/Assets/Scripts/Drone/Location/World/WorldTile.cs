using System.Collections.Generic;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.World
{
    public class WorldTile : WorldObjectExtension
    {
        public TileDescriptor Descriptor { get; set; }
        public Dictionary<GameObject, int> Obstacles { get; set; }
        

        public WorldTile(TileDescriptor descriptor)
        {
            Descriptor = descriptor;
        }
    }
}