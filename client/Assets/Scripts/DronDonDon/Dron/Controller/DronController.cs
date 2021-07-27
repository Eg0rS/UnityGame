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
        private bool ValidateVirtualPosition(int sector)
        {
            return sector switch
            {
                0 => (int)_virtualPosition.y != 1,
                1 => (int)_virtualPosition.x != 1 && (int)_virtualPosition.y != 1,
                2 => (int)_virtualPosition.x != 1,
                3 => (int)_virtualPosition.x != 1 && (int)_virtualPosition.y != -1,
                4 => (int)_virtualPosition.y != -1,
                5 => (int)_virtualPosition.x != -1 && (int)_virtualPosition.y != -1,
                6 => (int)_virtualPosition.x != -1,
                7 => (int)_virtualPosition.x != -1 && (int)_virtualPosition.y != 1,
                _ => false
            };
        }

        private void ShiftVirtualPosition(int sector)
        {
            if (!ValidateVirtualPosition(sector))
                return;
            _virtualPosition += virtualVectors[sector];
        }

        private void ShiftRealPosition()
        {
            _containerPosition = _virtualPosition * _containerCoefficient;
        }

        public void MoveDron(int sector)
        {
            ShiftVirtualPosition(sector);
            ShiftRealPosition();
        }
    }
}