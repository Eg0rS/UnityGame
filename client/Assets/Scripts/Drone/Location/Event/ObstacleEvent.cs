using AgkCommons.Event;

namespace Drone.Location.Event
{
    public class ObstacleEvent : GameEvent
    {
        public const string OBSTACLE_CONTACT_BEGIN = "obstacleContactBegin";
        public const string OBSTACLE_CONTACT_END = "obstacleContactEnd";

        public ObstacleEvent(string name) : base(name)
        {
            
        }
    }
}