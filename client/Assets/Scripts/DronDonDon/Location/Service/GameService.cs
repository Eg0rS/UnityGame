using System.Collections;
using System.Diagnostics;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Model;
using DronDonDon.Location.Model.BaseModel;
using DronDonDon.Location.Model.BonusChips;
using DronDonDon.Location.Model.Finish;
using DronDonDon.Location.Model.Obstacle;
using DronDonDon.Location.Model.ShieldBooster;
using DronDonDon.Location.Model.SpeedBooster;
using DronDonDon.Location.UI;
using DronDonDon.Location.World.Dron.Descriptor;
using DronDonDon.Location.World.Dron.Service;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

public struct DronStats
{
     public float _durability;
     public float _energy;
     public int _countChips;
}

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class GameService: GameEventDispatcher
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject] 
        private IoCProvider<OverlayManager> _overlayManager;
        
        [Inject]
        private IGestureService _gestureService;

        [Inject]
        private LevelService _levelService;
        
        [Inject] 
        private UIService _uiService;

        [Inject] 
        private DronService _dronService;

        private GameObject _levelContainer;
                
        private DronStats _dronStats;

        private bool _isPlay = false;

        private LevelDescriptor _levelDescriptor;

        private float _startTime=0;
        
        public void StartGame(LevelDescriptor levelDescriptor, string dronId)
        {
            _levelDescriptor = levelDescriptor; 
            DronDescriptor dronDescriptor = _dronService.GetDronById(dronId).DronDescriptor;
            _overlayManager.Require().HideLoadingOverlay(true);
             _levelContainer = GameObject.Find($"Overlay");
            
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.ON_COLLISION, DronCollision);
            _dronStats._durability = dronDescriptor.Durability;
            _dronStats._energy = dronDescriptor.Energy;
            _dronStats._countChips = 0;
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.UI_UPDATE,
                _dronStats));
            _overlayManager.Require().CreateGameOverlay(_dronStats);
        }

        private void OnTap(Tap tap)
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.START_GAME, 
                gameObject));
            _isPlay = true; 
            StartCoroutine(FallEnergy());
            _startTime=Time.time;
        }

        private void DronCollision(WorldObjectEvent worldObjectEvent)
        {
            GameObject _collisionObject = worldObjectEvent._collisionObject;
            switch (_collisionObject.GetComponent<PrefabModel>().ObjectType)
            {
                case WorldObjectType.OBSTACLE:
                    OnDronCrash(_collisionObject.GetComponent<ObstacleModel>());
                    break;
                case WorldObjectType.BONUS_CHIPS:
                    OnTakeChip(_collisionObject.GetComponent<BonusChipsModel>());
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    OnTakeSpeed(_collisionObject.GetComponent<SpeedBoosterModel>());
                    break;
                case WorldObjectType.SHIELD_BUSTER:
                    OnTakeShield(_collisionObject.GetComponent<ShieldBoosterModel>());
                    break;
                case WorldObjectType.FINISH:
                    Victory(_collisionObject.GetComponent<FinishModel>());
                    break;
                default:
                    break;
            }
        }

        private IEnumerator FallEnergy()
        {
            while (_isPlay)
            {
                _dronStats._energy -= 0.4f;
                if (_dronStats._energy <= 0)
                {
                    _dronStats._energy = 0;
                    DronFailed(1);
                }
                UiUpdate();
                yield return new WaitForSeconds(1f);
            }
        }
        private void OnTakeShield(ShieldBoosterModel getComponent)
        {
            getComponent.gameObject.SetActive(false);
        }

        private void OnTakeSpeed(SpeedBoosterModel getComponent)
        {
            getComponent.gameObject.SetActive(false);
        }

        private void OnTakeChip(BonusChipsModel getComponent)
        {
            _dronStats._countChips++;
            UiUpdate();
            getComponent.gameObject.SetActive(false);
        }

        private void OnDronCrash(ObstacleModel getComponent)
        {
            _dronStats._durability -= getComponent.Damage;
            if (_dronStats._durability <= 0)
            {
                _dronStats._durability = 0;
                DronFailed(0);
            }
            UiUpdate();
        }

        private void UiUpdate()
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.UI_UPDATE,
                _dronStats));
        }

        private void EndGame()
        {
            _isPlay = false;
            float timeInGame = Time.time - _startTime;
            Time.timeScale = 0f;
            _levelService.SetLevelProgress(_levelService.CurrentLevelId, CalculateStars(timeInGame), _dronStats._countChips, 
                timeInGame, _dronStats._durability, false, 
                _levelService.CurrentLevelId == _levelDescriptor.Id);
        }
        private void Victory(FinishModel getComponent)
        {
            EndGame();
            _uiService.Create<LevelFinishedDialog>(UiModel
                .Create<LevelFinishedDialog>()
                .Container(_levelContainer)).Done();
            _gestureService.AddTapHandler(OnTap);
        }

        private void DronFailed(short reason)
        {
            EndGame();
            _uiService.Create<LevelFailedCompactDialog>(UiModel
                .Create<LevelFailedCompactDialog>(reason)
                .Container(_levelContainer)).Done();
            _gestureService.AddTapHandler(OnTap);
        }

        private short CalculateStars(float timeInGame)
        {
            short countStars=0;

            if (_dronStats._durability >= _levelDescriptor.NecessaryDurability)
            {
                countStars++;
            }
            if (_dronStats._countChips >= _levelDescriptor.NecessaryCountChips)
            {
                countStars++;
            }
            if ( timeInGame >= _levelDescriptor.NecessaryTime)
            {
                countStars++;
            }

            return countStars;
        }
    }
}