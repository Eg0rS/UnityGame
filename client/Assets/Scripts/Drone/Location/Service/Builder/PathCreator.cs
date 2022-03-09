using System;
using System.Collections.Generic;
using BezierSolution;
using Drone.Location.Model.Obstacle;
using Drone.Random.MersenneTwister;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class PathCreator : MonoBehaviour
    {
        private List<ObstacleInfo> _obstacleInfos;

        private BezierSpline _bezierSpline;
        private Vector3 _pointPosition = Vector3.zero;
        private bool _isShotest;
        private MTRandomGenerator _randomGenerator;

        public BezierSpline Init(List<ObstacleInfo> obstacleInfos, bool shotest = true, uint seed = 0)
        {
            _isShotest = shotest;
            _randomGenerator = new MTRandomGenerator(seed);
            _bezierSpline = gameObject.AddComponent<BezierSpline>();
            _obstacleInfos = obstacleInfos;
            CreatePoint();
            foreach (ObstacleInfo obstacle in _obstacleInfos) {
                _pointPosition = new Vector3(0, 0, obstacle.gameObject.transform.position.z);
                //Vector2 cord = PathCalculated(obstacle.PassThroughGrid);
                Vector2 cord = Vector2.down;
                _pointPosition = new Vector3(cord.x, cord.y, _pointPosition.z);
                CreatePoint();
                _pointPosition += new Vector3(0, 0, obstacle.Depth);
                CreatePoint();
            }
            _bezierSpline.ConstructLinearPath();
            transform.position = new Vector3(0, 1.5f, 0);
            return _bezierSpline;
        }

        private void CreatePoint()
        {
            GameObject bPoint = new GameObject("point");
            bPoint.AddComponent<BezierPoint>();
            bPoint.transform.SetParent(transform);
            bPoint.transform.position = _pointPosition;
        }

        private Vector2 PathCalculated(PassThroughGrid grid)
        {
            List<Vector2> clearCells = new List<Vector2>();
            const float MAXDELTA = 2.0f;
            int selectIndex = 0;
            float minDelta = MAXDELTA;
            for (int i = 0; i < 9; i++) {
                if (grid._cellDatas[i]._isFilled) {
                    continue;
                }
                if (_isShotest) {
                    Vector2 pp = new Vector2(_pointPosition.x, _pointPosition.y);
                    float delta = Math.Abs(Vector2.Distance(pp, grid._cellDatas[i].Coords));
                    if (!(delta < minDelta)) {
                        continue;
                    }
                    minDelta = delta;
                    selectIndex = i;
                } else {
                    clearCells.Add(grid._cellDatas[i].Coords);
                }
            }
            return _isShotest ? grid._cellDatas[selectIndex].Coords : clearCells[_randomGenerator.Range(0, clearCells.Count)];
        }
    }
}