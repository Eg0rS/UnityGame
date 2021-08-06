using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
using IoC.Attribute;
using IoC.Util;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class EventService: GameEventDispatcher
    {
        public void StartGame(string levelPrefabName)
        {
            
        }
       
    }
}