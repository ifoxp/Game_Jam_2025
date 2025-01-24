using _Scripts._BuildingsEarn;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers
{
    public class BuildingManagerSelectorUI : MonoBehaviour
    {
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _removeButton;
        
        private BuildingContainerInfo _currentBuildingContainer;

        private void Awake()
        {
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if(_upgradeButton == null)
                throw new MissingComponentException("Building manager selector can't work without UpgradeButton");
            if(_removeButton == null)
                throw new MissingComponentException("Building manager selector can't work without RemoveButton");
        }

        public void OnStopPressingOnBuilding()
        {
        }

        public void OnPressedOnBuilding(BuildingContainerInfo containerInfo)
        {
            if(containerInfo is null) return;
            
            _currentBuildingContainer = containerInfo;
            GiveButtonsFunctionalities();
        }

        private void GiveButtonsFunctionalities()
        {
            if(_currentBuildingContainer == null) return;
            
            SetupUpgradeButton();
            SetupDestroyButton();
        }

        private void SetupUpgradeButton()
        {
            _upgradeButton.onClick.RemoveAllListeners();
            _upgradeButton.onClick.AddListener(() => 
                _currentBuildingContainer.UpgradableBuilding.UpgradeBuilding());
        }

        private void SetupDestroyButton()
        {
            _removeButton.onClick.RemoveAllListeners();
            _removeButton.onClick.AddListener(() => _currentBuildingContainer.DestroyBuilding());
        }
    }
}