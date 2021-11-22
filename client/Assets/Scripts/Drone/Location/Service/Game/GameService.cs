using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Service;
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
        private IoCProvider<BoosterService> _boosterService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DroneService _droneService;

        private const float TIME_FOR_DEAD = 1f;

        private LevelDescriptor _levelDescriptor;
        
        private float _startTime;
        private DroneModel _droneModel;

        public void Init()
        {
            _droneModel = new DroneModel(_droneService.GetDronById(_levelService.SelectedDroneId).DroneDescriptor);
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, _droneModel));
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.FINISHED, OnFinished);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASH, OnDroneCrash);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_LETHAL_CRASH, OnDroneLethalCrash);
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
                _levelService.SetLevelProgress(_levelService.SelectedLevelId, CalculateStars(timeInGame), _droneModel.countChips, timeInGame,
                                               (int) ((_droneModel.durability / _droneModel.maxDurability) * 100));
            }
        }

        private void OnDroneCrash(WorldEvent worldEvent)
        {
            _droneModel.durability -= worldEvent.Damage;
            if (_droneModel.durability <= 0) {
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASHED));
                Invoke(nameof(Crashed), TIME_FOR_DEAD);
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnDroneLethalCrash(WorldEvent obj)
        {
            _droneModel.durability = 0;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASHED));
            Invoke(nameof(Crashed), TIME_FOR_DEAD);
        }

        private void Crashed()
        {
            DroneFailed(FailedReasons.Crashed);
        }

        private void OnTakeChip(WorldEvent worldEvent)
        {
            if (_boosterService.Require().IsX2Activate) {
                _droneModel.countChips += 2;
            } else {
                _droneModel.countChips++;
            }

            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
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

            if (_droneModel.durability >= _levelDescriptor.NecessaryDurability) {
                countStars++;
            }
            if (_droneModel.countChips >= _levelDescriptor.NecessaryCountChips) {
                countStars++;
            }
            if (timeInGame <= _levelDescriptor.NecessaryTime) {
                countStars++;
            }

            return countStars;
        }
    }
}