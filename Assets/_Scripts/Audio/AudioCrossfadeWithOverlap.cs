using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace _Scripts.Audio
{
    public class AudioCrossfadeWithOverlap : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource1;
        [SerializeField] private AudioSource _audioSource2; 
        [SerializeField] private float _crossfadeDuration = 2f;
        [SerializeField] private float _overlapTime = 1f;

        private bool _isPlayingSource1 = true;
        
        private Coroutine _currentCoroutine;
        private void Start()
        {
            _audioSource1.clip.LoadAudioData();
            _audioSource2.clip.LoadAudioData();
        }

        [Button]
        public void _StartCrossfade()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
                
            if (_isPlayingSource1)
            {
                _currentCoroutine = StartCoroutine(Crossfade(_audioSource1, _audioSource2));
            }
            else
            {
                _currentCoroutine = StartCoroutine(Crossfade(_audioSource2, _audioSource1));
            }

            _isPlayingSource1 = !_isPlayingSource1;
        }

        private IEnumerator Crossfade(AudioSource fromSource, AudioSource toSource)
        {
            // Завантажуємо аудіо, якщо воно не готове
            if (!fromSource.clip.preloadAudioData)
            {
                fromSource.clip.LoadAudioData();
            }

            if (!toSource.clip.preloadAudioData)
            {
                toSource.clip.LoadAudioData();
            }

            // Ініціалізація
            var timer = 0f;

            toSource.volume = 0;
            toSource.Play();

            // Фаза перекриття
            while (timer < _overlapTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // Фаза плавного кросфейду
            timer = 0f;

            while (timer < _crossfadeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / _crossfadeDuration;

                fromSource.volume = Mathf.Lerp(1f, 0f, t);
                toSource.volume = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            fromSource.volume = 0f;
            toSource.volume = 1f;

            fromSource.Stop();
        }

    }
}