using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.Levels.Service;
using Drone.Location.Service;
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
    public class RespawnDialog : GameEventDispatcher
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/Respawn/pfRespawnDialog@embeded";
        private const float TIME_FOR_END = 10f;
        private static readonly Color RED = new Color(1, 0.02745098f, 0.02745098f);

        private float _timeForEndLeft = TIME_FOR_END;
        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private DialogManager _dialogManager;
        [Inject]
        private DroneWorld _gameWorld;
        [Inject]
        private RespawnService _respawnService;
        [Inject]
        private LevelService _levelService;

        [UIComponentBinding("RespawnButton")]
        private UIButton _restartButton;

        [UIComponentBinding("FiledArea")]
        private Image _filedArea;

        [UIComponentBinding("TimelLabel")]
        private UILabel _timerLabel;

        [UIComponentBinding("ChipCount")]
        private UILabel _chipCount;

        [UIComponentBinding("Close")]
        private UIButton _close;

        private bool _isFreeRespawn;

        [UICreated]
        public void Init(int respawnPrice)
        {
            SetRespawnPrice(respawnPrice);
            _restartButton.onClick.AddListener(RespawnButtonClick);
            _close.onClick.AddListener(ExitDialog);
        }

        private void SetRespawnPrice(int respawnPrice)
        {
            if (respawnPrice == 0) {
                _chipCount.text = "FREE";
                _isFreeRespawn = true;
            } else {
                _chipCount.text = respawnPrice.ToString();
            }
        }

        private void Update()
        {
            _timeForEndLeft -= Time.unscaledDeltaTime;
            if (_timeForEndLeft <= 0f) {
                ExitDialog();
                return;
            }
            float percent = _timeForEndLeft / TIME_FOR_END;
            if (percent <= 0.3f) {
                _filedArea.color = RED;
                _timerLabel.color = RED;
            }
            _filedArea.fillAmount = _timeForEndLeft / TIME_FOR_END;
            _timerLabel.text = _timeForEndLeft.ToString("F1");
        }

        private void ExitDialog()
        {
            _dialogManager.Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        private void RespawnButtonClick()
        {
            if (!_isFreeRespawn) {
                if (!_respawnService.BuyRespawn()) {
                    _restartButton.image.color = RED;
                    return;
                }
            }
            _levelService.AddRespawnCount();
            _dialogManager.Hide(this);
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.RESPAWN));
        }
    }
}