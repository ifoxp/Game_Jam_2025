using UnityEngine;

namespace _Scripts.Interact
{
    public class InteractToggleGameObject : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private GameObject _gameObjectToToggle;

        // He -> https://github.com/ShrAmix asked me to add this variable
        [SerializeField] private uint _price;
        
        public void OnInteractByPointer()
        {
            _gameObjectToToggle.SetActive(true);
        }

        public void OnStopInteractByPointer()
        {
            _gameObjectToToggle.SetActive(false);
        }
    }
}