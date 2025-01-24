using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Interact
{
    public class InteractBuildingControl : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private Button _destroyBuildingButton;
        [SerializeField] private Button _upgradeBuildingButton;

        [SerializeField] private GameObject _buildingController;

        private HouseGenerateResource _houseGenerateResource;
        
        private void Awake()
        {
            _houseGenerateResource = GetComponent<HouseGenerateResource>();
            
            _destroyBuildingButton.onClick.RemoveAllListeners();
            _upgradeBuildingButton.onClick.RemoveAllListeners();
        }

        public void OnInteractByPointer()
        {
            _buildingController.SetActive(true);
            
            _destroyBuildingButton.onClick.RemoveAllListeners();
            _upgradeBuildingButton.onClick.RemoveAllListeners();

            _destroyBuildingButton.onClick.AddListener(GetComponent<InteractShowBuildingStat>().OnStopInteractByPointer);
            _destroyBuildingButton.onClick.AddListener(OnStopInteractByPointer);
            
            _destroyBuildingButton.onClick.AddListener(DestroyBuilding);
            
            _upgradeBuildingButton.onClick.AddListener(UpgradeBuilding);
        }

        public void OnStopInteractByPointer()
        {
            _buildingController.SetActive(false);

            _destroyBuildingButton.onClick.RemoveAllListeners();
            _upgradeBuildingButton.onClick.RemoveAllListeners();
        }

        private void DestroyBuilding()
        {
            Destroy(_houseGenerateResource.gameObject);
        }

        private void UpgradeBuilding()
        {
            _houseGenerateResource.Upgrade();
        }
    }
}