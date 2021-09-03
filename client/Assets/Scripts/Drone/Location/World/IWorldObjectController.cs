using Drone.Location.Model.BaseModel;
using JetBrains.Annotations;

namespace Drone.Location.World
{
    public interface IWorldObjectController<in T>: IWorldObject
        where T : PrefabModel
    {
        [UsedImplicitly]
            void Init(T model);
    }
}