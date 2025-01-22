using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Audio
{
    [Serializable]
    public class AudioPlayer
    {
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private AudioSource _audioSource;
        
        [Space]
        [SerializeField] private float _volume;
        
        [Tooltip("x - min pitch; y = max pitch")]
        [SerializeField] private Vector2 _pitchRange = Vector2.one;

        public void PlayShot()
        {
            SetupAudioSource();
            _audioSource.PlayOneShot(GetRandomClip());
        }

        private void SetupAudioSource()
        {
            var pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.pitch = pitch;
            
            _audioSource.volume = _volume;
        }

        private AudioClip GetRandomClip()
        {
            return _audioClips[Random.Range(0, _audioClips.Length)];
        }
    }
}
