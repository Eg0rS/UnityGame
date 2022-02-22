namespace Drone.Core.Service
{
    public interface IRandomGenerator
    {
        int RandomSeed { get; set; }
        float NextInt();
        float Range(float min, float max);
        int Range(int min, int max);


    }
}