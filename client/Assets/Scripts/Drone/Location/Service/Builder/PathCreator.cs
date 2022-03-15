using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Location.Model.Obstacle;
using Drone.Random.MersenneTwister;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class PathCreator
    {
        private const float MAX_DELTA = 2.0f;
        private readonly MTRandomGenerator _randomGenerator;
        private readonly List<ObstacleInfo> _obstacleInfos;

        private bool _isShotest;

        public PathCreator(List<ObstacleInfo> obstacleInfos, uint seed)
        {
            _obstacleInfos = obstacleInfos;
            _randomGenerator = new MTRandomGenerator(seed);
        }

        [NotNull]
        public List<Vector3> GetPath(Transform start, Transform finish, bool isShortest = false)
        {
            List<Vector3> points = new List<Vector3> {
                    Vector3.zero
            };
            for (int z = (int) start.position.z; z < (int) finish.position.z; z++) {
                Vector3 curPoint = new Vector3(points.Last().x, points.Last().y, z);
                points.Add(ConfigPoint(curPoint));
            }
            return points;
        }

        private Vector3 ConfigPoint(Vector3 point)
        {
            float minDelta = MAX_DELTA;
            List<Vector3> clearCells = new List<Vector3>();
            Vector3 shortCord = new Vector3(0, 0, point.z);
            ObstacleInfo info = IsClearCell(point);
            if (info == null) {
                return point;
            }
            Vector2 pointPosition = new Vector2(point.x, point.y);
            for (int i = 0; i < 9; i++) {
                if (info.PassThroughGrid._cellDatas[i]._isFilled) {
                    continue;
                }
                if (_isShotest) {
                    float delta = Math.Abs(Vector2.Distance(pointPosition, info.PassThroughGrid._cellDatas[i].Coords));
                    if (!(delta < minDelta)) {
                        continue;
                    }
                    minDelta = delta;
                    shortCord.x = info.PassThroughGrid._cellDatas[i].Coords.x;
                    shortCord.y = info.PassThroughGrid._cellDatas[i].Coords.y;
                } else {
                    Vector3 clearCell = new Vector3(info.PassThroughGrid._cellDatas[i].Coords.x, info.PassThroughGrid._cellDatas[i].Coords.y,
                                                    point.z);
                    clearCells.Add(clearCell);
                }
            }
            return _isShotest ? shortCord : clearCells[_randomGenerator.Range(0, clearCells.Count)];
        }

        [CanBeNull]
        private ObstacleInfo IsClearCell(Vector3 position)
        {
            return _obstacleInfos.FirstOrDefault(x => Math.Abs(x.transform.position.z - position.z) < 1f);
        }
    }
}