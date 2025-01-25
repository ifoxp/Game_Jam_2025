using UnityEngine;

namespace _Scripts.Audio
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] private AudioPlayer _audioPlayer;

        public void _Trigger()
        {
            _audioPlayer?.PlayShot();
        }
    }
}
