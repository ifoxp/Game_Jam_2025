using _Scripts.Controllers;
using UnityEngine;

namespace _Scripts.Building.WhenPlaced
{
    public class OnPlacedParticle : MonoBehaviour, IOnPlaced
    {
        [SerializeField] private ParticlesPlayer _particlesPlayer;
        
        public void OnPlaced()
        {
            _particlesPlayer?.Play();
        }
    }
}