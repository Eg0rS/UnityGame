using System.Collections.Generic;
using System.Linq;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.World
{
    public class WorldTile : WorldObjectExtension
    {
        private const int SPAWN_STEP = 20;
        public Transform Begin
        {
            get { return _begin; }
        }
        public Transform End
        {
            get { return _end; }
        }
        private TileDescriptor _descriptor;
        private Transform _begin;
        private Transform _end;
        private GameObject _spot;
        private Transform _obstacles;

        public void Init(TileDescriptor tileDescriptor, GameObject spot)
        {
            _descriptor = tileDescriptor;
            _spot = spot;
            List<GameObject> allObjects = GetInnerObjects();
            _begin = allObjects.Find(x => x.name == "Begin").transform;
            _end = allObjects.Find(x => x.name == "End").transform;
            _obstacles = allObjects.Find(x => x.gameObject.name == "Obstacle").transform;
        }

        public void Configure()
        {
            Vector3 step = new Vector3(0, 0, SPAWN_STEP);
            Vector3 pos = Vector3.zero;
            while (pos.magnitude <= _end.localPosition.magnitude) {
                DeadZone flag = _descriptor.DeadZones?.FirstOrDefault(x => pos.magnitude >= x.Begin && pos.magnitude <= x.End);
                if (flag != null) {
                    pos = new Vector3(0, 0, flag.End);
                }
                if ((pos + step).magnitude <= _end.localPosition.magnitude || flag == null) {
                    GameObject instSpot = Instantiate(_spot, _obstacles);
                    instSpot.transform.localPosition = pos;
                }
                pos += step;
            }
        }
    }
}