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
        }
        
        public void Dispose()
        {
            _inputMap.Disable();
            
            UnsubscribeInputs();
            _inputMap.Dispose();
        }

        private void UnsubscribeInputs()
        {
            _inputMap.Player.ZoomLook.performed -= OnNotifyScrollLook;
            _inputMap.Player.ZoomLook.canceled -= OnNotifyScrollLook;
            
            _inputMap.Player.PositionLook.performed -= OnNotifyMouseMove;
            _inputMap.Player.PositionLook.canceled -= OnNotifyMouseMove;
        }

        private void OnNotifyScrollLook(InputAction.CallbackContext context) => 
            NotifyVector2Action(context, OnMouseWheelScroll);

        private void OnNotifyMouseMove(InputAction.CallbackContext context) => 
            NotifyVector2Action(context, OnMouseMove);

        private void NotifyVector2Action(InputAction.CallbackContext context, Action<Vector2> action)
        {
            var vector2 = context.ReadValue<Vector2>();
            action?.Invoke(vector2);
        }
    }
}