using _ScriptableAssets.AnimatedUI;
using UnityEngine;

namespace _Scripts.UI
{
    public class AnimateRectObject : MonoBehaviour
    {
        [SerializeField] private AnimateRectStrategy _animateStrategy;
        
        [Header("Set start menu section here")]
        [SerializeField] private RectTransform _currentMenuSection;

        public void _Animate(RectTransform newRectToShow)
        {
            _animateStrategy.AnimateMenuCategory(_currentMenuSection, newRectToShow, () => 
                Debug.Log("Completed!"));
            
            _currentMenuSection = newRectToShow;
        }
    }
}
