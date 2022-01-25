using Adept.Logger;
using AgkCommons.IoC;
using Drone.Billing.IoC;
using Drone.Console.IoC;
using Drone.Core.Audio.IoC;
using Drone.Core.IoC;
using Drone.Core.UI.IoC;
using Drone.Descriptor.IoC;
using Drone.Inventory.IoC;
using Drone.Levels.IoC;
using Drone.Location.IoC;
using Drone.Location.Service.Control.Drone.Module;
using Drone.PowerUp.IoC;
using Drone.Settings.IoC;
using Drone.Shop.IoC;
using IoC;
using IoC.Api;
using IoC.Extension;

namespace Drone.Core.Filter
{
    public class IoCFilter : IAppFilter
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<IoCFilter>();

        public void Run(AppFilterChain chain)
        {
            _logger.Info("Start IoC initialization");
            IoCContainer container = chain.gameObject.AddComponent<IoCContainer>();
            MonoBehaviourExtension.SetDependencyInjector(container, container);
            AppContext.Container = container;
            Configure(container);

            chain.Next();
        }

        private void Configure(IIoCContainer context)
        {
            context.RegisterModule(new AgkCommonsModule());
            context.RegisterModule(new GameKitModule());
            context.RegisterModule(new ConsoleModule());
            context.RegisterModule(new AudioModule());
            context.RegisterModule(new UIModule());
            context.RegisterModule(new CoreModule());
            context.RegisterModule(new DescriptorModule());
            context.RegisterModule(new LocationModule());
            context.RegisterModule(new InventoryModule());
            context.RegisterModule(new LevelModule());
            context.RegisterModule(new SettingsModule());
            context.RegisterModule(new BillingModule());
            context.RegisterModule(new ShopModule());
            context.RegisterModule(new DroneModule());
            context.RegisterModule(new PowerUpModule());
        }
    }
}