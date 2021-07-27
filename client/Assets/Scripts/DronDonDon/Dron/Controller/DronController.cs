using System.Collections.Generic;
using UnityEngine;

namespace DronDonDon.Dron.Controller
{
    public class DronController
    {
        private Vector2 _virtualPosition;
        private Vector2 _containerPosition;
        private float _containerCoefficient;

        public DronController()
        {
            _containerCoefficient = 1;
            _virtualPosition = Vector2.zero;
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
        }
    }
}