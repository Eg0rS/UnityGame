using System;
using System.Collections.Generic;
using System.Reflection;
using DronDonDon.Location.Model.Dron;
using UnityEngine;
using DronDonDon.Location.Model;
using DronDonDon.Location.Model.Object;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model;
using AgkCommons.Input.Gesture.Model.Gestures;
using DronDonDon.Location.Model.BaseModel;
using IoC.Attribute;
using IoC.Extension;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Binding.Attributes.Method;
using BezierSolution;
using Unity.Mathematics;

namespace DronDonDon.Location.World.Dron
{
    public class DronController: MonoBehaviour,  IWorldObjectController<DronModel>
    {
        private Vector2 _containerPosition=Vector2.zero;
        private BezierWalkerWithSpeed _bezier;
        private float _containerCoefficient=9;
        private bool _isShifting=false;
        private Vector3 _previusPosition;
        private float _speedShift ;
        private float _shiftCoeficient=0;
        private float _levelSpeed = 15;
        private float _acceleration = 0.2f;
        private bool _GameStarted = false;
        private float _angleRotate = 60;
        private Swipe _lastWorkSwipe;
        
        [Inject]
        private IGestureService _gestureService;
        
        public WorldObjectType ObjectType { get; private set; }
        public void Init(DronModel  model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            DisablePath();
            ObjectType = model.ObjectType;
            _speedShift = model.SpeedShift;
            _gestureService.AddSwipeHandler(OnSwiped,false);
            _gestureService.AddTapHandler(OnTap);
        }
        
        private void OnTap(Tap tap)
        {
            _GameStarted = true;
            EnablePath();
        }
        
        public void Update()
        {
            if (!_GameStarted) return;
            _levelSpeed += _acceleration *Time.deltaTime;
            SetBeizerSpeed(_levelSpeed);

            if (_isShifting)
            {
                _shiftCoeficient += _speedShift * Time.deltaTime;
                transform.localPosition = Vector3.Lerp(_previusPosition, _containerPosition, _shiftCoeficient);
                RotateDron();
                
                if (transform.localPosition.Equals(_containerPosition))
                {
                    _isShifting = false;
                    _shiftCoeficient = 0;
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
            
            if (!_isShifting)
            {
                _lastWorkSwipe = swipe;
                MoveDron(NumberSwipedToSector(swipe));
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

        private void ShiftVirtualPosition(int sector)
        {
            if (!ValidateMovement(sector))
            {
                return;
            }
            _containerPosition += virtualVectors[sector] *_containerCoefficient;
        }

        public void MoveDron(int sector)
        {
            ShiftVirtualPosition(sector);
            _previusPosition = transform.localPosition;
            _isShifting = true;
        }
        
        private void RotateDron()
        {
            if (_lastWorkSwipe.Check(HorizontalSwipeDirection.LEFT))
            {
                transform.localRotation = quaternion.RotateZ( (float)Math.Sin(_shiftCoeficient * Math.PI) * _angleRotate * Time.deltaTime);
            }
            else if (_lastWorkSwipe.Check(HorizontalSwipeDirection.RIGHT))
            {
                transform.localRotation = quaternion.RotateZ( (float)Math.Sin(_shiftCoeficient * Math.PI) * -_angleRotate * Time.deltaTime);
            }
            else if (_lastWorkSwipe.Check(VerticalSwipeDirection.DOWN))
            {
                transform.localRotation = quaternion.RotateX( (float)Math.Sin(_shiftCoeficient * Math.PI) * _angleRotate * Time.deltaTime);
            }
            else if (_lastWorkSwipe.Check(VerticalSwipeDirection.UP))
            {
                transform.localRotation = quaternion.RotateX( (float)Math.Sin(_shiftCoeficient * Math.PI) * -_angleRotate * Time.deltaTime);
            }
        }
        
    }
}