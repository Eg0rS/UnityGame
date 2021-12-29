using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Spline;
using UnityEngine;

namespace Drone.Location.Service.Control.Spline
{
    public class SplineController : MonoBehaviour, IWorldObjectController<SplineModel>
    {
        public WorldObjectType ObjectType { get; }
        private BezierSpline _bezierSpline;

        public void Init(SplineModel model)
        {
            _bezierSpline = gameObject.AddComponent<BezierSpline>();
            _bezierSpline.Refresh();
        }

        public BezierSpline BezierSpline
        {
            get { return _bezierSpline; }
        }
    }
}