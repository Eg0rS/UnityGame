using System.Collections.Generic;
using System.Linq;
using Drone.LevelDifficult.Descriptor;
using Drone.Location.Model.Spawner;
using Tile.Descriptor;
using UnityEngine;

namespace Drone.Location.World
{
    public class WorldTile : WorldObjectExtension
    {
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
        private SpawnerModel _spawner;
        private float _spawnStep;

        public void Init(TileDescriptor tileDescriptor, GameObject spot, DifficultDescriptor difficultDescriptor)
        {
            _descriptor = tileDescriptor;
            _spot = spot;
            List<GameObject> allObjects = GetInnerObjects();
            _begin = allObjects.Find(x => x.name == "Begin").transform;
            _end = allObjects.Find(x => x.name == "End").transform;
            _spawner = gameObject.GetComponentInChildren<SpawnerModel>();
            _spawner.TileDescriptor = _descriptor;
            _spawner.Diffcult = difficultDescriptor;
            _spawnStep = difficultDescriptor.SpawnStep;
        }

        public void Configure()
        {
            Vector3 step = new Vector3(0, 0, _spawnStep);
            Vector3 pos = Vector3.zero;
            while (pos.magnitude <= _end.localPosition.magnitude) {
                TileZone flag = _descriptor.RedZone?.FirstOrDefault(x => pos.magnitude >= x.Begin && pos.magnitude <= x.End);
                if (flag != null) {
                    pos = new Vector3(0, 0, flag.End);
                }
                if ((pos + step).magnitude <= _end.localPosition.magnitude || flag == null) {
                    GameObject instSpot = Instantiate(_spot, _spawner.transform);
                    instSpot.transform.localPosition = pos;
                }
                pos += step;
            }
        }
    }
}