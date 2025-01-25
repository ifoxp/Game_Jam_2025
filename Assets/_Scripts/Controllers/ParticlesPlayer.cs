using System;
using UnityEngine;

namespace _Scripts.Controllers
{
    [Serializable]
    public class ParticlesPlayer
    {
        [SerializeField] private ParticleSystem _particleToPlay;

        public void Construct(ParticleSystem particleToPlay)
        {
            _particleToPlay = particleToPlay;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Play()
        {
            if (!_particleToPlay)
            {
                Debug.LogWarning("Particles are null");
                return;
            }

            _particleToPlay.gameObject.SetActive(true);
            _particleToPlay.Play();
        }

        public void Stop()
        {
            if (!_particleToPlay)
            {
                Debug.LogWarning("Particles are null");
                return;
            }
            
            _particleToPlay.Stop();
            _particleToPlay.gameObject.SetActive(false);
        }
    }
}