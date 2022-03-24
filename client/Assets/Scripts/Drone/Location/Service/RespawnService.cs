using System.Collections;
using System.Linq;
using AgkUI.Dialog.Service;
using Drone.Billing.Service;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.LevelMap.LevelDialogs;
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
        private const float TIME_FOR_DEAD = 0.3f;
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
        [Inject]
        private DialogManager _dialogManager;

        private int _currentRespawnCount;
        private int _respawnPrice;

        public void Init()
        {
            _currentRespawnCount = 0;
            _droneWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
        }

        private void OnEndGame(InGameEvent obj)
        {
            if (obj.EndGameReason == EndGameReasons.CRUSH) {
                _currentRespawnCount++;
                StartCoroutine(GameFailed());
            }
        }

        private IEnumerator GameFailed()
        {
            SetRespawnPrice();
            yield return new WaitForSeconds(TIME_FOR_DEAD);
            _dialogManager.Show<RespawnDialog>(_respawnPrice);
        }

        private void SetRespawnPrice()
        {
            if (_levelService.GetPlayerProgressModel().RespawnCount <= 5) {
                _respawnPrice = 0;
                return;
            }
            RespawnDescriptor descriptor = _respawnDescriptors.Descriptors.FirstOrDefault(x => x.CollisionCount == _currentRespawnCount)
                                           ?? _respawnDescriptors.Descriptors.First(x => x.CollisionCount
                                                                                         == _respawnDescriptors.Descriptors.Max(respawnDescriptor =>
                                                                                                 respawnDescriptor.CollisionCount));
            _respawnPrice = descriptor.Price;
        }

        public bool BuyRespawn()
        {
            float respawnPrice = _respawnDescriptors.Descriptors.First(x => x.CollisionCount == _currentRespawnCount).Price;
            if ((_gameService.ChipsCount + _billingService.GetCreditsCount() <= respawnPrice)) {
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