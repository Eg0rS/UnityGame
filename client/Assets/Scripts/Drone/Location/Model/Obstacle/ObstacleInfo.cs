using System.Collections.Generic;
using UnityEngine;

namespace Drone.Location.Model.Obstacle
{
    public class ObstacleInfo : MonoBehaviour
    {
        [SerializeField]
        public List<PassThroughGrid> PassThroughGrids = new List<PassThroughGrid>();
        [SerializeField]
        [Min(0)]
        public int Depth;
    }

    [System.Serializable]
    public class PassThroughGrid
    {
        [SerializeField]
        public CellData[] _cellDatas = new CellData[9];

        public PassThroughGrid()
        {
            _cellDatas[0] = new CellData(new Vector2(-1, 1), true);
            _cellDatas[1] = new CellData(new Vector2(0, 1), true);
            _cellDatas[2] = new CellData(new Vector2(1, 1), true);
            _cellDatas[3] = new CellData(new Vector2(-1, 0), true);
            _cellDatas[4] = new CellData(new Vector2(0, 0), true);
            _cellDatas[5] = new CellData(new Vector2(1, 0), true);
            _cellDatas[6] = new CellData(new Vector2(-1, -1), true);
            _cellDatas[7] = new CellData(new Vector2(0, -1), true);
            _cellDatas[8] = new CellData(new Vector2(1, -1), true);
        }
    }

    [System.Serializable]
    public class CellData
    {
        private Vector2 _coords;
        [SerializeField]
        public bool _isFilled;

        public CellData(Vector2 coords, bool isFilled)
        {
            _coords = coords;
            _isFilled = isFilled;
        }

        public Vector2 Coords
        {
            get { return _coords; }
        }
    }
}