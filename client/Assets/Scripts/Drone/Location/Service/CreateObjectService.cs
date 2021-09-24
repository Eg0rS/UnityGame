﻿using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Battery;
using Drone.Location.Model.BonusChips;
using Drone.Location.Model.Dron;
using Drone.Location.Model.Finish;
using Drone.Location.Model.Obstacle;
using Drone.Location.Model.ShieldBooster;
using Drone.Location.Model.SpeedBooster;
using Drone.Location.World;
using Drone.Location.World.Battery;
using Drone.Location.World.BonusChips;
using Drone.Location.World.Dron;
using Drone.Location.World.Finish;
using Drone.Location.World.Obstacle;
using Drone.Location.World.ShieldBooster;
using Drone.Location.World.SpeedBooster;
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
            _controllers[DRON] = new ControllerData(typeof(DronController), InitController<DronController, DronModel>);
            _controllers[OBSTACLE] = new ControllerData(typeof(ObstacleController), InitController<ObstacleController, ObstacleModel>);
            _controllers[BONUS_CHIPS] = new ControllerData(typeof(BonusChipsController), InitController<BonusChipsController, BonusChipsModel>);
            _controllers[SPEED_BUSTER] =
                    new ControllerData(typeof(SpeedBoosterController), InitController<SpeedBoosterController, SpeedBoosterModel>);
            _controllers[SHIELD_BUSTER] =
                    new ControllerData(typeof(ShieldBoosterController), InitController<ShieldBoosterController, ShieldBoosterModel>);
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