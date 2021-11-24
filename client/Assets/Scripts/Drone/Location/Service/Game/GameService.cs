using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Service;
using Drone.Location.Event;
using Drone.Location.World.Drone.Event;
using Drone.Location.World.Drone.Model;
using Drone.Location.World.Drone.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
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
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

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
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, DroneModel));

            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.FINISHED, OnFinished);
            _gameWorld.Require().AddListener<ObstacleEvent>(ObstacleEvent.LETHAL_CRUSH, OnDroneLethalCrash);
            _gameWorld.Require().AddListener<ControllEvent>(ControllEvent.START_GAME, StartFlight);

            _overlayManager.Require().HideLoadingOverlay(true);
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
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
        }

        private void DroneFailed(FailedReasons reason)
        {
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(reason);
        }

        public void EndGame()
        {
            Time.timeScale = 0f;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.END_GAME));
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