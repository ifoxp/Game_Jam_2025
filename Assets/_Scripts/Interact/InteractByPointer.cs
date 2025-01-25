using _Scripts.Inputs.Reader;
using _Scripts.Utilities;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace _Scripts.Interact
{
    public class InteractByPointer : MonoBehaviour
    {
        [SerializeField] private LayerMask _ignoreLayer;
        private CastersAdditional _pointerCaster;
        
        private IInput _input;
        private Camera _playerCamera;

        private HashSet<IInteractableByPointer> _currentInteractable;

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
            _currentInteractable = new HashSet<IInteractableByPointer>();
        }

        private void OnDestroy()
        {
            if (_input != null) _input.OnInteract -= CheckInteractComponentByPointer;
        }

        private void CheckInteractComponentByPointer()
        {
            var objectInRay = _pointerCaster.GetGameObjectByPointer(_input.GetPointerPosition(), _playerCamera, 
                RAY_DISTANCE, _ignoreLayer);

            var newInteractable = objectInRay?.GetComponents<IInteractableByPointer>();

            if (newInteractable == null || newInteractable.Length == 0)
            {
                if (_currentInteractable.Count > 0)
                {
                    foreach (var interactable in _currentInteractable)
                    {
                        interactable.OnStopInteractByPointer();
                    }
                    _currentInteractable.Clear();
                }
            }
            else
            {
                var newInteractableSet = new HashSet<IInteractableByPointer>(newInteractable);

                if (!newInteractableSet.SetEquals(_currentInteractable))
                {
                    foreach (var interactable in _currentInteractable)
                    {
                        if (!newInteractableSet.Contains(interactable))
                        {
                            interactable.OnStopInteractByPointer();
                        }
                    }

                    foreach (var interactable in newInteractableSet)
                    {
                        if (!_currentInteractable.Contains(interactable))
                        {
                            interactable.OnInteractByPointer();
                        }
                    }

                    _currentInteractable = newInteractableSet;
                }
            }
        }
    }
}
