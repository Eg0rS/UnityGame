using Adept.Logger;
using AgkCommons.IoC;
using DeliveryRush.Billing.IoC;
using DeliveryRush.Console.IoC;
using DeliveryRush.Core.Audio.IoC;
using DeliveryRush.Core.IoC;
using DeliveryRush.Core.UI.IoC;
using DeliveryRush.Descriptor.IoC;
using DeliveryRush.Resource.IoC;
using DeliveryRush.Inventory.IoC;
using DeliveryRush.Location.IoC;
using DeliveryRush.Location.World.Dron.Module;
using DeliveryRush.Settings.IoC;
using DeliveryRush.Shop.IoC;
using IoC;
using IoC.Api;
using IoC.Extension;

namespace DeliveryRush.Core.Filter
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
            context.RegisterModule(new SettingsModule());
            context.RegisterModule(new LevelsModule());
            context.RegisterModule(new BillingModule());
            context.RegisterModule(new ShopModule());
            context.RegisterModule(new InventoryModule());
            context.RegisterModule(new DronModule());
        }
    }
}