using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using DronDonDon.Core;
using DronDonDon.Location.Model;
using DronDonDon.Location.Model.BaseModel;
using DronDonDon.Location.Model.BonusChips;
using DronDonDon.Location.Model.Dron;
using DronDonDon.Location.Model.Finish;
using DronDonDon.Location.Model.Obstacle;
using DronDonDon.Location.Model.ShieldBooster;
using DronDonDon.Location.Model.SpeedBooster;
using DronDonDon.Location.UI;
using DronDonDon.Location.World.Dron;
using DronDonDon.Location.World.Dron.Descriptor;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

public struct DronStats
{
     public float _durability;
     public float _energy;
     public float _countChips;
}

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class WorldService: GameEventDispatcher
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject] private IoCProvider<OverlayManager> _overlayManager;

        [Inject] private UIService _uiService;

        public DronStats _dronStats;
        public void StartGame( DronDescriptor dronDescriptor)
        {
            _overlayManager.Require().HideLoadingOverlay(true);
            GameObject levelContainer = GameObject.Find($"Overlay");
            
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.ON_COLLISION, DronCollision);
            _dronStats._durability = dronDescriptor.Durability;
            _dronStats._countChips = 0;
            _dronStats._energy = dronDescriptor.Energy;
            
            _uiService.Create<DronStatsDialog>(UiModel
                .Create<DronStatsDialog>(_dronStats)
                .Container(levelContainer)).Done();
        }

        private void DronCollision(WorldObjectEvent worldObjectEvent)
        {
            GameObject _collisionObject = worldObjectEvent._collisionObject;
            switch (worldObjectEvent._collisionObject.GetComponent<PrefabModel>().ObjectType)
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

        private void Victory(FinishModel getComponent)
        {
       
        }

        private void OnTakeShield(ShieldBoosterModel getComponent)
        {
            Destroy(getComponent.gameObject);
        }

        private void OnTakeSpeed(SpeedBoosterModel getComponent)
        {
            Destroy(getComponent.gameObject);
        }

        private void OnTakeChip(BonusChipsModel getComponent)
        {
            _dronStats._countChips++;
            Destroy(getComponent.gameObject);
            UiUpdate();
        }

        private void OnDronCrash(ObstacleModel getComponent)
        {
            if (_dronStats._durability <= 0)
            {
                DronFailed();
            }
            _dronStats._durability -= getComponent.Damage;
            UiUpdate();
        }

        private void UiUpdate()
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.UI_UPDATE,
                _dronStats));
        }
        
        private void DronFailed()
        {
            
        }
    }
}