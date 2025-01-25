using NaughtyAttributes;
using PrimeTween;
using UnityEngine;

namespace _Scripts.Building.WhenPlaced
{
    public class OnPlacedScaleAnimation : MonoBehaviour, IOnPlaced
    {
        
        [Tooltip("Set transform where model placed, it should be child")] [Required] [SerializeField]
        private Transform _modelTransform;
        
        [SerializeField] private float _timeToScale = 0.4f;
        [SerializeField] private Ease _ease = Ease.OutBounce;
        
        [SerializeField] private Vector3 _startScale = Vector3.zero;
        private Vector3 _finalScale;
        
        public void OnPlaced()
        {
            _finalScale = _modelTransform.localScale;

            _modelTransform.localScale = _startScale;
            Tween.Scale(_modelTransform, _finalScale, _timeToScale, _ease);
        }
    }
}