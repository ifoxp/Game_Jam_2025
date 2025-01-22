using System;
using UnityEngine;

namespace _Scripts.Utilities
{
    public class CastersAdditional
    {
        public GameObject GetGameObjectByPointer(Vector2 pointerPosition, Camera cameraWhereCasts, float distance)
        {
            var pointerWorldPosition = 
                cameraWhereCasts.ScreenToWorldPoint(new Vector3(
                    pointerPosition.x, pointerPosition.y, cameraWhereCasts.nearClipPlane));
            
            var cameraPosition = cameraWhereCasts.transform.position;
            var direction = (pointerWorldPosition - cameraPosition).normalized;

            return Physics.Raycast(cameraPosition, direction, out var hit, distance)
                ? hit.collider.gameObject : null;
        }

        public T[] GetComponentsByPointer<T>(Vector2 pointerPosition, Camera cameraWhereCasts, float distance)
        {
            var pointerWorldPosition = 
                cameraWhereCasts.ScreenToWorldPoint(new Vector3(
                    pointerPosition.x, pointerPosition.y, cameraWhereCasts.nearClipPlane));
            
            var cameraPosition = cameraWhereCasts.transform.position;
            var direction = (pointerWorldPosition - cameraPosition).normalized;

            return Physics.Raycast(cameraPosition, direction, out var hit, distance)
                ? hit.collider.GetComponents<T>() : Array.Empty<T>();
        }
    }
}