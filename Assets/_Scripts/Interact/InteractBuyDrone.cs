using _Scripts.Behaviours;
using UnityEngine;

namespace _Scripts.Interact
{
    public class InteractBuyDrone : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private GameObject droneToBuy;
        
        public void OnInteractByPointer()
        {
            DroneBuyPanel.One.droneToBuy = droneToBuy;
            print("Bruh");
        }

        public void OnStopInteractByPointer()
        {
            DroneBuyPanel.One.droneToBuy = null;
        }
    }
}