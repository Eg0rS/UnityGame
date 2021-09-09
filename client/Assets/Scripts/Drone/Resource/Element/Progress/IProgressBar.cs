using JetBrains.Annotations;

 namespace Drone.Resource.Element.Progress
{
    public interface IProgressBar
    {
        [UsedImplicitly]
        int Progress { get; set; }
        [UsedImplicitly]
        float Speed { get; set; }
        [UsedImplicitly]
        bool Completed { get; }
        [CanBeNull]
        [UsedImplicitly]
        string Label { get; set; }
    }
}