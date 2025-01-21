using _Scripts.Entities.Interfaces;
using PrimeTween;
using UnityEngine;

namespace _Scripts.Entities
{
    [RequireComponent(typeof(BoxCollider))]
    public class Meteorite : MonoBehaviour, IEntity
    {
        [SerializeField] private float _speed;
        [SerializeField] private Vector3 _flyDirection;

        [Tooltip("Object with layer which meteor forbidden to fly in")]
        [SerializeField] private LayerMask _forbiddenLayer;
        
        [SerializeField] private BoxCollider _meteoriteCollider;
        
        private Tween _currentTween;
        
        private const int MIN_SPEED = 3;
        private const int MAX_SPEED = 15;
        
        private const float MIN_SCALE = 0.5f;
        private const float MAX_SCALE = 2.5f;
        
        private const float MAX_CHECK_DISTANCE = 125f;

        private const string DESTROY_COLLIDER_TAG = "EntityDestroy";

        public void Initialize()
        {
            _speed = Random.Range(MIN_SPEED, MAX_SPEED);
            
            var randomScale = Random.Range(MIN_SCALE, MAX_SCALE);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            _flyDirection = CalculateDirection();
            
            var randomRotation = Random.rotation.eulerAngles;
            _currentTween = Tween.Rotation(transform, randomRotation, 3, Ease.Default, -1, CycleMode.Incremental);
        }

        private Vector3 CalculateDirection()
        {
            var baseDirection = Vector3.down;

            var colliderSize = _meteoriteCollider.size;
            var colliderCenter = _meteoriteCollider.center;
            
            Vector3[] possibleDirections =
            {
                baseDirection,
                baseDirection + Vector3.left * 0.5f,
                baseDirection + Vector3.right * 0.5f,
                baseDirection + Vector3.forward * 0.5f,
                baseDirection + Vector3.back * 0.5f
            };

            foreach (var direction in possibleDirections)
            {
                var startPoint = transform.position + colliderCenter;

                if (!Physics.BoxCast(
                        startPoint,
                        colliderSize / 2,
                        direction.normalized,
                        out var hit,
                        Quaternion.identity,
                        MAX_CHECK_DISTANCE,
                        _forbiddenLayer
                    ))
                {
                    return direction.normalized;
                }
            }

            return baseDirection;
        }

        private void FixedUpdate()
        {
            transform.position += _flyDirection * (_speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(DESTROY_COLLIDER_TAG))
            {
                _currentTween.Complete();
                
                Destroy(gameObject);
            }
        }
    }
}
