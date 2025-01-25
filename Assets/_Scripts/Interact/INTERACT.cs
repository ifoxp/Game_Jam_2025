using _Scripts.Interact;
using UnityEngine;

public class INTERACT : MonoBehaviour, IInteractableByPointer
{
    public void OnInteractByPointer()
    {
        Debug.Log("ok");
    }

    public void OnStopInteractByPointer()
    {
        Debug.Log("Nook");
    }
}
