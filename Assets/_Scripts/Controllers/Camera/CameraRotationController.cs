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

        private Vector2 _defaultHorizontalRange;
        private Vector2 _defaultVerticalRange;
        
        private IInput _input;

        [Inject]
        private void Construct(IInput input)
        {
            _input = input;
        }
        
        private void Awake()
        {
            InitializeCameraSettings();
            SubscribeToInputEvents();
            
            // Lock rotation when initialized
            OnRightsToRotateCameraChanged(false);
        }

        private void InitializeCameraSettings()
        {
            EnsureComponentsNotNull();
            RecordLastValues();
            CacheDefaultCameraRanges();
        }

        private void CacheDefaultCameraRanges()
        {
            _defaultHorizontalRange = _orbitalFollowCamera.HorizontalAxis.Range;
            _defaultVerticalRange = _orbitalFollowCamera.VerticalAxis.Range;
        }

        private void SubscribeToInputEvents()
        {
            _input.OnLeftMousePressed += OnRightsToRotateCameraChanged;
        }

        private void EnsureComponentsNotNull()
        {
            if (_input == null) throw new MissingComponentException("Input cannot be null!");
            if (_orbitalFollowCamera == null) throw new MissingComponentException("Cinemachine orbital camera is null");
        }

        private void OnDestroy()
        {
            UnsubscribeFromInputEvents();
        }

        private void UnsubscribeFromInputEvents()
        {
            _input.OnLeftMousePressed -= OnRightsToRotateCameraChanged;
        }

        private void OnRightsToRotateCameraChanged(bool allowRotate)
        {
            if (allowRotate)
            {
                EnableCameraRotation();
            }
            else
            {
                DisableCameraRotation();
            }
        }

        private void EnableCameraRotation()
        {
            Utilities.CursorStateChanger.SetCursorState(CursorLockMode.None, true);
            
            RestoreCameraRanges();
            LoadValuesIntoCamera();
        }

        private void DisableCameraRotation()
        {
            Utilities.CursorStateChanger.SetCursorState(CursorLockMode.None, true);
            
            RecordLastValues();
            LockCameraToCurrentView();
        }

        private void RestoreCameraRanges()
        {
            _orbitalFollowCamera.HorizontalAxis.Range = _defaultHorizontalRange;
            _orbitalFollowCamera.VerticalAxis.Range = _defaultVerticalRange;
        }

        private void LockCameraToCurrentView()
        {
            _orbitalFollowCamera.HorizontalAxis.Range = new Vector2(_orbitalFollowCamera.HorizontalAxis.Value,
                _orbitalFollowCamera.HorizontalAxis.Value);

            _orbitalFollowCamera.VerticalAxis.Range = new Vector2(_orbitalFollowCamera.VerticalAxis.Value,
                _orbitalFollowCamera.VerticalAxis.Value);
        }

        private void RecordLastValues()
        {
            _lastValues.x = _orbitalFollowCamera.HorizontalAxis.Value;
            _lastValues.y = _orbitalFollowCamera.VerticalAxis.Value;
        }

        private void LoadValuesIntoCamera()
        {
            _orbitalFollowCamera.HorizontalAxis.Value = _lastValues.x;
            _orbitalFollowCamera.VerticalAxis.Value = _lastValues.y;
        }
    }
}