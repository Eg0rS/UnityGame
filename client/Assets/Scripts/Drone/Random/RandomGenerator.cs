using Drone.Core.Service;
using Drone.Settings.Service;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Random
{
    public class RandomGenerator : MonoBehaviour, IRandomGenerator, IConfigurable
    {
        [Inject]
        private SettingsService _settingsService;
        public int RandomSeed { get; set; }

        public void InitGenerator()
        {
            UnityEngine.Random.InitState(RandomSeed);
        }

        public float NextInt()
        {
            return UnityEngine.Random.value;
        }

        public int Range(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public float Range(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public void Configure()
        {
            RandomSeed = _settingsService.GetSeed();
        }
    }
}