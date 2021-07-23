namespace DronDonDon.Dron.Model
{
    public class DronModel
    {
        private sbyte _containerCellPositionX;
        private sbyte _containerCellPositionY;
        private int _energy;
        private int _durability;
        private int _chipsCollected;
        
        public int Energy
        {
            get => _energy;
            set => _energy += value;
        }

        public int Durability
        {
            get => _durability;
            set => _durability += value;
        }

        public int Chips
        {
            get => _chipsCollected;
            set => _chipsCollected += value;
        }
        
        // Проверяем, может ли дрон совершить указанный манёвр, находясь с текущей ячейке
        private bool CheckChangePosition(SwipeType type)
        {
            switch (type)
            {
                // Если дрон в верхней строке и свайп вверх
                case SwipeType.UP:
                    if (_containerCellPositionY == 1)
                    {
                        return false;
                    }
                    return true;
                
                // Если дрон в нижней строке и свайп вниз
                case SwipeType.DOWN:
                    if (_containerCellPositionY == -1)
                    {
                        return false;
                    }
                    return true;
                
                // Если дрон в левом столбце и свайп влево
                case SwipeType.LEFT:
                    if (_containerCellPositionX == -1)
                    {
                        return false;
                    }
                    return true;
                    
                // Если дрон в правом столбце и свайп вправо
                case SwipeType.RIGHT:
                    if (_containerCellPositionX == 1)
                    {
                        return false;
                    }
                    return true;
                    
                // Если дрон находится в левой верхней ячейке и свайп влево вверх
                case SwipeType.UP_LEFT:
                    if (_containerCellPositionX == -1 && _containerCellPositionY == 1)
                    {
                        return false;
                    }
                    return true;
                
                // Если дрон находится в правой верхней ячейке и свайп вправо вверх
                case SwipeType.UP_RIGHT:
                    if (_containerCellPositionX == 1 && _containerCellPositionY == 1)
                    {
                        return false;
                    }
                    return true;
                
                // Если дрон находится в левой нижней ячейке и свайп влево вниз
                case SwipeType.DOWN_LEFT:
                    if (_containerCellPositionX == -1 && _containerCellPositionY == -1)
                    {
                        return false;
                    }
                    return true;
                
                // Если дрон находится в правой нижней ячейке и свайп вправо вниз
                case SwipeType.DOWN_RIGHT:
                    if (_containerCellPositionX == 1 && _containerCellPositionY == -1)
                    {
                        return false;
                    }
                    return true;
                default: return false;
            }
        }

        // Если можно, меняем координаты
        /*
         * TODO: покачивать дрон в сторону неудавшегося перемещения 
         */   
        public void MoveDron(SwipeType type)
        {
            if (CheckChangePosition(type))
            {
                switch (type)
                {
                    case SwipeType.UP:
                        _containerCellPositionY += 1;
                        break;
                
                    case SwipeType.DOWN:
                        _containerCellPositionY -= 1;
                        break;
                
                    case SwipeType.LEFT:
                        _containerCellPositionX -= 1;
                        break;
                
                    case SwipeType.RIGHT:
                        _containerCellPositionX += 1;
                        break;
                
                    // Движение ВЛЕВО ВВЕРХ
                    case SwipeType.UP_LEFT:
                        // Если дрон находится в левом столбце (кроме верхней ячейки), мы можем двигать его только вверх
                        if (_containerCellPositionX == -1 && _containerCellPositionY < 1)
                        {
                            _containerCellPositionY += 1;
                        }
                        // Если дрон находится в верхней строке (кроме левой ячейки), мы можем двигать его только влево
                        else if (_containerCellPositionX > -1 && _containerCellPositionY == 1)
                        {
                            _containerCellPositionX -= 1;
                        }
                        // иначе двигаем и влево, и вверх
                        else
                        {
                            _containerCellPositionX -= 1;
                            _containerCellPositionY += 1;
                        }
                        break;
                
                    // Движение ВПРАВО ВВЕРХ
                    case SwipeType.UP_RIGHT:
                        // Если дрон находится в правом столбце (кроме верхней ячейки), мы можем двигать его только вверх
                        if (_containerCellPositionX == 1 && _containerCellPositionY < 1)
                        {
                            _containerCellPositionY += 1;
                        }
                        // Если дрон находится в верхней строке (кроме правой ячейки), мы можем двигать его только вправо
                        else if (_containerCellPositionX < 1 && _containerCellPositionY == 1)
                        {
                            _containerCellPositionX += 1;
                        }
                        // иначе двигаем и вправо, и вверх
                        else
                        {
                            _containerCellPositionX += 1;
                            _containerCellPositionY += 1;
                        }
                        break;
                
                    // Движение ВЛЕВО ВНИЗ
                    case SwipeType.DOWN_LEFT:
                        // Если дрон находится в левом столбце (кроме нижней ячейки), мы можем двигать его только вниз
                        if (_containerCellPositionX == -1 && _containerCellPositionY > -1)
                        {
                            _containerCellPositionY -= 1;
                        }
                        // Если дрон находится в нижней строке (кроме левой ячейки), мы можем двигать его только влево
                        else if (_containerCellPositionX > -1 && _containerCellPositionY == -1)
                        {
                            _containerCellPositionX -= 1;
                        }
                        // иначе двигаем и влево, и вниз
                        else
                        {
                            _containerCellPositionX -= 1;
                            _containerCellPositionY -= 1;
                        }
                        break;
                    
                    // Движение ВПРАВО ВНИЗ
                    case SwipeType.DOWN_RIGHT:
                        // Если дрон находится в правом столбце (кроме нижней ячейки), мы можем двигать его только вниз
                        if (_containerCellPositionX == 1 && _containerCellPositionY > -1)
                        {
                            _containerCellPositionY -= 1;
                        }
                        // Если дрон находится в нижней строке (кроме правой ячейки), мы можем двигать его только вправо
                        else if (_containerCellPositionX < 1 && _containerCellPositionY == -1)
                        {
                            _containerCellPositionX += 1;
                        }
                        // иначе...
                        else
                        {
                            _containerCellPositionX += 1;
                            _containerCellPositionY -= 1;
                        }
                        break;
                }
            }
        }
    }
}