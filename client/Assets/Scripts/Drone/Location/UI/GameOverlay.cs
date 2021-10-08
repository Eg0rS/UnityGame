using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using Drone.Core.Audio.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.UI
{
    [UIController("UI/Dialog/pfGameOverlay@embeded")]
    public class GameOverlay : MonoBehaviour
    {
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private SoundService _soundService;

        [UIComponentBinding("CountChips")]
        private UILabel _countChips;

        [UIComponentBinding("ParceTime")]
        private UILabel _timer;

        [UIComponentBinding("CountEnergy")]
        private UILabel _countEnergy;

        [UIComponentBinding("CountDurability")]
        private UILabel _durability;

        [UIObjectBinding("ShieldActive")]
        private GameObject _shieldActive;

        private float _time;
        private float _maxDurability;
        private bool _isGame;

        [UICreated]
        private void Init(DroneModel droneModel)
        {
            _maxDurability = droneModel.durability; //для вывода в процентах
            SetStats(droneModel);
            _timer.text = "0,00";
            _shieldActive.SetActive(false);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.UI_UPDATE, UiUpdate);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }

        private void EndGame(WorldEvent objectEvent)
        {
            _isGame = false;
        }

        private void StartGame(WorldEvent objectEvent)
        {
            _isGame = true;
        }

        private void Update()
        {
            if (!_isGame) {
                return;
            }
            _time += Time.deltaTime;
            _timer.text = _time.ToString("F2");
        }

        private void UiUpdate(WorldEvent objectEvent)
        {
            SetStats(objectEvent.DroneModel);
        }

        private void SetStats(DroneModel dronStats)
        {
            _countChips.text = dronStats.countChips.ToString();
            _countEnergy.text = dronStats.energy.ToString("F0");
            _durability.text = ((dronStats.durability / _maxDurability) * 100).ToString("F0") + "%";
        }

        [UIOnClick("PauseButton")]
        private void OnPauseButton()
        {
            _dialogManager.Require().ShowModal<LevelPauseDialog>();
        }
    }
}