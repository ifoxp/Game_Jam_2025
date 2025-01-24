using UnityEngine;

namespace _Scripts.Hover
{
    [RequireComponent(typeof(Outline))]
    public class HoverOutline : MonoBehaviour, IHoverable
    {
        [SerializeField] private Outline _outline;

        public void Hovered()
        {
            if (!_outline) return;
            _outline.enabled = true;
        }

        public void UnHovered()
        {
            if (!_outline) return;
            _outline.enabled = false;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (_outline != null) return;

            _outline = GetComponent<Outline>();
            _outline.enabled = false;

            var outlineColor = new Color(6f / 255f, 180f / 255f, 255f / 255f);
            _outline.OutlineColor = outlineColor;

            const float width = 5f;
            _outline.OutlineWidth = width;
        }
#endif
    }
}