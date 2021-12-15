using Drone.Location.Model.BaseModel;
using JetBrains.Annotations;

namespace Drone.Location.Service.Control
{
    public interface IWorldObjectController<in T> : IWorldObject
            where T : PrefabModel
    {
        [UsedImplicitly]
        void Init(T model);
    }
}