using UnityEngine;

namespace Drone.Location.Model.Obstacle
{
    public class ObstacleInfo : MonoBehaviour
    {
        [SerializeField]
        private PassThroughGrid _passThroughGrid;
        [SerializeField]
        [Range(0, 20)]
        private int _depth;

        public PassThroughGrid PassThroughGrid
        {
            get { return _passThroughGrid; }
        }
        public int Depth
        {
            get { return _depth; }
        }
    }

    [System.Serializable]
    public class ColumnsData
    {
        public bool[] columns;
    }

    [System.Serializable]
    public class PassThroughGrid
    {
        public ColumnsData[] rows = new ColumnsData[3];
    }
}