using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using TMPro;
using UnityEngine;

namespace Drone.Location.UI
{
    [UIController("UI/GameOverlay/pfGameOverlay@embeded")]
    public class GameOverlay : MonoBehaviour
    {
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [UIComponentBinding("ChipsValue")]
        private TextMeshProUGUI _countChips;

        [UIComponentBinding("TimeValue")]
        private TextMeshProUGUI _timer;

        [UIComponentBinding("EnergyValue")]
        private TextMeshProUGUI _countEnergy;

        [UIComponentBinding("DurabilityValue")]
        private TextMeshProUGUI _durability;

        private float _time;
        private bool _isGame;

        [UICreated]
        private void Init()
        {
            
            _timer.text = "0,00";
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.UI_UPDATE, UiUpdate);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }

        private void EndGame(WorldEvent objectEvent)
        {
            _isGame = false;
            Destroy(gameObject);
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

        private void SetStats(DroneModel model)
        {
            _countChips.text = model.countChips.ToString();
            _countEnergy.text = model.energy.ToString("F0");
            _durability.text = ((model.durability / model.DroneDescriptor.Durability) * 100).ToString("F0") + "%";
        }

        [UIOnClick("PauseButton")]
        private void OnPauseButton()
        {
            _dialogManager.Require().ShowModal<LevelPauseDialog>();
        }
    }
}