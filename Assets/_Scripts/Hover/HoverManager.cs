using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Utilities;
using _Scripts.Inputs.Reader;
using Zenject;

namespace _Scripts.Hover
{
    public class HoverManager : MonoBehaviour
    {
        [SerializeField] private Camera _playerCamera;

        private Vector2 _lastPointerPosition;
        
        private IInput _input;
        private CastersAdditional _pointerCaster;

        private List<IHoverable> _currentHovered = new();

        private const float RAY_CHECK_DISTANCE = 150f; 

        [Inject]
        private void Construct(IInput input)
        {
            _input = input;
        }

        private void Awake()
        {
            if (_input == null) throw new MissingComponentException("Input is null");
            
            _pointerCaster = new CastersAdditional();
        }

        private void Update()
        {
            var currentPointerPosition = _input.GetPointerPosition();

            if (_lastPointerPosition == currentPointerPosition)
            {
                return;
            }

            _lastPointerPosition = currentPointerPosition;

            var newHovered = _pointerCaster.GetComponentsByPointer<IHoverable>(
                currentPointerPosition, _playerCamera, RAY_CHECK_DISTANCE
            );
            
            if (newHovered.Length == 0)
            {
                if (_currentHovered.Count != 0)
                {
                    InvokeAllUnHovered(_currentHovered);
                    _currentHovered.Clear();
                }
                return;
            }

            var newHoveredList = newHovered.ToList();

            var toUnHover = _currentHovered.Where(hoverable => !newHoveredList.Contains(hoverable)).ToList();
            var toHover = newHoveredList.Where(hoverable => !_currentHovered.Contains(hoverable)).ToList();

            if (toUnHover.Count > 0)
            {
                InvokeAllUnHovered(toUnHover);
            }

            if (toHover.Count > 0)
            {
                InvokeAllHovered(toHover);
            }

            _currentHovered = newHoveredList;
        }

        private void InvokeAllHovered(List<IHoverable> toInvoke)
        {
            foreach (var hoverable in toInvoke)
            {
                hoverable.Hovered();
            }
        }

        private void InvokeAllUnHovered(List<IHoverable> toInvoke)
        {
            foreach (var hoverable in toInvoke)
            {
                hoverable.UnHovered();
            }
        }

        #if UNITY_EDITOR
        private void Reset()
        {
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main;

                if (_playerCamera == null)
                {
                    Debug.LogWarning("Player camera is not assigned to HoverManager. Please set it manually.");
                }
            }
        }
        #endif
    }
}
