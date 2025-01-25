using System.Collections;
using _Scripts.Audio;
using UnityEngine;

namespace _Scripts.Building.WhenPlaced
{
    public class OnPlacedDelayedSound : MonoBehaviour, IOnPlaced
    {
        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private float _delay;
        
        public void OnPlaced()
        {
            StartCoroutine(PlaySoundWithDelay());
        }

        private IEnumerator PlaySoundWithDelay()
        {
            yield return new WaitForSeconds(_delay);
            _audioPlayer.PlayShot();
        }
    }
}