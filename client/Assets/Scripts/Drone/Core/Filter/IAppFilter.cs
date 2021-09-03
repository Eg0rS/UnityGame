namespace Drone.Core.Filter
{
    public interface IAppFilter
    {
        void Run(AppFilterChain chain);
    }
}