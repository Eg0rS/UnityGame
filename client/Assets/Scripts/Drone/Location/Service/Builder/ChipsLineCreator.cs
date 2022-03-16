using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Location.Model.Obstacle;
using RSG;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class ChipsLineCreator : MonoBehaviour
    {
        private int _chipCount;
        private const string CHIP_PATH = "AssetObjects/Zones/Common/UI/Chip/pfChip@embeded";
        private PathCreator _pathCreator;
        private List<Vector3> _path;
        private Transform _start;
        private Transform _finish;

        public IPromise Init(List<ObstacleInfo> obstacleInfos,
                             uint seed,
                             int chipsCount,
                             LoadLocationObjectService objectService,
                             Transform start,
                             Transform finish)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            _start = start;
            _finish = finish;
            _chipCount = chipsCount;
            _pathCreator = new PathCreator(obstacleInfos, seed);
            _path = _pathCreator.GetPath(start, finish);
            return objectService.LoadResource<GameObject>(CHIP_PATH).Then(CreateChipsPath);
        }

        private void CreateChipsPath(GameObject chip)
        {
            int start = (int) _start.position.z + 10;
            int end = (int) _finish.position.z - 10;
            int delta = (end - start) / _chipCount;
            for (float normalT = start; normalT <= end; normalT += delta) {
                Vector3 position = _path.First(p => Math.Abs(p.z - normalT) < 1);
                GameObject intsChip = Instantiate(chip, transform);
                intsChip.transform.localPosition = position;
            }
        }
    }
}