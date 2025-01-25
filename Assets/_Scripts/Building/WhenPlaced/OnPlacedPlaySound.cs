using _Scripts.Audio;
using UnityEngine;

namespace _Scripts.Building.WhenPlaced
{
    public class OnPlacedPlaySound : MonoBehaviour, IOnPlaced
    {
        [SerializeField] private AudioPlayer _audioPlayer;
        
        public void OnPlaced()
        {
            _audioPlayer?.PlayShot();
        }
    }
}