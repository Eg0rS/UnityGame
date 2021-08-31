using DeliveryRush.Core.Configurations;
using IoC.Attribute;
using IoC.Util;
using Plugins.IngameDebugConsole.Scripts.Console.Service;

namespace DeliveryRush.Core.Filter
{
    public class ConsoleFilter : IAppFilter
    {
        [Inject]
        private IoCProvider<IConsoleService> _console;

        public void Run(AppFilterChain chain)
        {
            if (Config.ShowConsole) {
                _console.Require().Create();
            }
            chain.Next();
        }
    }
}