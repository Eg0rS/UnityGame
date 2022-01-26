using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Finish;
using Drone.Location.Model.Obstacle;
using Drone.Location.Model.Player;
using Drone.Location.Model.Spline;
using Drone.Location.Model.StartPlatform;
using Drone.Location.World;
using Drone.Location.World.Drone;
using Drone.Location.World.Finish;
using Drone.Location.World.Obstacle;
using Drone.Location.World.Spline;
using Drone.Location.World.StartPlatform;
using UnityEngine;
using static Drone.Location.Model.WorldObjectType;
using AppContext = IoC.AppContext;

namespace Drone.Location.Service
{
    [Injectable]
    public class CreateLocationObjectService
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<CreateLocationObjectService>();
        
        private readonly Dictionary<WorldObjectType, ControllerData> _controllers = new Dictionary<WorldObjectType, ControllerData>();
       

        public CreateLocationObjectService()
        {
            _controllers[START_PLATFORM] =
                    new ControllerData(typeof(StartPlatformController), InitController<StartPlatformController, StartPlatformModel>);
            _controllers[PLAYER] = new ControllerData(typeof(PlayerController), InitController<PlayerController, PlayerModel>);

            _controllers[OBSTACLE] = new ControllerData(typeof(ObstacleController), InitController<ObstacleController, ObstacleModel>);
            _controllers[FINISH] = new ControllerData(typeof(FinishController), InitController<FinishController, FinishModel>);
            _controllers[SPLINE] = new ControllerData(typeof(SplineController), InitController<SplineController, SplineModel>);
            _controllers[SPLINE_WALKER] =
                    new ControllerData(typeof(SplineWalkerController), InitController<SplineWalkerController, SplineWalkerModel>);
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