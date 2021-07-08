using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkCommons.Resources;
using IoC.Attribute;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

namespace ChatStories.Core.Audio.Service
{    
    [Injectable]
    public class MusicService : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MusicService>();

        private string _lastMusicPrefab;

        [Inject]
        private ResourceService _resourceService;
        [Inject]
        private AudioService _audioService;

        [PublicAPI]
        public void PlayMusic(string prefab)
        {
            _lastMusicPrefab = prefab;
            _resourceService.LoadResource<AudioClip>(_lastMusicPrefab, MusicPrefabLoaded, _lastMusicPrefab);
        }

        private void MusicPrefabLoaded(AudioClip loadedObject, object[] loadParameters)
        {
            string prefabName = loadParameters[0] as string;
            if (loadedObject == null) {
                _logger.Warn("Music not loaded. id=" + prefabName);
                return;
            }

            if (!_lastMusicPrefab.Equals(prefabName)) {
                return;
            }

            _audioService.PlayMusic(loadedObject);
        }

        [PublicAPI]
        public void PlayList(List<string> tracks, bool shuffle = true)
        {
            if (tracks.Count == 0) {
                FadeAndStop();
                return;
            }

            if (tracks.Count == 1 || !shuffle) {
                PlayMusic(tracks[0]);
                return;
            }

            Random rnd = new Random();
            int index = rnd.Next(tracks.Count);
            PlayMusic(tracks[index]);
        }


        [PublicAPI]
        public void StopMusic()
        {
            _audioService.StopMusic();
        }
        
        public void FadeAndStop()
        {
            _audioService.FadeAndStopMusic();
        }
    }
}