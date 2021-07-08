using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkCommons.Resources;
using IoC.Attribute;
using JetBrains.Annotations;
using UnityEngine;

namespace DronDonDon.Core.Audio.Service
{
    [Injectable]
    public class SoundService : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<SoundService>();

        private const string EMBEDED_NAME = "embeded";

        [Inject]
        private readonly ResourceService _resourceService;
        [Inject]
        private readonly AudioService _audioService;

        private readonly IDictionary<string, AudioClip> _soundClips = new Dictionary<string, AudioClip>();

        private void Start()
        {
            LoadEmbededSounds();
        }

        [PublicAPI]
        public void PlaySound(string soundName, bool loop = false)
        {
           
            string actualSoundName = soundName.ToLower();

            if (!_soundClips.ContainsKey(actualSoundName)) {
                _logger.Warn("Sound not found: name=" + actualSoundName);
                return;
            }
            
            AudioClip clip = _soundClips[actualSoundName];
            _audioService.PlaySound(clip, loop);
        }
    
        [PublicAPI]
        public void StopSound()
        {
            _audioService.StopSound();
        }
        [PublicAPI]
        public void PauseSound()
        {
            _audioService.PauseSound();
        }
        [PublicAPI]
        public void ResumeSound()
        {
            _audioService.ResumeSound();
        }
        [PublicAPI]
        public void RemoveSound()
        {
            _audioService.RemoveSound();
        }
        private void LoadEmbededSounds()
        {
            List<string> embededSounds = new List<string> {
                    /*SoundPrefabs.PLAYER_HEARTBEAT,
                    SoundPrefabs.PLAYER_FREQUENT_BREATHING,*/
            };
            foreach (string assetPath in embededSounds) {
                string soundName = assetPath.ToLower();
                string prefab = soundName + "@" + EMBEDED_NAME;
                _resourceService.LoadAudioClip(prefab, (loadedClip, loadedParams) => {
                    if (loadedClip == null) {
                        _logger.Warn("Sound not loaded: prefab=" + prefab);
                        return;
                    }
                    if (!_soundClips.ContainsKey(soundName)) {
                        _soundClips.Add(soundName, loadedClip);
                    }
                });
            }
        }
    }
}