using AgkCommons.Sound.DataSource;
using Drone.Core.Audio.Service;
using IoC.Attribute;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Core.Audio
{
    [UsedImplicitly]
    public class SoundDataSource : ISoundDataSource
    {
        [Inject]
        private AudioService _audioService;

        public void PlaySound(AudioClip clip)
        {
            _audioService.PlaySound(clip);
        }
    }
}