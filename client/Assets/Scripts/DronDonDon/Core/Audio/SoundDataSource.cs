using AgkCommons.Sound.DataSource;
using ChatStories.Core.Audio.Service;
using IoC.Attribute;
using JetBrains.Annotations;
using UnityEngine;

namespace ChatStories.Core.Audio
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