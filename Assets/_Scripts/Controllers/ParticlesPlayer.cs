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

        public void Play()
        {
            _particleToPlay.Play();
        }

        public void Stop()
        {
            _particleToPlay.Stop();
        }
    }
}