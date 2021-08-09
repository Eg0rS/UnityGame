﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DronDonDon.Location.Model.Dron;
using UnityEngine;
using DronDonDon.Location.Model;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model;
using AgkCommons.Input.Gesture.Model.Gestures;
using IoC.Attribute;
using AgkCommons.Input.Gesture.Service;
using BezierSolution;
using DronDonDon.Location.Model.BaseModel;
using DronDonDon.Location.Model.BonusChips;
using DronDonDon.Location.Model.Finish;
using DronDonDon.Location.Model.Obstacle;
using DronDonDon.Location.Model.ShieldBooster;
using DronDonDon.Location.Model.SpeedBooster;
using DronDonDon.Location.World.Obstacle;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Util;

namespace DronDonDon.Location.World.Dron
{
    public class DronController: GameEventDispatcher,  IWorldObjectController<DronModel>
    {
        private Vector2 _containerPosition=Vector2.zero;
        private BezierWalkerWithSpeed _bezier;
        private float _containerCoefficient=9;
        private bool _isShifting=false;
        private Vector3 _previusPosition;
        private float _shiftCoeficient=0;
        private float _levelSpeed = 15;
        private float _acceleration = 0.2f;
        private bool _isGameRun = false;
        private float _angleRotate = 60;
        private Swipe _lastWorkSwipe=null;
        private DronModel _model=null;
        
        private float _speedShift=0;
        private float _durability=0;
        private float _energy=0;

        [Inject]
        private IGestureService _gestureService;
        
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        
        public WorldObjectType ObjectType { get; private set; }
        public void Init(DronModel  model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            DisablePath();
            ObjectType = model.ObjectType;
            _model = model;
            _speedShift = model.SpeedShift;
            _durability = model.durability;
            _energy = _model.Energy;
            
            _gestureService.AddSwipeHandler(OnSwiped,false);
            _gestureService.AddTapHandler(OnTap);
            
                //_gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.SHIELD_ACTIVE, ShieldActivate );
        }
        
        private void OnTap(Tap tap)
        {
            _isGameRun = true;
            EnablePath();
        }
        
        public void Update()
        {
            if (!_isGameRun) return;
            
            if (GetBeizerPassedPath() < 0.5f)
            {
                _levelSpeed += _acceleration *Time.deltaTime;
            }
            SetBeizerSpeed(_levelSpeed);
            
            if (_isShifting)
            {
                _shiftCoeficient += _speedShift * Time.deltaTime;
                transform.localPosition = Vector3.Lerp(_previusPosition, _containerPosition, _shiftCoeficient);
                RotateSelf(_lastWorkSwipe, (float)Math.Sin(_shiftCoeficient * Math.PI) * _angleRotate);
                
                if (transform.localPosition.Equals(_containerPosition))
                {
                    _isShifting = false;
                    _shiftCoeficient = 0;
                    RotateSelf(_lastWorkSwipe, 0);
                }
            }
        }
        
        private void SetBeizerSpeed(float newSpeed)
        {
            _bezier.speed = newSpeed;
        }
        
        private float GetBeizerPassedPath()
        {
            return _bezier.NormalizedT;
        }

        private void EnablePath()
        {
            _bezier.enabled=true;
        }
        private void DisablePath()
        {
            _bezier.enabled=false;
        }
        
        private void OnSwiped(Swipe swipe)
        {
            if (!_isShifting && _isGameRun)
            {
                _lastWorkSwipe = swipe;
                ShiftNewPosition(NumberSwipedToSector(swipe));
            }
        }
        
        private int NumberSwipedToSector(Swipe swipe)
        {
            Vector2 swipeEndPoint;
            Vector2 swipeVector;
            int angle;
            int result;
         
            swipeEndPoint = (Vector2) typeof(Swipe).GetField("_endPoint", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(swipe);
            swipeVector = swipeEndPoint - swipe.Position;
            angle = (int) Vector2.Angle(Vector2.up, swipeVector.normalized);
            
            result = Vector2.Angle(Vector2.right, swipeVector.normalized) > 90 ? 360 - angle : angle;
            return (int) Mathf.Round(result / 45f) % 8;
            
        }
    
       
        private Dictionary<int, Vector2> virtualVectors = new Dictionary<int, Vector2>(8)
        {
            {0, new Vector2(0, 1)},    // вверх
            {1, new Vector2(1, 1)},    // вправо вверх
            {2, new Vector2(1, 0)},    // вправо
            {3, new Vector2(1, -1)},   // вправо вниз
            {4, new Vector2(0, -1)},   // вниз
            {5, new Vector2(-1, -1)},  // влево вниз
            {6, new Vector2(-1, 0)},   // влево 
            {7, new Vector2(-1, 1)},   // влево вверх
        };
        
        private bool ValidateMovement(int sector)
        {
            Vector2Int fakePosition = Vector2Int.RoundToInt(_containerPosition / _containerCoefficient); 
            return sector switch
            {
                0 => fakePosition.y != 1,
                1 => fakePosition.x != 1 && fakePosition.y != 1,
                2 => fakePosition.x != 1,
                3 => fakePosition.x != 1 && fakePosition.y != -1,
                4 => fakePosition.y != -1,
                5 => fakePosition.x != -1 && fakePosition.y != -1,
                6 => fakePosition.x != -1,
                7 => fakePosition.x != -1 && fakePosition.y != 1,
                _ => false
            };
        }

        private void ShiftNewPosition(int sector)
        {
            if (!ValidateMovement(sector))
            {
                return;
            }
            _containerPosition += virtualVectors[sector] *_containerCoefficient;
            _previusPosition = transform.localPosition;
            _isShifting = true;
        }

        private void RotateSelf(Swipe swipe, float angleRotate)
        {
            if (swipe.Check(HorizontalSwipeDirection.LEFT))
            {
                transform.localRotation = Quaternion.Euler(0,0, angleRotate);
            }
            else if (swipe.Check(HorizontalSwipeDirection.RIGHT))
            {
                transform.localRotation = Quaternion.Euler(0,0, -angleRotate);
            }
            else if (swipe.Check(VerticalSwipeDirection.DOWN))
            {
                transform.localRotation = Quaternion.Euler(angleRotate * 0.5f, 0,0);
            }
            else if (swipe.Check(VerticalSwipeDirection.UP))
            {
                transform.localRotation = Quaternion.Euler( -angleRotate * 0.5f, 0,0);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            switch (other.gameObject.GetComponent<PrefabModel>().ObjectType)
            {
                case WorldObjectType.OBSTACLE:
                    OnCrash(other.gameObject.GetComponent<ObstacleModel>());
                    break;
                case WorldObjectType.BONUS_CHIPS:
                    OnTakeChip(other.gameObject.GetComponent<BonusChipsModel>());
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    OnTakeSpeed(other.gameObject.GetComponent<SpeedBoosterModel>());
                    break;
                case WorldObjectType.SHIELD_BUSTER:
                    OnTakeShield(other.gameObject.GetComponent<ShieldBoosterModel>());
                    break;
                case WorldObjectType.FINISH:
                    Victory(other.gameObject.GetComponent<FinishModel>());
                    break;
                default:
                    break;
            }
        }

        private void OnCrash(ObstacleModel obstacle)
        {
            _durability -= obstacle.Damage;
        }
        
        private void OnTakeChip(BonusChipsModel chip)
        {
            
            Destroy(chip.gameObject);
        }

        private void OnTakeSpeed(SpeedBoosterModel speedBooster)
        {
            Destroy(speedBooster.gameObject);
        }

        private void OnTakeShield(ShieldBoosterModel shieldBooster)
        {
            Destroy(shieldBooster.gameObject);
        }

        private void Victory(FinishModel finish)
        {
            _isGameRun = false;
            DisablePath();
        }
        
        private void GameOver()
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            _isGameRun = false;
            DisablePath();
        }
    }
}