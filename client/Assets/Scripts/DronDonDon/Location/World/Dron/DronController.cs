using System.Collections.Generic;
using System.Reflection;
using DronDonDon.Location.Model.Dron;
using UnityEngine;
using DronDonDon.Location.Model;
using DronDonDon.Location.Model.Object;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using DronDonDon.Location.Model.BaseModel;
using IoC.Attribute;
using IoC.Extension;
using AgkCommons.Input.Gesture.Service;

namespace DronDonDon.Location.World.Dron
{
    public class DronController: MonoBehaviour,  IWorldObjectController<DronModel>
    {
        private Vector2 _virtualPosition=Vector2.zero;
        private Vector2 _containerPosition =Vector2.zero;
        private float _containerCoefficient=9;
        
        [Inject]
        private IGestureService _gestureService;
        
        public WorldObjectType ObjectType { get; private set; }
        public void Init(DronModel  model)
        {
            ObjectType = model.ObjectType;
            _gestureService.AddSwipeHandler(OnSwiped,false);
        }

        public void Update()
        {
           
        }
        
        private void OnSwiped(Swipe swipe)
        {
            MoveDron(SwipeToSector(swipe));
        }
        
        private int SwipeToSector(Swipe swipe)
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

        // Может ли дрон совершить манёвр, находясь в текущей позиции (столбце/строке/ячейке)
        private bool ValidateMovement(int sector)
        {
            Vector2Int fakePosition = Vector2Int.RoundToInt(_virtualPosition); 
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
                Debug.Log("[Swipe] Невозможно переместиться из текущей ячейки по этому свайпу");
                return;
            }
            Debug.Log("[Swipe] Старые координаты ячейки: " + _virtualPosition);
            _virtualPosition += virtualVectors[sector];
            Debug.Log("[Swipe] Новые координаты ячейки: " + _virtualPosition);
        }

        private void ShiftContainerPosition()
        {
            _containerPosition = _virtualPosition * _containerCoefficient;
            Debug.Log("[Swipe] Координаты в контейнере: " + _containerPosition);
        }

        public void MoveDron(int sector)
        {
            ShiftVirtualPosition(sector);
            ShiftContainerPosition();
            gameObject.transform.localPosition = _containerPosition;
        }
    }
}