using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Scripts.Inputs.Reader
{
    public class InputReader : IInitializable, IDisposable, IInput
    {
        public event Action<Vector2> OnMouseWheelScroll;
        public event Action<Vector2> OnMouseMove;
        public event Action<bool> OnLeftMousePressed;
        public event Action OnInteract;
        public event Action<bool> OnToggleMoveCamera;

        private PlayerInputMap _inputMap;
        
        public void Initialize()
        {
            _inputMap = new PlayerInputMap();
            
            SubscribeInputs();
            _inputMap.Player.Enable();
        }

        private void SubscribeInputs()
        {
            _inputMap.Player.ZoomLook.performed += OnNotifyScrollLook;
            _inputMap.Player.ZoomLook.canceled += OnNotifyScrollLook;

            _inputMap.Player.PositionLook.performed += OnNotifyMouseMove;
            _inputMap.Player.PositionLook.canceled += OnNotifyMouseMove;

            _inputMap.Player.LeftMouseButton.performed += OnLeftMouseButtonPressed;
            _inputMap.Player.LeftMouseButton.canceled += OnLeftMouseButtonPressed;
            
            _inputMap.Player.Interact.performed += OnInteractNotify;

            _inputMap.Player.ToggleMoveCamera.performed += OnToggleMoveCameraNotify;
            _inputMap.Player.ToggleMoveCamera.canceled += OnToggleMoveCameraNotify;
        }

        public void Dispose()
        {
            _inputMap.Disable();
            
            UnsubscribeInputs();
            ClearActions();
            
            _inputMap.Dispose();
        }

        private void UnsubscribeInputs()
        {
            _inputMap.Player.ZoomLook.performed -= OnNotifyScrollLook;
            _inputMap.Player.ZoomLook.canceled -= OnNotifyScrollLook;
            
            _inputMap.Player.PositionLook.performed -= OnNotifyMouseMove;
            _inputMap.Player.PositionLook.canceled -= OnNotifyMouseMove;
            
            _inputMap.Player.LeftMouseButton.performed -= OnLeftMouseButtonPressed;
            _inputMap.Player.LeftMouseButton.canceled -= OnLeftMouseButtonPressed;
            
            _inputMap.Player.Interact.performed -= OnInteractNotify;
            
            _inputMap.Player.ToggleMoveCamera.performed -= OnToggleMoveCameraNotify;
            _inputMap.Player.ToggleMoveCamera.canceled -= OnToggleMoveCameraNotify;
        }

        private void ClearActions()
        {
            OnMouseWheelScroll = null;
            OnMouseMove = null;
            OnLeftMousePressed = null;
            OnInteract = null;
            OnToggleMoveCamera = null;
        }

        private void OnNotifyScrollLook(InputAction.CallbackContext context) => 
            NotifyVector2Action(context, OnMouseWheelScroll);

        private void OnNotifyMouseMove(InputAction.CallbackContext context) => 
            NotifyVector2Action(context, OnMouseMove);
        
        private void OnLeftMouseButtonPressed(InputAction.CallbackContext context) =>
            OnLeftMousePressed?.Invoke(context.performed);
        
        private void OnInteractNotify(InputAction.CallbackContext context) =>
            OnInteract?.Invoke();

        private void OnToggleMoveCameraNotify(InputAction.CallbackContext context) =>
            OnToggleMoveCamera?.Invoke(context.performed);
            
        private void NotifyVector2Action(InputAction.CallbackContext context, Action<Vector2> action)
        {
            var vector2 = context.ReadValue<Vector2>();
            action?.Invoke(vector2);
        }

        public Vector2 GetPointerPosition()
        {
            return Mouse.current.position.ReadValue();
        }
    }
}