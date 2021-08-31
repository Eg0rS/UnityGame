using IoC.Api;
using Plugins.IngameDebugConsole.Scripts.Console.Service;

namespace DeliveryRush.Console.IoC
{
    public class ConsoleModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<IConsoleService, IngameConsoleService>();
            
        }
    }
}