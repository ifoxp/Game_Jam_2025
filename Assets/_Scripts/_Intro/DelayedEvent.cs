using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts._Intro
{
    public class DelayedEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _event;
        
        public void _StartDelayedEvent(float delay)
        {
            StartCoroutine(_DelayedEvent(delay));
        }

        private IEnumerator _DelayedEvent(float delay)
        {
            yield return new WaitForSeconds(delay);
            _event?.Invoke();
        }
    }
}
