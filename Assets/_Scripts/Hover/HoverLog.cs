using UnityEngine;

namespace _Scripts.Hover
{
    public class HoverLog : MonoBehaviour, IHoverable
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public void Hovered()
        {
            Debug.Log("Hovered!");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void UnHovered()
        {
            Debug.Log("Unhovered!");
        }
    }
}
