using System.Linq;
using Drone.Billing.Service;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Levels.Service;
using Drone.Location.Service.Game;
using Drone.Location.Service.Game.Event;
using Drone.Location.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service
{
    public class RespawnService : MonoBehaviour, IWorldServiceInitiable
    {
        [Inject]
        private RespawnDescriptors _respawnDescriptors;
        [Inject]
        private LevelService _levelService;
        [Inject]
        private DroneWorld _droneWorld;
        [Inject]
        private GameService _gameService;
        [Inject]
        private BillingService _billingService;
        private int _levelRespawnCount;

        public void Init()
        {
            _levelRespawnCount = 0;
            _droneWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
        }

        private void OnEndGame(InGameEvent obj)
        {
            if (obj.EndGameReason == EndGameReasons.CRUSH) {
                _levelRespawnCount++;
            }
        }

        public int SetRespawnPrice()
        {
            if (_levelService.GetPlayerProgressModel().RespawnCount <= 5) {
                return -1;
            }
            if (_levelRespawnCount > 3) {
                return -2;
            }
            return _respawnDescriptors.Descriptors.First(x => x.CollisionCount == _levelRespawnCount).Price;
        }

        public bool BuyRespawn()
        {
            float respawnPrice = _respawnDescriptors.Descriptors.First(x => x.CollisionCount == _levelRespawnCount).Price;
            if (!(_gameService.ChipsCount + _billingService.GetCreditsCount() <= respawnPrice)) {
                return false;
            }
            float temp = respawnPrice - _gameService.ChipsCount;
            if (temp <= 0) {
                _gameService.ChipsCount -= (int) respawnPrice;
            } else {
                _billingService.SetCreditsCount(_billingService.GetCreditsCount() - (int) respawnPrice + _gameService.ChipsCount);
                _gameService.ChipsCount = 0;
            }
            return true;
        }
    }
}