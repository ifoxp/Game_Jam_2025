using _Scripts.Inputs.Reader;
using _Scripts.Interact;
using UnityEngine;
using Zenject;

namespace _Scripts.Controllers
{
    public class InteractByPointer : MonoBehaviour
    {
        private IInput _input;
        private UnityEngine.Camera _playerCamera;

        private IInteractableByPointer _currentInteractable;
        
        private const int RAY_DISTANCE = 100;
        
        [Inject]
        private void Construct(IInput input, UnityEngine.Camera playerCamera)
        {
            _input = input;
            _playerCamera = playerCamera;
        }

        private void Awake()
        {
            if (_input == null) throw new MissingComponentException("Input cannot be null");
            _input.OnInteract += CheckInteractComponentByPointer;
        }

        private void OnDestroy()
        {
            if(_input != null) _input.OnInteract -= CheckInteractComponentByPointer;
        }

        private void CheckInteractComponentByPointer()
        {
            var objectInRay = GetGameObjectByPointer();
           // print(objectInRay);
            var newInteractable = objectInRay.GetComponent<IInteractableByPointer>();

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

        private GameObject GetGameObjectByPointer()
        {
            var pointerPosition = _input.GetPointerPosition();
            var pointerWorldPosition = 
                _playerCamera.ScreenToWorldPoint(new Vector3(
                    pointerPosition.x, pointerPosition.y, _playerCamera.nearClipPlane));
            
            var cameraPosition = _playerCamera.transform.position;
            var direction = (pointerWorldPosition - cameraPosition).normalized;

            return Physics.Raycast(cameraPosition, direction, out var hit, RAY_DISTANCE)
                ? hit.collider.gameObject : null;
        }
    }
}
