using System;
using _Scripts.Inputs.Reader;
using _Scripts.Utilities;
using UnityEngine;
using Zenject;

namespace _Scripts.Interact
{
    public class InteractByPointer : MonoBehaviour
    {
        private CastersAdditional _pointerCaster;
        
        private IInput _input;
        private Camera _playerCamera;

        private IInteractableByPointer _currentInteractable;
     
        public event Action<IInteractableByPointer> OnInteractComponentChanged;
        
        private const int RAY_DISTANCE = 100;
        
        [Inject]
        private void Construct(IInput input, Camera playerCamera)
        {
            _input = input;
            _playerCamera = playerCamera;
        }

        private void Awake()
        {
            if (_input == null) throw new MissingComponentException("Input cannot be null");
            if (_playerCamera == null) 
                throw new MissingComponentException("Player camera cannot be null. Set it in the installer");
            
            _input.OnInteract += CheckInteractComponentByPointer;
            
            _pointerCaster = new CastersAdditional();
        }

        private void OnDestroy()
        {
            if(_input != null) _input.OnInteract -= CheckInteractComponentByPointer;
            OnInteractComponentChanged = null;
        }

        private void CheckInteractComponentByPointer()
        {
            var objectInRay = _pointerCaster.GetGameObjectByPointer(_input.GetPointerPosition(), _playerCamera, 
                RAY_DISTANCE);
           // print(objectInRay);
            var newInteractable = objectInRay?.GetComponent<IInteractableByPointer>();

            if (newInteractable == null)
            {
                _currentInteractable?.OnStopInteractByPointer();
            }
            else
            {
                if (_currentInteractable != newInteractable)
                {
                    _currentInteractable?.OnStopInteractByPointer();
                }
                
                newInteractable.OnInteractByPointer();
                _currentInteractable = newInteractable;
                
                OnInteractComponentChanged?.Invoke(_currentInteractable);
            }
        }
    }
}
