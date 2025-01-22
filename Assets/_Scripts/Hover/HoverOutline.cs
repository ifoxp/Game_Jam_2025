using _Scripts.Hover;
using UnityEngine;
[RequireComponent(typeof(Outline))]
public class HoverOutline : MonoBehaviour, IHoverable
{
    [SerializeField] private Outline outline;
    public void Hovered()
    {
        outline.enabled = true;

    }

    public void UnHovered()
    {
        outline.enabled = false;
    }
    private void Reset()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
            Color outlineColor = new Color(6f / 255f, 180f / 255f, 255f / 255f);

            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = 5f;
        }
    }
}
