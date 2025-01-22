using _Scripts.Entities.Interfaces;
using PrimeTween;
using UnityEngine;

namespace _Scripts.Entities
{
    [RequireComponent(typeof(BoxCollider))]
    public class FlyingMeteorite : MonoBehaviour, IEntity
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _maxErrorAngle = 20f;
        [SerializeField] private Vector3 _calculatedFlyDirection;

        [Tooltip("Object with layer which meteor forbidden to fly in")]
        [SerializeField] private LayerMask _forbiddenLayer;
        
        [SerializeField] private BoxCollider _meteoriteCollider;
        
        private Tween _currentTween;
        
        private const int MIN_SPEED = 3;
        private const int MAX_SPEED = 15;
        
        private const float MIN_SCALE = 0.5f;
        private const float MAX_SCALE = 2.5f;
        [SerializeField]
        private float SCALE_ANIMATION_TIME_BLIAT = 4f;
        
        private const float MAX_CHECK_DISTANCE =400f;

        private const string DESTROY_COLLIDER_TAG = "EntityDestroy";

        public void Initialize()
        {
            _speed = Random.Range(MIN_SPEED, MAX_SPEED);
            //transform.localScale = Vector3.zero;

            var randomScale = Random.Range(MIN_SCALE, MAX_SCALE);
            //Tween.Scale(transform, randomScale, SCALE_ANIMATION_TIME_BLIAT,Ease.Linear);

            _calculatedFlyDirection = CalculateDirection();
            //Debug.Log(_calculatedFlyDirection);
            
            var randomRotation = Random.rotation.eulerAngles;
            _currentTween = Tween.Rotation(transform, randomRotation, 3, Ease.Linear, -1, CycleMode.Incremental);
        }

        private Vector3 CalculateDirection()
        {
            var baseDirection = Vector3.back;

            var maxErrorAngleRadians = _maxErrorAngle * Mathf.Deg2Rad;
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

            var colliderSize = _meteoriteCollider.size;
            foreach (var direction in possibleDirections)
            {
                var startPoint = transform.position + _meteoriteCollider.center;

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

            Debug.Log("<color=red>Destroying earlier, because can't set direction</color>");
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
               // _currentTween.Complete();
                Tween.CompleteAll();

                /*Tween.Scale(transform, 0, SCALE_ANIMATION_TIME_BLIAT, Ease.Linear).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    Invoke("DestroyObjectForTime",1);
                });*/

                Destroy(gameObject);
            }
        }
        /*private void DestroyObjectForTime()
        {
            Destroy(gameObject);
        }*/
    }
}