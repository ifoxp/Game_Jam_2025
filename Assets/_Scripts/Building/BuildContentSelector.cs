using _Scripts.Inputs.Reader;
using PrimeTween;
using UnityEngine;
using Zenject;

namespace _Scripts.Building
{
    public class BuildContentSelector : MonoBehaviour
    {
        [SerializeField] private GameObject _contentSelector;
        [SerializeField] private float _selectorAnimationSegmentTime = 0.3f;
        private IInput _input;

        private const float FULL_SCALED_CONTENT_Y = 1;

        [Inject]
        private void Construct(IInput input)
        {
            _input = input;
        }

        private void Awake()
        {
            if(_input == null) throw new MissingComponentException("Input cannot be null");
            if(_contentSelector == null) throw new MissingComponentException("Content selector cannot be null");

            _input.OnSelectContentPressed += OnSelectorButtonChanged;
        }

        private void OnSelectorButtonChanged(bool isPressed)
        {
            if (isPressed)
            {
                PlaceSelectorToPointer();
                ShowAnimateSelector();
            }
            else
            {
                HideAnimateSelector();
            }
        }

        private void PlaceSelectorToPointer()
        {
            var pointerPosition = _input.GetPointerPosition();
            _contentSelector.transform.position = pointerPosition;
        }
        
        private void ShowAnimateSelector()
        {
            if (_contentSelector.transform.localScale.y >= FULL_SCALED_CONTENT_Y) return;
            Tween.ScaleY(_contentSelector.transform, FULL_SCALED_CONTENT_Y, _selectorAnimationSegmentTime);
        }

        private void HideAnimateSelector()
        {
            if(_contentSelector.transform.localScale.y <= 0) return;
            Tween.ScaleY(_contentSelector.transform, 0,
                _selectorAnimationSegmentTime);
        }
    }
}