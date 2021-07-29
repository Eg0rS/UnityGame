using DronDonDon.Inventory.Service;
using IoC.Api;

namespace DronDonDon.Inventory.IoC
{
    public class InvntoryModel : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<InventoryService>(); 
            container.RegisterSingleton<InventoryRepository>();
        }
    }
}