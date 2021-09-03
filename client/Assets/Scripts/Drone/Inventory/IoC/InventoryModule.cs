using Drone.Inventory.Service;
using IoC.Api;

namespace Drone.Inventory.IoC
{
    public class InventoryModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<InventoryService>();
            container.RegisterSingleton<InventoryRepository>();
        }
    }
}