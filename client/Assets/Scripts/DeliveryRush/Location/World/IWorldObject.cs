using DeliveryRush.Location.Model;

namespace DeliveryRush.Location.World
{
    public interface IWorldObject
    {
        WorldObjectType ObjectType { get; }
    }
}