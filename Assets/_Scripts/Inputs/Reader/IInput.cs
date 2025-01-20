using System;
using UnityEngine;

namespace _Scripts.Inputs.Reader
{
    public interface IInput
    {
        public event Action<Vector2> OnMouseWheelScroll;
        public event Action<Vector2> OnMouseMove;
        
        public event Action<bool> OnLeftMousePressed;
    }
}