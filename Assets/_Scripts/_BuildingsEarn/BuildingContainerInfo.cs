using _Scripts.Interact;
using _Scripts.Managers;
using UnityEngine;
using Zenject;

namespace _Scripts._BuildingsEarn
{
    public class BuildingContainerInfo : MonoBehaviour, IInteractableByPointer
    {
        [field: SerializeField] public UpgradableBuilding UpgradableBuilding { get; private set; }
        
        private BuildingManagerSelectorUI _buildingManagerSelectorUI;
        
        [Inject]
        private void Construct(BuildingManagerSelectorUI buildingManagerSelectorUI)
        {
            _buildingManagerSelectorUI = buildingManagerSelectorUI;
        }

        private void Awake()
        {
            if(_buildingManagerSelectorUI == null) 
                throw new MissingComponentException("BuildingManagerSelectorUI is null");
        }
        
        public bool CanUpgradeCurrentBuilding() =>
            UpgradableBuilding.CanUpgrade(UpgradableBuilding.CurrentLevelArrayIndex);
        
        private void Reset()
        {
            UpgradableBuilding = GetComponent<UpgradableBuilding>();
        }

        public void OnInteractByPointer()
        {
            _buildingManagerSelectorUI.OnPressedOnBuilding(this);
        }

        public void OnStopInteractByPointer()
        {
            _buildingManagerSelectorUI.OnStopPressingOnBuilding();
        }

        public void DestroyBuilding()
        {
            Destroy(gameObject);
        }
    }
}