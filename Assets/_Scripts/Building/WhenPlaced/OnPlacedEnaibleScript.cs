using _Scripts.Building.WhenPlaced;
using UnityEngine;

public class OnPlacedEnaibleScript : MonoBehaviour, IOnPlaced
{
    [SerializeField ] private MonoBehaviour[] _components;
    public void OnPlaced()
    {
        foreach (var component in _components)
        {
            component.enabled = true;
        }
    }
}
