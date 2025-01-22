using System;
using PrimeTween;
using UnityEngine;

namespace _ScriptableAssets.AnimatedUI
{
    [CreateAssetMenu(fileName = "New Animated Rect by Position", menuName = "GJ/UI/Animated Rect by Position")]
    public class AnimatedRectByPosition : AnimateRectStrategy
    {
        [SerializeField] private float _animationFragmentDuration;

        [SerializeField] private Vector3 _hiddenPosition = new(-1920, 0, 0);
        [SerializeField] private Vector3 _shownPosition = new(0, 0, 0);
        
        [SerializeField] private Ease _easeParentIn = Ease.Linear;

        public override void AnimateMenuCategory(RectTransform lastMenuParent, RectTransform newMenuParent, 
            Action onCompleteAction)
        {
            newMenuParent.anchoredPosition = new Vector3(_hiddenPosition.x, newMenuParent.anchoredPosition.y, 0);

            Sequence.Create()
                .Chain(Tween.UIAnchoredPosition(lastMenuParent, _hiddenPosition, _animationFragmentDuration))
                .Chain(Tween.UIAnchoredPosition(newMenuParent, _shownPosition, _animationFragmentDuration, _easeParentIn))
                .ChainCallback(onCompleteAction);
        }
    }
}