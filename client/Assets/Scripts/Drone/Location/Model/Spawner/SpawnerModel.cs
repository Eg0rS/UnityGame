using Drone.LevelDifficult.Descriptor;
using Drone.Location.Model.BaseModel;
using Tile.Descriptor;

namespace Drone.Location.Model.Spawner
{
    public class SpawnerModel : PrefabModel
    {
        public TileDescriptor TileDescriptor { get; set; }
        public DifficultDescriptor Diffcult { get; set; }

        public void Awake()
        {
            ObjectType = WorldObjectType.SPAWNER;
        }
    }
}