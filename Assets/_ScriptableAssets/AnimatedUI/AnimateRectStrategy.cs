using System;
using UnityEngine;

namespace _ScriptableAssets.AnimatedUI
{
    public abstract class AnimateRectStrategy : ScriptableObject
    {
        public abstract void AnimateMenuCategory(RectTransform lastMenuParent, RectTransform newMenuParent, 
            Action onCompleteAction);
    }
}