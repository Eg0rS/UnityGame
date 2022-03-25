using AgkCommons.Extension;
using Drone.Location.Service.Game.Event;
using Drone.Location.World.Player.Service;
using IoC.Attribute;
using IoC.Extension;
using UnityEngine;
using UnityEngine.Playables;

namespace Drone.Location.World.CutScene
{
    public class CutSceneController : MonoBehaviour
    {
        private GameObject _mesh;
        [Inject]
        private DroneWorld _droneWorld;
        [Inject]
        private DroneService _droneService;
        private PlayableDirector _director;
        public CutSceneType Type { get; set; }

        public void Init(CutSceneType type)
        {
            Type = type;
            gameObject.SetActive(false);
            _director = GetComponent<PlayableDirector>();
            _director.stopped += OnEndCutscene;
            this.Inject();
            _mesh = gameObject.GetChildren().Find(x => x.name == "CutSceneMeshDrone");
            GameObject drone =
                    Instantiate(Resources.Load<GameObject>(_droneService.GetDronById(_droneService.SelectedDroneId).DroneDescriptor.Prefab));
            _droneWorld.AddGameObject(drone, _mesh);
        }

        private void OnEndCutscene(PlayableDirector obj)
        {
            gameObject.SetActive(false);
            _droneWorld.Dispatch(new InGameEvent(InGameEvent.CUTSCENE_END, Type));
        }

        public void StartCutScene()
        {
            gameObject.SetActive(true);
        }
    }
}