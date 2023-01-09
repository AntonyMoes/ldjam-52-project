using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts {
    public class SoundController : SingletonBehaviour<SoundController> {
        [SerializeField] private GameObject _sounds;
        [SerializeField] private AudioSource _music;

        [SerializeField] private AudioClip _buttonPressClip;
        public AudioClip ButtonPressClip => _buttonPressClip;
        [SerializeField] private AudioClip _levelStartClip;
        public AudioClip LevelStartClip => _levelStartClip;
        [SerializeField] private AudioClip _levelWinClip;
        public AudioClip LevelWinClip => _levelWinClip;
        [SerializeField] private AudioClip _treePlaceClip;
        public AudioClip TreePlaceClip => _treePlaceClip;
        [SerializeField] private AudioClip _treeFallClip;
        public AudioClip TreeFallClip => _treeFallClip;
        [SerializeField] private AudioClip _plantPlace1Clip;
        public AudioClip PlantPlace1Clip => _plantPlace1Clip;
        [SerializeField] private AudioClip _plantPlace2Clip;
        public AudioClip PlantPlace2Clip => _plantPlace2Clip;
        [SerializeField] private AudioClip _plantPlace3Clip;
        public AudioClip PlantPlace3Clip => _plantPlace3Clip;

        [SerializeField] private AudioClip _soundTrackClip;
        public AudioClip SoundTrackClip => _soundTrackClip;


        private readonly List<AudioSource> _soundSources = new List<AudioSource>();

        public AudioSource PlaySound(AudioClip sound, float volume = 1f, float pitch = 1f) {
            var source = _soundSources.FirstOrDefault(ss => !ss.isPlaying);
            if (source == null) {
                source = _sounds.AddComponent<AudioSource>();
                _soundSources.Add(source);
            }

            source.clip = sound;
            source.Play();
            source.volume = volume;
            source.pitch = pitch;

            return source;
        }

        public AudioSource PlayMusic(AudioClip music, float volume = 1f) {
            _music.clip = music;
            _music.Play();
            _music.volume = volume;

            return _music;
        }
    }
}
