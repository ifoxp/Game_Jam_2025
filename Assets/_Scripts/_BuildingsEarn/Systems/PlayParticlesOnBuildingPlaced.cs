using _Scripts._BuildingsEarn.Systems.Interfaces;
using _Scripts.Controllers;
using UnityEngine;

namespace _Scripts._BuildingsEarn.Systems
{
    public class PlayParticlesOnBuildingPlaced : MonoBehaviour, IOnBuildingPlaced
    {
        [SerializeField] private ParticlesPlayer _particlesPlayer; 
            
        public void OnBuildingPlaced()
        {
            _particlesPlayer?.Play();
        }
    }
}