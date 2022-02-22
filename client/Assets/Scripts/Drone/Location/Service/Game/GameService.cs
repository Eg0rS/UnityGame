using System;
using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using AmazingAssets.CurvedWorld;
using DG.Tweening;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using Drone.Location.Service.Game.Event;
using Drone.Location.Service.Control.Drone.Model;
using Drone.Location.Service.Control.Drone.Service;
using Drone.Location.World;
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

        private int _countChips;

        private CurvedWorldController _curvedWorldController;

        public void Init()
        {
            InitCurveWorldController();
            Time.timeScale = 1f;
            DroneModel droneModel = new DroneModel(_droneService.GetDronById(_levelService.SelectedDroneId).DroneDescriptor);
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _countChips = 0;
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.SET_DRONE_PARAMETERS, droneModel));
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            _overlayManager.HideLoadingOverlay(true);
        }

        private void InitCurveWorldController()
        {
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CHANGE_TILE, ChangeTile);
            _curvedWorldController = gameObject.AddComponent<CurvedWorldController>();
            _curvedWorldController.bendType = BEND_TYPE.ClassicRunner_Z_Positive;
            _curvedWorldController.bendHorizontalSize = 3;
            _curvedWorldController.bendVerticalSize = 1.5f;
        }

        private void ChangeTile(InGameEvent obj)
        {
            float fromHorizontal = _curvedWorldController.bendHorizontalSize;
            float toHorizontal = fromHorizontal * -1;
            DOVirtual.Float(fromHorizontal, toHorizontal, 10.0f, value => _curvedWorldController.bendHorizontalSize = value);
            float fromVertical = _curvedWorldController.bendVerticalSize;
            float toVertical = UnityEngine.Random.Range(-1.5f, 1.5f);
            DOVirtual.Float(fromVertical, toVertical, 10.0f, value => _curvedWorldController.bendVerticalSize = value);
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
            _levelService.SetLevelProgress(_levelDescriptor, _countChips);
        }

        private void Victory()
        {
            SetStatsInProgress(true);
            _dialogManager.ShowModal<LevelFinishedDialog>();
        }

        private void OnDestroy()
        {
            Destroy(_curvedWorldController);
        }
    }
}