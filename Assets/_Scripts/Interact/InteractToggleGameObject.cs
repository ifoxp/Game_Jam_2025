using _Scripts.Behaviours;
using UnityEngine;

namespace _Scripts.Interact
{
    public class InteractToggleGameObject : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private GameObject _gameObjectToToggle;

        [SerializeField] private GameObject droneToBuy;
        
        // He -> https://github.com/ShrAmix asked me to add this variable
        // [SerializeField] private uint _price;
        // He -> https://github.com/erthmaster commented that line
        
        public void OnInteractByPointer()
        {
            if(droneToBuy.activeInHierarchy) return;
            
            _gameObjectToToggle.SetActive(true);
            DroneBuyPanel.One.droneToBuy = droneToBuy;
        }

        public void OnStopInteractByPointer()
        {
            _gameObjectToToggle.SetActive(false);
        }
    }
}