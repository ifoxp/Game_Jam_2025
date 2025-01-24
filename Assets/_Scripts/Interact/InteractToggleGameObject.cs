using UnityEngine;

namespace _Scripts.Interact
{
    public class InteractToggleGameObject : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private GameObject _gameObjectToToggle;
        [SerializeField] private DroneBuy droneBuy;
        // He -> https://github.com/ShrAmix asked me to add this variable
        //[SerializeField] private uint _price;

        public void OnInteractByPointer()
        {
            _gameObjectToToggle.SetActive(true);
            if (droneBuy != null)
            {
                droneBuy.enabled = true;
            }
        }

        public void OnStopInteractByPointer()
        {
            _gameObjectToToggle.SetActive(false);
            if (droneBuy != null)
            {
                droneBuy.enabled = false;
            }
        }
       

        public bool IsActive()
        {
            return _gameObjectToToggle.activeSelf;
        }
    }
}