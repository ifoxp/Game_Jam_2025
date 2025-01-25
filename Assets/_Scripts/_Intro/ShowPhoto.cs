using _Scripts.Audio;
using NaughtyAttributes;
using UnityEngine;
using PrimeTween;
using UnityEngine.Events;

namespace _Scripts._Intro
{
    public class ShowPhoto : MonoBehaviour
    {
        [SerializeField] private RectTransform _photo;
        
        [SerializeField] private Vector3 _shownPosition;
        [SerializeField] private Vector3 _hiddenPosition;

        [Space] [SerializeField] private AudioTrigger _onShow;

        [SerializeField] private UnityEvent _after;
        
        [Button]
        private void SetShownPosition()
        {
            _shownPosition = _photo.anchoredPosition;
        }

        [Button]
        private void SetHiddenPosition()
        {
            _hiddenPosition = _photo.anchoredPosition;
        }

        [Button]
        public void _Show()
        {
            Tween.UIAnchoredPosition(_photo, _shownPosition, 0.5f, Ease.OutQuad)
                .OnComplete(() => _after?.Invoke());
            _onShow?._Trigger();
        }

        [Button]
        public void _Hide()
        {
            Tween.UIAnchoredPosition(_photo, _hiddenPosition, 0.5f, Ease.InQuad);
        }
    }
}