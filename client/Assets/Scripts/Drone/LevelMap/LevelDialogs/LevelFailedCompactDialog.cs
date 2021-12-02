﻿using Adept.Logger;
using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.Levels.Service;
using Drone.Location.Service;
using Drone.Location.Service.Game;
using Drone.Location.Service.Game.Event;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFailedCompactDialog : GameEventDispatcher
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFailedCompactDialog>();
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFailedCompactDialog@embeded";

        private string _levelId;
        private EndGameReasons _endGameReasons;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LevelService _levelService;

        [UIComponentBinding("ReasonText")]
        private Text _failReasonLabel;

        [UICreated]
        public void Init(EndGameReasons failReason)
        {
            _levelId = _levelService.SelectedLevelId;
            _endGameReasons = failReason;
            SetDialogLabels();
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelId);
        }

        [UIOnClick("MenuButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        private void SetDialogLabels()
        {
            _failReasonLabel.text = _endGameReasons switch {
                    EndGameReasons.OUT_OF_DURABILITY => "Дрон разбился",
                    EndGameReasons.OUT_OF_ENERGY => "Закончилась энергия",
                    _ => _failReasonLabel.text
            };
        }
    }
}