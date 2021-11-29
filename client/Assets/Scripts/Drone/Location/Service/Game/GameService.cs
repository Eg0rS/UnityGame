using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using Drone.Location.Event;
using Drone.Location.World.Drone.Event;
using Drone.Location.World.Drone.Model;
using Drone.Location.World.Drone.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service.Game
{
    public enum FailedReasons
    {
        Crashed,
        EnergyFalled,
    }

    [Injectable]
    public class GameService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private GameWorld _gameWorld;

        [Inject]
        private OverlayManager _overlayManager;

        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DroneService _droneService;

        private const float TIME_FOR_DEAD = 0.5f;

        private LevelDescriptor _levelDescriptor;

        private float _startTime;

        private float _countChips;
        public DroneModel DroneModel { get; private set; }

        public void Init()
        {
            DroneModel = new DroneModel(_droneService.GetDronById(_levelService.SelectedDroneId).DroneDescriptor);
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _countChips = 0;
            _gameWorld.Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, DroneModel));

            _gameWorld.AddListener<WorldEvent>(WorldEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.AddListener<WorldEvent>(WorldEvent.FINISHED, OnFinished);
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.LETHAL_CRUSH, OnDroneLethalCrash);
            _gameWorld.AddListener<ControllEvent>(ControllEvent.START_GAME, StartFlight);

            _overlayManager.HideLoadingOverlay(true);
        }

        private void StartFlight(ControllEvent controllEvent)
        {
            _startTime = Time.time;
        }

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                // _levelService.SetLevelProgress(_levelService.SelectedLevelId, CalculateStars(timeInGame), _droneModel.countChips, timeInGame,
                //                                (int) ((_droneModel.durability / _droneModel.maxDurability) * 100));
            }
        }

        private void OnDroneLethalCrash(ObstacleEvent obstacleEvent)
        {
            Invoke(nameof(Crashed), TIME_FOR_DEAD);
        }

        private void Crashed()
        {
            DroneFailed(FailedReasons.Crashed);
        }

        private void OnTakeChip(WorldEvent worldEvent)
        {
            _countChips++;
            
        }

        private void OnFinished(WorldEvent worldEvent)
        {
            Victory();
        }

        private void Victory()
        {
            SetStatsInProgress(true);
            EndGame();
            _dialogManager.ShowModal<LevelFinishedDialog>();
        }

        private void DroneFailed(FailedReasons reason)
        {
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.ShowModal<LevelFailedCompactDialog>(reason);
        }

        public void EndGame()
        {
            Time.timeScale = 0f;
            _gameWorld.Dispatch(new WorldEvent(WorldEvent.END_GAME));
        }

        private int CalculateStars(float timeInGame)
        {
            int countStars = 0;

            // if (_droneModel.durability >= _levelDescriptor.NecessaryDurability) {
            //     countStars++;
            // }
            // if (_droneModel.countChips >= _levelDescriptor.NecessaryCountChips) {
            //     countStars++;
            // }
            // if (timeInGame <= _levelDescriptor.NecessaryTime) {
            //     countStars++;
            // }

            return countStars;
        }
    }
}