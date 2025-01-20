using UnityEngine;
using Unity.Cinemachine;

using _Scripts.Inputs.Reader;

using NaughtyAttributes;
using Zenject;

namespace _Scripts.Controllers.Camera
{
    public class CameraRotationController : MonoBehaviour
    {
        [Required]
        [SerializeField] private CinemachineOrbitalFollow _orbitalFollowCamera;
        private Vector2 _lastValues;
        
        private IInput _input;

        [Inject]
        private void Construct(IInput input)
        {
            _input = input;
        }
        
        private void Awake()
        {
            EnsureComponentsNotNull();
            
            RecordLastValues();
            _input.OnLeftMousePressed += OnRightsToRotateCameraChanged;
        }
        
        private void EnsureComponentsNotNull()
        {
            if(_input == null) throw new MissingComponentException("Input cannot be null!");
            if(_orbitalFollowCamera == null) throw new MissingComponentException("Cinemachine orbital camera is null");
        }

        private void OnDestroy()
        {
            _input.OnLeftMousePressed -= OnRightsToRotateCameraChanged;
        }

        private void OnRightsToRotateCameraChanged(bool allowRotate)
        {
            _orbitalFollowCamera.enabled = allowRotate;

            // cursor states better to put it in some kind of 'event bus' or something
            if(!allowRotate)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                RecordLastValues();
            }
            else
            { 
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                LoadValuesIntoCameraValues();
            }
        }

        private void RecordLastValues()
        {
            _lastValues.x = _orbitalFollowCamera.HorizontalAxis.Value;
            _lastValues.y = _orbitalFollowCamera.VerticalAxis.Value;
        }

        private void LoadValuesIntoCameraValues()
        {
            _orbitalFollowCamera.HorizontalAxis.Value = _lastValues.x; 
            _orbitalFollowCamera.VerticalAxis.Value = _lastValues.y;
        }
    }
}