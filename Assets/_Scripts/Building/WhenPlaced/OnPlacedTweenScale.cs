using PrimeTween;
using UnityEngine;

namespace _Scripts.Building.WhenPlaced
{
    public class OnPlacedTweenScale : MonoBehaviour, IOnPlaced
    {
        [SerializeField] private Transform _modelToScale;
        [SerializeField] private Vector3 _startScale = new Vector3(0.2f, 0.2f, 0.2f);
        
        [SerializeField] private float _duration = 0.4f;
        [SerializeField] private Ease _easeType = Ease.OutBounce;
        
        private Vector3 _finalScale;
        
        public void OnPlaced()
        {
            _finalScale = _modelToScale.localScale;
            _modelToScale.localScale = _startScale;
            
            Tween.Scale(_modelToScale, _finalScale, _duration, _easeType);
        }
    }
}
