using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using DG.Tweening;
using Drone.Core.UI.Dialog;
using Drone.Location.Service.Game.Event;
using Drone.Location.World;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class FailLevelDialog : GameEventDispatcher
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/Respawn/pfRespawnDialog@embeded";
        private const float TIME_FOR_END = 10f;
        private string _levelId;

        private float _timeForEndLeft = TIME_FOR_END;
        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private DroneWorld _gameWorld;

        [UIComponentBinding("RespawnButton")]
        private UIButton _restartButton;

        [UIComponentBinding("FiledArea")]
        private Image _filedArea;

        [UIComponentBinding("TimelLabel")]
        private UILabel _timerLabel;

        [UICreated]
        public void Init()
        {
            _restartButton.onClick.AddListener(RespawnButtonClick);
        }

        private void Update()
        {
            _timeForEndLeft -= Time.unscaledDeltaTime;
            if (_timeForEndLeft <= 0f) {
                OnComplete();
                return;
            }
            float percent = _timeForEndLeft / TIME_FOR_END;
            if (percent <= 0.3f) {
                _filedArea.color = new Color(1, 0.02745098f, 0.02745098f);
                _timerLabel.color = new Color(1, 0.02745098f, 0.02745098f);
            }
            _filedArea.fillAmount = _timeForEndLeft / TIME_FOR_END;
            _timerLabel.text = _timeForEndLeft.ToString("F1");
        }

        private void OnComplete()
        {
            _dialogManager.Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        private void RespawnButtonClick()
        {
            _dialogManager.Hide(this);
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.RESPAWN));
        }
    }
}