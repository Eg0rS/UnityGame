using System.Collections.Generic;
using BezierSolution;
using Drone.Location.Model.Obstacle;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class PathCreator : MonoBehaviour
    {
        private List<ObstacleInfo> _obstacleInfos;

        private BezierSpline _bezierSpline;
        private Vector3 _pointPosition = Vector3.zero;

        public void Init(List<ObstacleInfo> obstacleInfos)
        {
            _bezierSpline = gameObject.AddComponent<BezierSpline>();
            _obstacleInfos = obstacleInfos;
            CreatePoint();
            foreach (ObstacleInfo obstacle in _obstacleInfos) {
                _pointPosition += new Vector3(0, 0, obstacle.gameObject.transform.position.z);
                int[] cord = PassCalculated(obstacle.PassThroughGrid);
                _pointPosition += new Vector3(cord[0], cord[1], 0);
                CreatePoint();
                _pointPosition += new Vector3(0, 0, obstacle.Depth);
                CreatePoint();
            }
            _bezierSpline.ConstructLinearPath();
        }

        private void CreatePoint()
        {
            GameObject bPoint = new GameObject("point");
            bPoint.AddComponent<BezierPoint>();
            bPoint.transform.SetParent(transform);
            bPoint.transform.position = _pointPosition;
        }

        private int[] PassCalculated(PassThroughGrid grid)
        {
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 2; j++) {
                    if (grid.rows[i].columns[j] == false) {
                        return new[] {i, j};
                    }
                }
            }
            return null;
        }
    }
}