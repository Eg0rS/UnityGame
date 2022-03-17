using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using Drone.Core.UI.Dialog;
using Drone.Location.Service.Game;
using Drone.Location.Service.Game.Event;
using Drone.Location.World;
using GameKit.World;
using IoC.Attribute;
using Image = UnityEngine.UI.Image;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class FailLevelDialog : GameEventDispatcher
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/Respawn/pfRespawnDialog@embeded";

        private string _levelId;

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

        private void RespawnButtonClick()
        {
            _dialogManager.Hide(this);
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.RESPAWN));
        }
    }
}