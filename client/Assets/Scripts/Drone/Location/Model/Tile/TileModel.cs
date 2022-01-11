using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Tile
{
    public class TileModel : PrefabModel
    {
        private void Awake()
        {
            ObjectType = WorldObjectType.TILE;  
        }
    }
}