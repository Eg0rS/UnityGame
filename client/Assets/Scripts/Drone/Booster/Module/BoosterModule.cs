using Drone.Booster.Service;
using IoC.Api;
using IoC.Scope;

namespace Drone.Booster.Module
{
    public class BoosterModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<BoosterService>(null, ScopeType.SCREEN); //todo убрать в отдельный модуль
        }
    }
}