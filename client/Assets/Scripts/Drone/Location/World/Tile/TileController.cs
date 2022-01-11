using Drone.Location.Model;
using Drone.Location.Model.Tile;
using UnityEngine;

namespace Drone.Location.World.Tile
{
    public class TileController : MonoBehaviour, IWorldObjectController<TileModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(TileModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}