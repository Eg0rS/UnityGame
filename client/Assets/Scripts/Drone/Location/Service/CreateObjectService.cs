using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Battery;
using Drone.Location.Model.BonusChips;
using Drone.Location.Model.Drone;
using Drone.Location.Model.Finish;
using Drone.Location.Model.Magnet;
using Drone.Location.Model.Obstacle;
using Drone.Location.Model.ShieldBooster;
using Drone.Location.Model.SpeedBooster;
using Drone.Location.Model.Spline;
using Drone.Location.Model.X2Booster;
using Drone.Location.Service.Control;
using Drone.Location.Service.Control.Drone;
using Drone.Location.Service.Control.Battery;
using Drone.Location.Service.Control.BonusChips;
using Drone.Location.Service.Control.Finish;
using Drone.Location.Service.Control.Magnet;
using Drone.Location.Service.Control.Obstacle;
using Drone.Location.Service.Control.ShieldBooster;
using Drone.Location.Service.Control.SpeedBooster;
using Drone.Location.Service.Control.Spline;
using Drone.Location.Service.Control.X2Booster;
using UnityEngine;
using static Drone.Location.Model.WorldObjectType;
using AppContext = IoC.AppContext;

namespace Drone.Location.Service
{
    [Injectable]
    public class CreateObjectService
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<CreateObjectService>();

        private readonly Dictionary<WorldObjectType, ControllerData> _controllers = new Dictionary<WorldObjectType, ControllerData>();

        public CreateObjectService()
        {
            _controllers[PLAYER] = new ControllerData(typeof(DroneController), InitController<DroneController, DroneModel>);
            _controllers[OBSTACLE] = new ControllerData(typeof(ObstacleController), InitController<ObstacleController, ObstacleModel>);
            _controllers[BONUS_CHIPS] = new ControllerData(typeof(BonusChipsController), InitController<BonusChipsController, BonusChipsModel>);
            _controllers[SPEED_BOOSTER] =
                    new ControllerData(typeof(SpeedBoosterController), InitController<SpeedBoosterController, SpeedBoosterModel>);
            _controllers[SHIELD_BOOSTER] =
                    new ControllerData(typeof(ShieldBoosterController), InitController<ShieldBoosterController, ShieldBoosterModel>);
            _controllers[X2_BOOSTER] =
                    new ControllerData(typeof(X2BoosterController), InitController<X2BoosterController, X2BoosterModel>);
            _controllers[MAGNET_BOOSTER] =
                    new ControllerData(typeof(MagnetBoosterController), InitController<MagnetBoosterController, MagnetBoosterModel>);
            _controllers[BATTERY] = new ControllerData(typeof(BatteryController), InitController<BatteryController, BatteryModel>);
            _controllers[FINISH] = new ControllerData(typeof(FinishController), InitController<FinishController, FinishModel>);
            _controllers[SPLINE] = new ControllerData(typeof(SplineController), InitController<SplineController, SplineModel>);
            
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