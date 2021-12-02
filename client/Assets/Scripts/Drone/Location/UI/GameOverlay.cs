using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Location.Event;
using Drone.Location.Service.Game;
using Drone.Location.Service.Game.Event;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using TMPro;
using UnityEngine;

namespace Drone.Location.UI
{
    [UIController("UI/GameOverlay/pfGameOverlay@embeded")]
    public class GameOverlay : MonoBehaviour
    {
        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private GameWorld _gameWorld;
        [Inject]
        private GameService _gameService;

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
        private DroneModel _droneModel;

        [UICreated]
        private void Init()
        {
            _timer.text = "0,00";
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, StartGame);
            
            _gameWorld.AddListener<EnergyEvent>(EnergyEvent.UPDATE, EnergyUpdate);
            _gameWorld.AddListener<DurabilityEvent>(DurabilityEvent.UPDATED, DurabilityUpdate);
            
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, EndGame);
            SetStats(_gameService.DroneModel);
        }

        private void DurabilityUpdate(DurabilityEvent obstacleEvent)
        {
            _durability.text = ((obstacleEvent.DurabilityValue / _droneModel.DroneDescriptor.Durability) * 100).ToString("F0") + "%";
        }

        private void EnergyUpdate(EnergyEvent energyEvent)
        {
            _countEnergy.text = energyEvent.EnergyValue.ToString("F0");
        }

        private void EndGame(InGameEvent inGameEvent)
        {
            _isGame = false;
            _dialogManager.Hide(gameObject);
        }

        private void StartGame(InGameEvent inGameEvent)
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

        private void SetStats(DroneModel model)
        {
            _droneModel = model;
            _countChips.text = model.countChips.ToString();
            _countEnergy.text = model.energy.ToString("F0");
            _durability.text = ((model.durability / model.DroneDescriptor.Durability) * 100).ToString("F0") + "%";
        }

        [UIOnClick("PauseButton")]
        private void OnPauseButton()
        {
            _dialogManager.ShowModal<LevelPauseDialog>();
        }
    }
}