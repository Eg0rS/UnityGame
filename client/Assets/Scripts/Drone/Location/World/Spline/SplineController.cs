using Drone.Location.Model;
using Drone.Location.Model.Spline;
using Drone.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Spline
{
    public class SplineController : MonoBehaviour, IWorldObjectController<SplineModel>
    {
        public WorldObjectType ObjectType { get; }
        [Inject]
        private GameWorld _gameWorld;

        public void Init(SplineModel model)
        {
            gameObject.transform.SetParent(_gameWorld.RequireGameObjectByName("Spline").transform, false);
        }
    }
}