using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgkCommons.CodeStyle;
using AgkCommons.Resources;
using IoC.Attribute;
using IoC.Extension;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

namespace DronDonDon.Core.Audio.Service
{
    [Injectable]
    public class AudioService : MonoBehaviour
    {
        private readonly List<string> _pathMusic = new List<string> {
       
        };
        private const int START_POSITION = 0;
        [SerializeField]
        private AudioSource _music1Source;

        [SerializeField]
        private AudioSource _music2Source;
        [SerializeField]
        private AudioListener _menuAudioListener;
        [SerializeField]
        [Range(1, 10)]
        private float _musicCrossFadeRate = 3f;

        [SerializeField]
        private AudioSource _soundSource;

        [Inject]
        private ResourceService _resourceService;

        private List<AudioClip> _availableAudioClips;
        private List<int> _playList;
        private AudioSource _activeMusic;
        private AudioSource _inactiveMusic;

        private bool _musicCrossFading;

        private bool _musicMute;
        private bool _sound3D;

        private bool _soundMute;
        private bool _isPlaying;
        private int _currentAudioClip;

        private void Awake()
        {
            this.InjectAll();
            InitClipList();
            _playList = new List<int>();
            _activeMusic = _music1Source;
            _inactiveMusic = _music2Source;
        }

        private void InitClipList()
        {
            _availableAudioClips = new List<AudioClip>();
            foreach (string s in _pathMusic) {
                _resourceService.LoadResource<AudioClip>(s).Then(ac => _availableAudioClips.Add(ac)).Done();
            }
        }

        private void Update()
        {
            if (_soundSource.isPlaying) {
                return;
            }
            PlayDefaultList();
        }

        private void PlayDefaultList()
        {
            if (_playList.Count == 0) {
                _playList = GetRandomPlayList(_availableAudioClips.Count);
            }
            _currentAudioClip = _playList[START_POSITION];
            _playList.RemoveAt(START_POSITION);
            PlaySound(_availableAudioClips[_currentAudioClip]);
        }

        private List<int> GetRandomPlayList(int numberOfElement)
        {
            Random random = new Random();
            HashSet<int> numbers = new HashSet<int>();
            while (numbers.Count < numberOfElement)
            {
                numbers.Add(random.Next(0, numberOfElement));
            }
            return numbers.ToList();
        }

        public void PlaySound(AudioClip clip, bool loop = false)
        {
            // ReSharper disable once MergeSequentialChecks
            if (ReferenceEquals(_soundSource, null) || ReferenceEquals(_soundSource.gameObject, null)) {
                return;
            }
            _soundSource.clip = clip;
            if (!loop) {
                _soundSource.PlayOneShot(clip);
                return;
            }
            _soundSource.loop = true;
            _soundSource.Play();
        }

        public void RemoveSound()
        {
            _soundSource.clip = null;
        }

        public void PauseSound()
        {
            _soundSource.Pause();
        }

        public void ResumeSound()
        {
            _soundSource.UnPause();
        }

        public void PlayMusic(AudioClip clip, float offset = 0f)
        {
            if (_musicCrossFading) {
                return;
            }
            StartCoroutine(CrossFadeMusic(clip, offset));
        }

        /*public void PlayDefaultMusic()
        {
            if (_activeMusic.clip == null) {
                return;
            }
            _activeMusic.PlayOneShot(_activeMusic.clip);
        }*/
        [PublicAPI]
        public void EnableMenuAudioListener()
        {
            _menuAudioListener.gameObject.SetActive(true);
        }

        [PublicAPI]
        public void DisableMenuAudioListener()
        {
            _menuAudioListener.gameObject.SetActive(false);
        }

        [PublicAPI]
        public void UnMute()
        {
            AudioListener.volume = 1.0f;
        }

        [PublicAPI]
        public void Mute()
        {
            AudioListener.volume = 0f;
        }

        [PublicAPI]
        public void StopMusic()
        {
            _activeMusic.Stop();
            _inactiveMusic.Stop();
        }

        [PublicAPI]
        public void StopSound()
        {
            _soundSource.Stop();
        }

        public void FadeAndStopMusic()
        {
            if (_musicCrossFading) {
                return;
            }
            StartCoroutine(FadeMusic());
        }

        private IEnumerator FadeMusic()
        {
            _musicCrossFading = true;

            float scaleRate = 1 / _musicCrossFadeRate;
            while (_activeMusic.volume > 0) {
                _activeMusic.volume -= scaleRate * Time.deltaTime;
                yield return null;
            }
            _activeMusic.Stop();
            _musicCrossFading = false;
        }

        private IEnumerator CrossFadeMusic(AudioClip clip, float offset)
        {
            _musicCrossFading = true;
            _inactiveMusic.clip = clip;
            _inactiveMusic.volume = 0;
            _inactiveMusic.time = offset;
            _activeMusic.mute = _musicMute;
            _inactiveMusic.Play();

            float scaleRate = 1 / _musicCrossFadeRate;

            while (_activeMusic.volume > 0) {
                _activeMusic.volume -= scaleRate * Time.deltaTime;
                _inactiveMusic.volume += scaleRate * Time.deltaTime;

                yield return null;
            }

            AudioSource temp = _activeMusic;
            _activeMusic = _inactiveMusic;
            _activeMusic.volume = 1;

            _inactiveMusic = temp;
            _inactiveMusic.Stop();

            _musicCrossFading = false;
        }

        [PublicAPI]
        public bool SoundMute
        {
            get { return _soundMute; }
            set
            {
                _soundMute = value;
                if (_soundSource != null) {
                    _soundSource.mute = _soundMute;
                }
            }
        }

        [PublicAPI]
        public bool MusicMute
        {
            get { return _musicMute; }
            set
            {
                _musicMute = value;
                if (_music1Source != null) {
                    _music1Source.mute = _musicMute;
                }
                if (_music2Source != null) {
                    _music2Source.mute = _musicMute;
                }
            }
        }
    }
}