using UnityEngine;
using Unity.Cinemachine;

using _Scripts.Inputs.Reader;

using NaughtyAttributes;
using Zenject;

namespace _Scripts.Controllers.Camera
{
    public class CameraMoveController : MonoBehaviour
    {
        [Required] [SerializeField] private Transform _cameraTarget;
        [SerializeField] private CinemachineRotationComposer _rotationComposerCamera;

        [SerializeField] private float _maxRadius = 30f;
        [SerializeField] private float _moveTargetSensitivity = 0.5f;
        
        private UnityEngine.Camera _playerCamera;
        private Vector3 _cameraStartPosition;
        
        private bool _canMove;
        
        private IInput _input;
        
        [Inject]
        private void Construct(IInput input, UnityEngine.Camera playerCamera)
        {
            _input = input;
            _playerCamera = playerCamera;
        }

        private void Awake()
        {
            _cameraStartPosition = _cameraTarget.position;
            ValidateComponents();
            
            _input.OnToggleMoveCamera += SetCanMoveCamera;
            _input.OnMouseMove += OnTargetMove;
            
            _input.OnReturnToMapCenter += ReturnToCenter;
        }

        private void OnDestroy()
        {
            _input.OnToggleMoveCamera -= SetCanMoveCamera;
            _input.OnMouseMove -= OnTargetMove;

            _input.OnReturnToMapCenter -= ReturnToCenter;
        }

        private void ValidateComponents()
        {
            if(_input == null) throw new MissingComponentException("Input cannot be null");
            if(_cameraTarget == null) throw new MissingComponentException("CameraTarget cannot be null");
            if(_rotationComposerCamera == null) throw new MissingComponentException("Rotation composer camera cannot be null");
            if(_playerCamera == null) throw new MissingComponentException("Player Camera cannot be null");
        }

        private void ReturnToCenter()
        {
            _cameraTarget.position = _cameraStartPosition;
        }

        private void SetCanMoveCamera(bool canMove)
        {
            _canMove = canMove;

            _rotationComposerCamera.enabled = !canMove;
        }

        private void OnTargetMove(Vector2 direction)
        {
            if (!_canMove) return;

            var moveDirection = CalculateNextCameraPosition(direction);
            var clampedMove = ClampDirectionUsingRadius(moveDirection);
            _cameraTarget.position = clampedMove;
        }

        private Vector3 ClampDirectionUsingRadius(Vector3 moveDirection)
        {
            var potentialPosition = _cameraTarget.position + moveDirection * Time.deltaTime * _moveTargetSensitivity;

            var offset = potentialPosition - _cameraStartPosition;
            if (offset.magnitude > _maxRadius)
            {
                offset = offset.normalized * _maxRadius;
                potentialPosition = _cameraStartPosition + offset;
            }

            return potentialPosition;
        }

        private Vector3 CalculateNextCameraPosition(Vector2 direction)
        {
            var cameraForward = _playerCamera.transform.forward;
            var cameraRight = _playerCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            return (cameraForward * direction.y + cameraRight * direction.x);
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_cameraTarget == null) return;

            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawWireSphere(_cameraStartPosition, _maxRadius);
        }
        #endif
    }
}
