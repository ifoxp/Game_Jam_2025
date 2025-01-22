using _ScriptableAssets.Building;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.Interact
{
    public class InteractShowBuildingInfo : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private BuildingInfo _buildingInfo;

        [Inject(Id = Keys.INTERACTABLE_INSTALLER)]
        private GameObject _buildingInfoParent;
        
        [Inject(Id = Keys.INTERACTABLE_INSTALLER)]
        private TextMeshProUGUI _buildingNameTextPlace;

        private void Awake()
        {
            if(_buildingInfoParent == null) throw new MissingComponentException("No building info parent is set");
            if(_buildingNameTextPlace == null) throw new MissingComponentException("No building name text place is set");
        }
        
        public void OnInteractByPointer()
        {
            _buildingInfoParent.SetActive(true);
            _buildingNameTextPlace.text = _buildingInfo.BuildingName;
        }

        public void OnStopInteractByPointer()
        {
            _buildingInfoParent.SetActive(false);
            _buildingNameTextPlace.text = string.Empty;
        }
    }
}