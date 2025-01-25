using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private Image _imageToFade;
        [SerializeField] private bool _fadeOutOnStart = true;

        private const float FADE_DURATION = 2.5f;

        private const float FADED = 1;
        private const float UN_FADED = 0;

        private void Start()
        {
            if (!_fadeOutOnStart) return;

            FadeOut();
        }

        public void FadeIn()
        {
            _imageToFade.gameObject.SetActive(true);
            Tween.Alpha(_imageToFade, FADED, FADE_DURATION);
        }

        public void FadeOut()
        {
            Tween.Alpha(_imageToFade, UN_FADED, FADE_DURATION)
                .OnComplete(() => _imageToFade.gameObject.SetActive(false));
        }
    }
}