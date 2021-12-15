using Drone.Location.Model;

namespace Drone.Location.Service.Control
{
    public interface IWorldObject
    {
        WorldObjectType ObjectType { get; }
    }
}