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
        [SerializeField] private float _rotationSensitivity = 0.5f;

        private bool _canRotate;
        
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
            _input.OnMouseMove += RotateCameraByChangingValues;
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
        
        private void RotateCameraByChangingValues(Vector2 inputValue)
        {
            if (!_canRotate) return;

            var sensitivity = PlayerPrefs.GetFloat(
                Utilities.SettingsKeys.ROTATE_CAMERA_SENSITIVITY, _rotationSensitivity);

            var newHorizontalValue = CalculateWrappedAxisValue(inputValue.x, 
                _orbitalFollowCamera.HorizontalAxis, sensitivity);
            
            var newVerticalValue = CalculateClampedAxisValue(inputValue.y, 
                _orbitalFollowCamera.VerticalAxis, sensitivity);

            _orbitalFollowCamera.HorizontalAxis.Value = newHorizontalValue;
            _orbitalFollowCamera.VerticalAxis.Value = newVerticalValue;
        }

        private float CalculateWrappedAxisValue(float inputValue, InputAxis axis, float sensitivity)
        {
            var axisRangeWidth = axis.Range.y - axis.Range.x;
            var newAxisValue = axis.Value + inputValue * sensitivity * Time.deltaTime;
            
            return Mathf.Repeat(newAxisValue - axis.Range.x, axisRangeWidth) + axis.Range.x;
        }

        private float CalculateClampedAxisValue(float inputValue, InputAxis axis, float sensitivity)
        {
            var newAxisValue = axis.Value - inputValue * sensitivity * Time.deltaTime;
            return Mathf.Clamp(newAxisValue, axis.Range.x, axis.Range.y);
        }
        
        private void OnRightsToRotateCameraChanged(bool allowRotate)
        {
            _canRotate = allowRotate;
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
            RestoreCameraRanges();
            LoadValuesIntoCamera();
        }

        private void DisableCameraRotation()
        {
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
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            PlayerPrefs.SetFloat(Utilities.SettingsKeys.ROTATE_CAMERA_SENSITIVITY, _rotationSensitivity);
            PlayerPrefs.Save();
        }
#endif
    }
}