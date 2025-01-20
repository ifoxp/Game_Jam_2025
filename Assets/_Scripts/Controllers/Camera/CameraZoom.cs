using Unity.Cinemachine;
using UnityEngine;

using _Scripts.Inputs.Reader;

using Zenject;

using System;

namespace _Scripts.Controllers.Camera
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
        [SerializeField] private Transform _cameraCenterZoomTarget;
        
        [Space]
        [SerializeField] private float _moveSensitivity;
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
            if(_input == null) throw new NullReferenceException("Input cannot be null!");
            if(_orbitalFollow == null) throw new NullReferenceException("Cinemachine orbital camera is null");
            if(_cameraCenterZoomTarget == null) throw new NullReferenceException("Camera center target cannot be null!");

            _input.OnMouseWheelScroll += UpdateCameraPosition;
            
            _orbitalFollow.GetComponent<CinemachineCamera>().Follow = _cameraCenterZoomTarget;
        }

        private void OnDestroy()
        {
            if(_input != null) _input.OnMouseWheelScroll -= UpdateCameraPosition;
        }

        private void UpdateCameraPosition(Vector2 direction)
        {
            if (_reversedZoom) direction *= -1;
            
            var newZoom = Mathf.Clamp(_orbitalFollow.Radius + (direction.y * _moveSensitivity),
                _maxZoomInDistance, _maxZoomOutDistance);
            _orbitalFollow.Radius = newZoom;
        }
    }
}