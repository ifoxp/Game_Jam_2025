using Unity.Cinemachine;
using UnityEngine;

using _Scripts.Inputs.Reader;

using NaughtyAttributes;
using Zenject;

namespace _Scripts.Controllers.Camera
{
    public class CameraZoom : MonoBehaviour
    {
        [Required]
        [SerializeField] private CinemachineOrbitalFollow _orbitalFollowCamera;
        
        [Required]
        [SerializeField] private Transform _cameraCenterZoomTarget;
        
        [Space]
        [SerializeField] private float _zoomSensitivity;
        [SerializeField] private bool _reversedZoom = true;
        
        [Space, Header("Zoom Settings")]
        [SerializeField] private float _maxZoomOutDistance = 10f;
        [SerializeField] private float _maxZoomInDistance = 2f;

        private IInput _input;
        
        [Inject]
        private void Construct(IInput input)
        {
            _input = input;
        }

        private void Awake()
        {
            EnsureComponentsNotNull();
            
            _orbitalFollowCamera.GetComponent<CinemachineCamera>().Follow = _cameraCenterZoomTarget;
            SubscribeButtons();
        }

        private void EnsureComponentsNotNull()
        {
            if(_input == null) throw new MissingComponentException("Input cannot be null!");
            if(_orbitalFollowCamera == null) throw new MissingComponentException("Cinemachine orbital camera is null");
            if(_cameraCenterZoomTarget == null) throw new MissingComponentException("Camera center target cannot be null!");
        }

        private void SubscribeButtons()
        {
            _input.OnMouseWheelScroll += Zoom;
        }

        private void OnDestroy()
        {
            UnsubscribeButtons();
        }

        private void UnsubscribeButtons()
        {
            if (_input == null) return;
            
            _input.OnMouseWheelScroll -= Zoom;
        }

        private void Zoom(Vector2 direction)
        {
            if (_reversedZoom) direction *= -1;
            
            var newZoom = Mathf.Clamp(_orbitalFollowCamera.Radius + (direction.y * _zoomSensitivity),
                _maxZoomInDistance, _maxZoomOutDistance);
            _orbitalFollowCamera.Radius = newZoom;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            PlayerPrefs.SetFloat(Utilities.Keys.ZOOM_CAMERA_SENSITIVITY, _zoomSensitivity);
            PlayerPrefs.Save();
        }
#endif
    }
}