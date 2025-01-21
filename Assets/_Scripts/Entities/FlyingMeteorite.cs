using _Scripts.Entities.Interfaces;
using PrimeTween;
using UnityEngine;

namespace _Scripts.Entities
{
    [RequireComponent(typeof(BoxCollider))]
    public class FlyingMeteorite : MonoBehaviour, IEntity
    {
        [SerializeField] private float _speed;
        [SerializeField] private Vector3 _calculatedFlyDirection;

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

            _calculatedFlyDirection = CalculateDirection();
            Debug.Log(_calculatedFlyDirection);
            
            var randomRotation = Random.rotation.eulerAngles;
            _currentTween = Tween.Rotation(transform, randomRotation, 3, Ease.Default, -1, CycleMode.Incremental);
        }

        private Vector3 CalculateDirection()
        {
            var baseDirection = Vector3.back;

            var colliderSize = _meteoriteCollider.size;

            var maxErrorAngle = 25f;
            var maxErrorAngleRadians = maxErrorAngle * Mathf.Deg2Rad;

            Vector3[] possibleDirections =
            {
                baseDirection,
                baseDirection + new Vector3(Mathf.Sin(maxErrorAngleRadians), 0, 0),
                baseDirection + new Vector3(-Mathf.Sin(maxErrorAngleRadians), 0, 0),
                baseDirection + new Vector3(0, Mathf.Sin(maxErrorAngleRadians), 0),
                baseDirection + new Vector3(0, -Mathf.Sin(maxErrorAngleRadians), 0),
                baseDirection + new Vector3(0, 0, Mathf.Sin(maxErrorAngleRadians)),
                baseDirection + new Vector3(0, 0, -Mathf.Sin(maxErrorAngleRadians))
            };

            foreach (var direction in possibleDirections)
            {
                var startPoint = transform.position ;

                if (!Physics.BoxCast(
                        startPoint,
                        colliderSize / 2,
                        direction.normalized,
                        out _,
                        Quaternion.identity,
                        MAX_CHECK_DISTANCE,
                        _forbiddenLayer
                    ))
                {
                    return direction;
                }
            }

            Debug.Log("<color=red>touching</color>");
            Destroy(gameObject);
            
            return Vector3.zero;
        }

        private void FixedUpdate()
        {
            transform.position += _calculatedFlyDirection * (_speed * Time.fixedDeltaTime);
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