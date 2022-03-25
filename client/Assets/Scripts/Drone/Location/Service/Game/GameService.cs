using System;
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
using Drone.Location.World;
using Drone.Location.World.CutScene;
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

        private LevelDescriptor _levelDescriptor;

        private int _countChips;

        private CurvedWorldController _curvedWorldController;

        public void Init()
        {
            InitCurveWorldController();
            Time.timeScale = 1f;
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _countChips = 0;
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CHIP_UP, OnChipUp);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CUTSCENE_END, OnFinishCutSceneEnd);
            _overlayManager.HideLoadingOverlay(true);
        }

        private void OnFinishCutSceneEnd(InGameEvent obj)
        {
            if (obj.CutSceneType == CutSceneType.FINISH) {
                _dialogManager.ShowModal<LevelFinishedDialog>();
            }
        }

        private void OnChipUp(InGameEvent obj)
        {
            _countChips += 1;
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
            switch (inGameEvent.EndGameReason) {
                case EndGameReasons.CRUSH:
                    break;
                case EndGameReasons.VICTORY:
                    Victory();
                    break;
                default:
                    throw new Exception($"Reason {inGameEvent.EndGameReason} is not implemented");
            }
        }

        private void SetStatsInProgress(bool isWin)
        {
            if (!isWin) {
                return;
            }
            _levelService.SetLevelProgress(_levelDescriptor, _countChips + _levelDescriptor.RewardForPassing);
        }

        private void Victory()
        {
            SetStatsInProgress(true);
        }

        private void OnDestroy()
        {
            Destroy(_curvedWorldController);
        }

        public int ChipsCount
        {
            get { return _countChips; }
            set { _countChips = value; }
        }
    }
}