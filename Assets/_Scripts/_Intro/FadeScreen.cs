using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts._Intro
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private Image _blackScreen;
        [SerializeField] private UnityEvent _onFadeComplete;

        private const float FADE_DURATION = 3f;

        public void _StartFade()
        {
            Tween.Alpha(_blackScreen, 1, FADE_DURATION)
                .OnComplete(() => _onFadeComplete?.Invoke());
        }
    }
}
