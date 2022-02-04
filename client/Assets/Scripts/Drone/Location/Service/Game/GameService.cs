using System;
using System.Collections;
using System.Collections.Generic;
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
using Drone.Location.World.Spline;
using IoC.Attribute;
using RSG.Promises;
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

        private int _countChips;

        public void Init()
        {
            Time.timeScale = 1f;
            DroneModel droneModel = new DroneModel(_droneService.GetDronById(_levelService.SelectedDroneId).DroneDescriptor);
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _countChips = 0;
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.SET_DRONE_PARAMETERS, droneModel));
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            _overlayManager.HideLoadingOverlay(true);
        }

        private void OnEndGame(InGameEvent inGameEvent)
        {
            EndGameReasons reason = inGameEvent.EndGameReason;
            switch (reason) {
                case EndGameReasons.CRUSH:
                    StartCoroutine(GameFailed());
                    break;
                case EndGameReasons.VICTORY:
                    Victory();
                    break;
                default:
                    throw new Exception($"Reason {reason} is not implemented");
            }
        }

        private IEnumerator GameFailed()
        {
            yield return new WaitForSeconds(TIME_FOR_DEAD);
            SetStatsInProgress(false);
            _dialogManager.ShowModal<FailLevelDialog>();
        }

        private void SetStatsInProgress(bool isWin)
        {
            if (!isWin) {
                return;
            }
            Dictionary<LevelTask, bool> tasks = new Dictionary<LevelTask, bool>();
            _levelDescriptor.GameData.LevelTasks.Each(x => tasks.Add(x, true));
            _levelService.SetLevelProgress(_levelService.SelectedLevelId, tasks, _countChips);
        }

        private void Victory()
        {
            SetStatsInProgress(true);
            _dialogManager.ShowModal<LevelFinishedDialog>();
        }
    }
}