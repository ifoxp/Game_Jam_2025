using _Scripts._BuildingsEarn.Systems.Interfaces;
using _Scripts.Audio;
using UnityEngine;

namespace _Scripts._BuildingsEarn.Systems
{
    public class PlaySoundOnBuildingPlaced : MonoBehaviour, IOnBuildingPlaced
    {
        [SerializeField] private AudioPlayer _audioPlayer;
        
        public void OnBuildingPlaced()
        {
            _audioPlayer?.PlayShot();
        }
    }
}