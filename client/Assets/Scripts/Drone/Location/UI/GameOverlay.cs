using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.Location.Event;
using Drone.Location.Service.Game;
using Drone.Location.Service.Game.Event;
using Drone.Location.Service.Control.Drone.Event;
using Drone.Location.Service.Control.Drone.Model;
using Drone.World;
using IoC.Attribute;
using TMPro;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEngine.UI;

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

        [UIComponentBinding("UpArrow")]
        private Image _upArrow;
        [UIComponentBinding("DownArrow")]
        private Image _downArrow;
        [UIComponentBinding("LeftArrow")]
        private Image _leftArrow;
        [UIComponentBinding("RightArrow")]
        private Image _rightArrow;

        [UIComponentBinding("UpRightArrow")]
        private Image _upRightArrow;
        [UIComponentBinding("DownRightArrow")]
        private Image _downRightArrow;
        [UIComponentBinding("UpLeftArrow")]
        private Image _upLeftArrow;
        [UIComponentBinding("DownLeftArrow")]
        private Image _downLeftArrow;

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

            _gameWorld.AddListener<ControllEvent>(ControllEvent.MOVEMENT, OnMovement);

            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, EndGame);
            SetStats(_gameService.DroneModel);
        }

        private void OnMovement(ControllEvent сontrollEvent)
        {
            Vector2 move = сontrollEvent.Movement;
            if (move == new Vector2(0, 2)) {
                _upArrow.DOFade(1, 0.5f).OnComplete(() => _upArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(0, -2)) {
                _downArrow.DOFade(1, 0.5f).OnComplete(() => _downArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(-2, 0)) {
                _leftArrow.DOFade(1, 0.5f).OnComplete(() => _leftArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(2, 0)) {
                _rightArrow.DOFade(1, 0.5f).OnComplete(() => _rightArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(1, 2) || move == new Vector2(2, 1) || move == new Vector2(2, 2)) {
                _upRightArrow.DOFade(1, 0.5f).OnComplete(() => _upRightArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(-2, 1) || move == new Vector2(-1, 2) || move == new Vector2(-2, 2)) {
                _upLeftArrow.DOFade(1, 0.5f).OnComplete(() => _upLeftArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(1, -2) || move == new Vector2(2, -1) || move == new Vector2(2, -2)) {
                _downRightArrow.DOFade(1, 0.5f).OnComplete(() => _downRightArrow.DOFade(0, 0.5f));
            } else if (move == new Vector2(-1, -2) || move == new Vector2(-2, -1) || move == new Vector2(-2, -2)) {
                _downLeftArrow.DOFade(1, 0.5f).OnComplete(() => _downLeftArrow.DOFade(0, 0.5f));
            }
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