using _Scripts._BuildingsEarn.Systems.Interfaces;
using _Scripts.DataModel;
using UnityEngine;
using Zenject;

namespace _Scripts._BuildingsEarn.Systems
{
    public class ResourceEarnerBuilding : MonoBehaviour, IOnBuildingPlaced
    {
        private bool _buildingActive = true;
        
        private GameResourcesType _resourceToEarn;
        private int _resourceAmountPerMinute;
        
        private float _timeToGetOneResource;
        private GameResourcesInventory _gameResourcesInventory;

        private bool _doesntHaveUpgrades;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _gameResourcesInventory = gameResourcesInventory;
        }

        public void ConstructorNoUpgrades(
            int resourcePerMinute)
        {
            _resourceAmountPerMinute = resourcePerMinute;

            _doesntHaveUpgrades = true;
        }

        public void SetResourceToEarn(GameResourcesType resourceToEarn)
        {
            _resourceToEarn = resourceToEarn;
        }

        private void GetOneResource()
        {
            _gameResourcesInventory.AddResource(_resourceToEarn, 1);
        }
        
        private float CalculateWhenGotOneResource(float resourceAmountPerMinute)
        {
            var resourcePerSecond = resourceAmountPerMinute / 60;

            // Time to get one resource
            return 1 / resourcePerSecond;
        }

        public void ChangeBuildingStats(BuildingSetup.AllLevelContainer newLevelStats)
        {
            CancelInvoke(nameof(GetOneResource));

            _resourceAmountPerMinute = (int)newLevelStats.NewResourcePerMinute;
            _timeToGetOneResource = CalculateWhenGotOneResource(_resourceAmountPerMinute);
            
            if(_buildingActive)
                InvokeRepeating(nameof(GetOneResource), _timeToGetOneResource, _timeToGetOneResource);
        }

        public void OnBuildingPlaced()
        {
            if (_doesntHaveUpgrades)
            {
                _timeToGetOneResource = CalculateWhenGotOneResource(_resourceAmountPerMinute);
                InvokeRepeating(nameof(GetOneResource), _timeToGetOneResource, _timeToGetOneResource);
            }
        }

        public void StopEarn()
        {
            _buildingActive = false;
            CancelInvoke(nameof(GetOneResource));
        }

        public void ContinueEarn()
        {
            _buildingActive = true;
            InvokeRepeating(nameof(GetOneResource), _timeToGetOneResource, _timeToGetOneResource);
        }
    }
}