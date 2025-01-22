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
            _input.OnInteract += CheckInteractComponentByPointer;
            
            _pointerCaster = new CastersAdditional();
        }

        private void OnDestroy()
        {
            if(_input != null) _input.OnInteract -= CheckInteractComponentByPointer;
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
            }
        }
    }
}
