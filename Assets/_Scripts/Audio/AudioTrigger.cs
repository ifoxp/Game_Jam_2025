using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Audio
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] private AudioPlayer _audioPlayer;
        public void _TriggerPlay()
        {
            _audioPlayer.PlayShot();
        }

        #if UNITY_EDITOR
        
        [SerializeField] private Button[] _buttonsToPlaySoundOnClick;
        private bool _initialized;
        
        [Button]
        private void InitializeButtons()
        {
            if (_initialized)
            {
                Debug.Log("<color=red>Delete listeners before initialize</color>");
                return;
            }
            _initialized = true;

            foreach (var button in _buttonsToPlaySoundOnClick)
            {
                button.onClick.AddListener(_TriggerPlay);
            }

            Debug.Log("<color=green>Initialized</color>");
        }

        [Button]
        private void DeleteListener()
        {
            if (!_initialized)
            {
                Debug.Log("<color=red>Initialize before delete listeners!</color>");
                return;
            }
            _initialized = false;

            foreach (var button in _buttonsToPlaySoundOnClick)
            {
                button.onClick.RemoveListener(_TriggerPlay);
            }
            
            Debug.Log("<color=green>Deleted listeners successfully!</color>");
        }
        #endif
    }
}
