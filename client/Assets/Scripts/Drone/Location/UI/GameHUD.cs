using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using Drone.LevelMap.LevelDialogs;
using Drone.Location.Service.Game.Event;
using Drone.Location.Service.Control.Drone.Event;
using IoC.Attribute;
using UnityEngine;
using DG.Tweening;
using Drone.Location.World;
using Drone.Location.World.CutScene;
using UnityEngine.UI;

namespace Drone.Location.UI
{
    [UIController("UI_Prototype/HUD/pfHUD@embeded")]
    public class GameHUD : MonoBehaviour
    {
        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private DroneWorld _gameWorld;

        [UIComponentBinding("SettingsButton")]
        private UIButton _pause;

        [UIComponentBinding("TimerLabel")]
        private UILabel _timer;

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

        [UICreated]
        private void Init()
        {
            gameObject.SetActive(false);
            _pause.onClick.AddListener(OnPauseButton);
            _timer.text = "0,00";
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, StartGame);
            _gameWorld.AddListener<ControllEvent>(ControllEvent.MOVEMENT, OnMovement);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CUTSCENE_END, EndStartCutScene);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, EndGame);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.RESPAWN, Respawn);
        }

        private void EndStartCutScene(InGameEvent obj)
        {
            if (obj.CutSceneType == CutSceneType.START) {
                gameObject.SetActive(true);
            }
        }

        private void Respawn(InGameEvent obj)
        {
            _isGame = true;
            gameObject.SetActive(true);
        }

        private void EndGame(InGameEvent inGameEvent)
        {
            _isGame = false;
            gameObject.SetActive(false);
        }

        private void StartGame(InGameEvent inGameEvent)
        {
            _isGame = true;
        }

        private void Update()
        {
            if (_gameWorld.IsPauseWorld || !_isGame) {
                return;
            }
            _time += Time.unscaledDeltaTime;
            _timer.text = _time.ToString("F1");
        }

        private void OnPauseButton()
        {
            _gameWorld.Pause();
            _dialogManager.ShowModal<LevelPauseDialog>();
        }

        private void OnMovement(ControllEvent сontrollEvent)
        {
            float defaultTimeScale = 1 / Time.timeScale;
            Vector2 move = сontrollEvent.Movement;
            if (move == new Vector2(0, 2)) {
                _upArrow.DOFade(1, 0.5f).OnComplete(() => _upArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale = defaultTimeScale;
            } else if (move == new Vector2(0, -2)) {
                _downArrow.DOFade(1, 0.5f).OnComplete(() => _downArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale = defaultTimeScale;
            } else if (move == new Vector2(-2, 0)) {
                _leftArrow.DOFade(1, 0.5f).OnComplete(() => _leftArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale = defaultTimeScale;
            } else if (move == new Vector2(2, 0)) {
                _rightArrow.DOFade(1, 0.5f).OnComplete(() => _rightArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale = defaultTimeScale;
            } else if (move == new Vector2(1, 2) || move == new Vector2(2, 1) || move == new Vector2(2, 2)) {
                _upRightArrow.DOFade(1, 0.5f).OnComplete(() => _upRightArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale =
                        defaultTimeScale;
            } else if (move == new Vector2(-2, 1) || move == new Vector2(-1, 2) || move == new Vector2(-2, 2)) {
                _upLeftArrow.DOFade(1, 0.5f).OnComplete(() => _upLeftArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale = defaultTimeScale;
            } else if (move == new Vector2(1, -2) || move == new Vector2(2, -1) || move == new Vector2(2, -2)) {
                _downRightArrow.DOFade(1, 0.5f).OnComplete(() => _downRightArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale =
                        defaultTimeScale;
            } else if (move == new Vector2(-1, -2) || move == new Vector2(-2, -1) || move == new Vector2(-2, -2)) {
                _downLeftArrow.DOFade(1, 0.5f).OnComplete(() => _downLeftArrow.DOFade(0, 0.5f).timeScale = defaultTimeScale).timeScale =
                        defaultTimeScale;
            }
        }
    }
}