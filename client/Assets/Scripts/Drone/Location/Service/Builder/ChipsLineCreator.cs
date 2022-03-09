using System.Collections.Generic;
using BezierSolution;
using Drone.Location.Model.Obstacle;
using RSG;
using UnityEngine;

namespace Drone.Location.Service.Builder
{
    public class ChipsLineCreator : MonoBehaviour
    {
        private int _chipCount;
        private const string CHIP_PATH = "AssetObjects/Zones/Common/UI/Chip/pfChip@embeded";
        private BezierSpline _bezierSpline;

        public IPromise Init(List<ObstacleInfo> obstacleInfos, uint seed, int chipsCount, LoadLocationObjectService objectService)
        {
            _chipCount = chipsCount;
            _bezierSpline = gameObject.AddComponent<PathCreator>().Init(obstacleInfos, false, seed);
            return objectService.LoadResource<GameObject>(CHIP_PATH).Then(CreateChipsPath);
        }

        private void CreateChipsPath(GameObject chip)
        {
            float start = 0.02f;
            float end = 0.98f;
            float delta = (end - start) / _chipCount;
            for (float normalT = start; normalT <= end; normalT += delta) {
                Vector3 position = _bezierSpline.GetPoint(normalT);
                GameObject intsChip = Instantiate(chip, gameObject.transform);
                intsChip.transform.position = position;
            }
        }
    }
}