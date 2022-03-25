using System.Collections.Generic;
using AgkCommons.Extension;
using Drone.Location.Event;
using Drone.Location.Model;
using Drone.Location.Model.CutScene;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;
using RSG.Promises;
using UnityEngine;

namespace Drone.Location.World.CutScene
{
    public class CutScenesController : MonoBehaviour, IWorldObjectController<CutScenesModel>
    {
        private List<CutSceneController> _cutScenes = new List<CutSceneController>();

        [Inject]
        private DroneWorld _droneWorld;

        public void Init(CutScenesModel model)
        {
            gameObject.GetChildren()
                      .Each(x => {
                          CutScene cutScene = x.GetComponent<CutScene>();
                          CutSceneController controller = x.AddComponent<CutSceneController>();
                          controller.Init(cutScene.Type);
                          _cutScenes.Add(controller);
                      });
            _droneWorld.AddListener<WorldEvent>(WorldEvent.CREATED, OnCreateWorld);
            _droneWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
        }

        private void OnEndGame(InGameEvent obj)
        {
            if (obj.EndGameReason == EndGameReasons.VICTORY) {
                _cutScenes.Find(x => x.Type == CutSceneType.FINISH).StartCutScene();
            }
            
        }

        private void OnCreateWorld(WorldEvent obj)
        {
            _cutScenes.Find(x => x.Type == CutSceneType.START).StartCutScene();
        }

        public WorldObjectType ObjectType { get; }
    }
}