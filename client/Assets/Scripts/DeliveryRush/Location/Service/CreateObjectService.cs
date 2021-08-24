using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.BaseModel;
using DeliveryRush.Location.Model.Battery;
using DeliveryRush.Location.Model.BonusChips;
using DeliveryRush.Location.Model.Dron;
using DeliveryRush.Location.Model.Finish;
using DeliveryRush.Location.Model.Obstacle;
using DeliveryRush.Location.Model.ShieldBooster;
using DeliveryRush.Location.Model.SpeedBooster;
using DeliveryRush.Location.World;
using DeliveryRush.Location.World.Battery;
using DeliveryRush.Location.World.BonusChips;
using DeliveryRush.Location.World.Dron;
using DeliveryRush.Location.World.Finish;
using DeliveryRush.Location.World.Obstacle;
using DeliveryRush.Location.World.ShieldBooster;
using DeliveryRush.Location.World.SpeedBooster;
using UnityEngine;
using static DeliveryRush.Location.Model.WorldObjectType;
using AppContext = IoC.AppContext;

namespace DeliveryRush.Location.Service
{
    [Injectable]
    public class CreateObjectService
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<CreateObjectService>();
        
        private readonly Dictionary<WorldObjectType, ControllerData> _controllers = new Dictionary<WorldObjectType, ControllerData>();

        public CreateObjectService()
        {
           _controllers[DRON] = new ControllerData(typeof(DronController), InitController<DronController, DronModel>);
            _controllers[OBSTACLE] = new ControllerData(typeof(ObstacleController), InitController<ObstacleController, ObstacleModel>);
           _controllers[BONUS_CHIPS] = new ControllerData(typeof(BonusChipsController), InitController<BonusChipsController, BonusChipsModel>);
            _controllers[SPEED_BUSTER] = new ControllerData(typeof(SpeedBoosterController), InitController<SpeedBoosterController, SpeedBoosterModel>);
           _controllers[SHIELD_BUSTER] = new ControllerData(typeof(ShieldBoosterController), InitController<ShieldBoosterController, ShieldBoosterModel>);
            _controllers[Battery] = new ControllerData(typeof(BatteryController), InitController<BatteryController, BatteryModel>);
            _controllers[FINISH] = new ControllerData(typeof(FinishController), InitController<FinishController, FinishModel>);
        }
        public Component AttachController(PrefabModel model)
        {
            if (model.ObjectType == NONE) {
                throw new ArgumentException("Prefab model dont contains propper type " + model.ObjectType);
            }
            if (!_controllers.ContainsKey(model.ObjectType)) {
                throw new ArgumentException("Invalid objectType " + model.ObjectType);
            }
            ControllerData controllerData = _controllers[model.ObjectType];
            Component controller = model.gameObject.AddComponent(controllerData.Controller);
            AppContext.Inject(controller);
            controllerData.Initializer.Invoke(controller, model);

            return controller;
        }
        private static void InitController<T, TS>(object controller, PrefabModel model)
            where TS : PrefabModel
            where T : IWorldObjectController<TS>
        {
            try {
                ((T) controller).Init((TS) model);
            } catch (InvalidCastException e) {
                _logger.Error("Error while init controller. PrefabModel: " + model.ObjectType, e);
            } catch (NullReferenceException e) {
                _logger.Error("Error while init controller. Cant find Controler for PrefabModel: " + model.ObjectType, e);
            }
        }
    }
    internal class ControllerData
    {
        private readonly Type _controller;
        private readonly Action<object, PrefabModel> _initializer;
        public ControllerData(Type controller, Action<object, PrefabModel> initializer)
        {
            _controller = controller;
            _initializer = initializer;
        }
        public Type Controller
        {
            get { return _controller; }
        }
        public Action<object, PrefabModel> Initializer
        {
            get { return _initializer; }
        }
    }
}