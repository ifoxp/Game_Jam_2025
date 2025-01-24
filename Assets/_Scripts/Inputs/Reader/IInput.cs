using System;
using UnityEngine;

namespace _Scripts.Inputs.Reader
{
    public interface IInput
    {
        public event Action<Vector2> OnMouseWheelScroll;
        public event Action<Vector2> OnMouseMove;
        public event Action<bool> OnToggleMoveCamera;

        public event Action<bool> OnLeftMousePressed;
        public event Action OnInteract;

        public event Action OnReturnToMapCenter;
        public event Action<bool> OnSelectContentPressed;

        public Vector2 GetPointerPosition();
    }
}