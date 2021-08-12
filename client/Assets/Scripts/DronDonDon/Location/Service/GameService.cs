using System.Collections;
using System.Diagnostics;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Model;
using DronDonDon.Location.Model.BaseModel;
using DronDonDon.Location.Model.Battery;
using DronDonDon.Location.Model.BonusChips;
using DronDonDon.Location.Model.Finish;
using DronDonDon.Location.Model.Obstacle;
using DronDonDon.Location.Model.ShieldBooster;
using DronDonDon.Location.Model.SpeedBooster;
using DronDonDon.Location.UI;
using DronDonDon.Location.World.Dron;
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
     public bool _onHasShield;
     public bool _onHasSpeed;
     public float _boostSpeedValue;

     public float _boostShieldTime;
     public float _boostSpeedTime;
     public bool _onActiveShield;
     public float _energyFall;
     public float _energyForSpeed;
     public float _MaxDurability;
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
        private IoCProvider<DialogManager> _dialogManager;
        
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
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.ACTIVATE_BOOST, ActivateBoost);
            _dronStats._MaxDurability = _dronStats._durability;
            _dronStats._durability = dronDescriptor.Durability;
            _dronStats._energy = dronDescriptor.Energy;
            _dronStats._countChips = 0;
            _dronStats._energyFall = 0.15f;
            _overlayManager.Require().CreateGameOverlay(_dronStats);
            _gestureService.AddTapHandler(OnTap,false);
        }

        private void OnTap(Tap tap)
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.START_GAME, 
                gameObject));
            _isPlay = true; 
            StartCoroutine(FallEnergy());
            _startTime=Time.time;
            _gestureService.RemoveTapHandler(OnTap);
        }

        private void DronCollision(WorldObjectEvent worldObjectEvent)
        {
            GameObject _collisionObject = worldObjectEvent._collisionObject;
            switch (_collisionObject.GetComponent<PrefabModel>().ObjectType)
            {
                case WorldObjectType.OBSTACLE:
                    OnDronCrash(_collisionObject.GetComponent<ObstacleModel>());
                    break;
                case WorldObjectType.Battery:
                    OnTakeBattery(_collisionObject.GetComponent<BatteryModel>());
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
                _dronStats._energy -= _dronStats._energyFall;
                if (_dronStats._energy <= 0)
                {
                    _dronStats._energy = 0;
                    UiUpdate();
                    DronFailed(1);
                }
                else
                {
                    UiUpdate();
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        
        private void OnTakeBattery(BatteryModel getComponent)
        {
            _dronStats._energy += getComponent.Energy;
            getComponent.gameObject.SetActive(false);
        }

        private void OnTakeShield(ShieldBoosterModel getComponent)
        {
            _dronStats._onHasShield = true;
            _dronStats._boostShieldTime = getComponent.Duration;
            getComponent.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.TAKE_BOOST, getComponent.ObjectType));
        }

        private void OnTakeSpeed(SpeedBoosterModel getComponent)
        {
            _dronStats._onHasSpeed = true;
            _dronStats._boostSpeedTime = getComponent.Duration;
            _dronStats._boostSpeedValue = getComponent.SpeedBoost;
            _dronStats._energyForSpeed = getComponent.NeedsEnergy;
            getComponent.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.TAKE_BOOST, getComponent.ObjectType));
        }

        private void OnTakeChip(BonusChipsModel getComponent)
        {
            _dronStats._countChips++;
            UiUpdate();
            getComponent.gameObject.SetActive(false);
        }

        private void OnDronCrash(ObstacleModel getComponent)
        {
            if (_dronStats._onActiveShield) return;
            _dronStats._durability -= getComponent.Damage;
            if (_dronStats._durability <= 0)
            {
                _dronStats._durability = 0;
                DronFailed(0);
            }
            UiUpdate();
        }
        
        private void ActivateBoost(WorldObjectEvent objectEvent)
        {
            switch (objectEvent._typeBoost)
            {
                case WorldObjectType.SHIELD_BUSTER:
                    _dronStats._onActiveShield = true;
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    _dronStats._energy -= _dronStats._energyForSpeed;
                    Invoke("DisableSpeed", _dronStats._boostSpeedTime );
                    _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.DRON_BOOST_SPEED, 
                         _dronStats._boostSpeedValue, _dronStats._boostSpeedTime));
                    break;
                default:
                    break;
            }
        }

        private void DisableShield()
        {
            _dronStats._onActiveShield = false;
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
                timeInGame, ((_dronStats._durability / _dronStats._MaxDurability) * 100), false, 
                _levelService.CurrentLevelId == _levelDescriptor.Id);
        }
        private void Victory(FinishModel getComponent)
        {
            EndGame();
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
        }

        private void DronFailed(short reason)
        {
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(reason);
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
            if ( timeInGame <= _levelDescriptor.NecessaryTime)
            {
                countStars++;
            }

            return countStars;
        }
    }
}