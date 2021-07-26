using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Extension;
using AgkCommons.Resources;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using RSG;
using UnityEngine;
using UnityEngine.AI;
using DronDonDon.Location.Model;
using static DronDonDon.Location.Model.WorldObjectType;
using DronDonDon.Location.Model.BaseModel;
using AppContext = IoC.AppContext;
using Object = UnityEngine.Object;
using DronDonDon.Location.World.Object;
using DronDonDon.Location.Model.Object;
using DronDonDon.Location.World;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class CreateObjectService
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<CreateObjectService>();
        
        private readonly Dictionary<WorldObjectType, ControllerData> _controllers = new Dictionary<WorldObjectType, ControllerData>();

        [Inject]
        private ResourceService _resourceService;
        
        public CreateObjectService()
        {
           // _controllers[DRON] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
            _controllers[OBSTACLE] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
          //  _controllers[BONUSCHIPS] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
          //  _controllers[SPEEDBUSTER] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
           // _controllers[SHIELDBUSTER] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
          //  _controllers[START] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
          //  _controllers[FINISH] = new ControllerData(typeof(ObjectController), InitController<ObjectController, ObjectModel>);
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