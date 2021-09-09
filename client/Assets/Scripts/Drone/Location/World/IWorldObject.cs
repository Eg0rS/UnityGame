using Drone.Location.Model;

namespace Drone.Location.World
{
    public interface IWorldObject
    {
        WorldObjectType ObjectType { get; }
    }
}