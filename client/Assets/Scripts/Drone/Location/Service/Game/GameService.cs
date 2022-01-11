using System;
using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using Drone.Location.Service.Game.Event;
using Drone.Location.Service.Control.Drone.Model;
using Drone.Location.Service.Control.Drone.Service;
using Drone.Location.World;
using GameKit.World.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service.Game
{

    [Injectable]
    public class GameService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private DroneWorld _gameWorld;

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

        private int _countChips;
        public DroneModel DroneModel { get; private set; }

        public void Init()
        {
            DroneModel = new DroneModel(_droneService.GetDronById(_levelService.SelectedDroneId).DroneDescriptor);
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _countChips = 0;
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.SET_DRONE_PARAMETERS, DroneModel));

            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            
            //событие смерти
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, StartFlight);

            _overlayManager.HideLoadingOverlay(true);
        }

        private void OnEndGame(InGameEvent inGameEvent)
        {
            EndGameReasons reason = inGameEvent.EndGameReason;
            switch (reason) {
                case EndGameReasons.OUT_OF_DURABILITY:
                case EndGameReasons.OUT_OF_ENERGY:
                    StartCoroutine(GameFailed(reason));
                    break;
                case EndGameReasons.VICTORY:
                    Victory();
                    break;
                default:
                    throw new Exception($"Reason {reason} is not implemented");
                    break;
            }
        }

        private IEnumerator GameFailed(EndGameReasons reason)
        {
            yield return new WaitForSeconds(TIME_FOR_DEAD);
            SetStatsInProgress(false);
            _dialogManager.ShowModal<LevelFailedCompactDialog>(reason);
        }

        private void StartFlight(InGameEvent obj)
        {
            _startTime = Time.time;
        }

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                // _levelService.SetLevelProgress(_levelService.SelectedLevelId, CalculateStars(timeInGame), _countChips, timeInGame,
                //                                (int) ((_durabilityService.Durability / _durabilityService.MaxDurability) * 100));
            }
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
            _dialogManager.ShowModal<LevelFinishedDialog>();
        }

        private int CalculateStars(float timeInGame)
        {
            int countStars = 0;

            // if (_durabilityService.Durability >= _levelDescriptor.Goals.NecessaryDurability) {
            //     countStars++;
            // }
            if (_countChips >= _levelDescriptor.Goals.NecessaryCountChips) {
                countStars++;
            }
            if (timeInGame <= _levelDescriptor.Goals.NecessaryTime) {
                countStars++;
            }

            return countStars;
        }
    }
}