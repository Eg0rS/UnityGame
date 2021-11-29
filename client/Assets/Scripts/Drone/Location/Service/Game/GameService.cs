using System;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using Drone.Location.Event;
using Drone.Location.Service.Game.Event;
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

        private const float TIME_FOR_DEAD = 0.3f;

        private LevelDescriptor _levelDescriptor;

        private float _startTime;

        private float _countChips;
        public DroneModel DroneModel { get; private set; }

        public void Init()
        {
            DroneModel = new DroneModel(_droneService.GetDronById(_levelService.SelectedDroneId).DroneDescriptor);
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _countChips = 0;
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.SET_DRONE_PARAMETERS, DroneModel));
            
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);

            _gameWorld.AddListener<WorldObjectEvent>(WorldObjectEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.AddListener<WorldObjectEvent>(WorldObjectEvent.FINISHED, OnFinished);
            //событие смерти 
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, StartFlight);

            _overlayManager.HideLoadingOverlay(true);
        }

        private void OnEndGame(InGameEvent inGameEvent)
        {
            EndGameReasons reason = inGameEvent.EndGameReason;
            switch (reason) {
                case EndGameReasons.OUT_OF_DURABILITY:
                case EndGameReasons.OUT_OF_ENEGRY:
                    GameFailed(reason);
                    break;
                case EndGameReasons.VICTORY:
                    break;
                default:
                    throw new Exception($"Reason {reason} is not implemented");
                    break;
                    
                
            }
        }

        private void GameFailed(EndGameReasons reason)
        {
            
        }

        private void StartFlight(InGameEvent obj)
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

        private void OnTakeChip(WorldObjectEvent worldObjectEvent)
        {
            _countChips++;
            
        }

        private void OnFinished(WorldObjectEvent worldObjectEvent)
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
            _gameWorld.Dispatch(new WorldObjectEvent(WorldObjectEvent.END_GAME));
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