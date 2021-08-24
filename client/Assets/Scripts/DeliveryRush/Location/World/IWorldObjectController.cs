using DeliveryRush.Location.Model.BaseModel;
using JetBrains.Annotations;

namespace DeliveryRush.Location.World
{
    public interface IWorldObjectController<in T>: IWorldObject
        where T : PrefabModel
    {
        [UsedImplicitly]
            void Init(T model);
    }
}