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
        outline.enabled=false;
    }
}
