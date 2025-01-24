using _Scripts.DataModel;
using UnityEngine;
using Zenject;

namespace _Scripts._BuildingsEarn
{
    public class ResourceEarnerBuilding : MonoBehaviour
    {
        [SerializeField] private GameResourcesType _resourceToEarn;
        [SerializeField] private int _currentResourceAmountPerMinute;

        private float _timeToGetOneResource;
        
        private GameResourcesInventory _gameResourcesInventory;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _gameResourcesInventory = gameResourcesInventory;
        }

        private void GetOneResource()
        {
            _gameResourcesInventory.AddResource(_resourceToEarn, 1);
        }
        
        private float CalculateWhenGotOneResource()
        {
            var resourcePerSecond = (float)_currentResourceAmountPerMinute / 60;

            // Time to get one resource
            return 1 / resourcePerSecond;
        }

        public void ChangeBuildingStats(UpgradableBuilding.LevelStats newLevelStats)
        {
            _currentResourceAmountPerMinute = newLevelStats.NewResourceAmountPerMinute;
            _timeToGetOneResource = CalculateWhenGotOneResource();
            
            CancelInvoke(nameof(GetOneResource));
            InvokeRepeating(nameof(GetOneResource), _timeToGetOneResource, _timeToGetOneResource);

            Debug.Log($"Building stats changed! Per minute: {_currentResourceAmountPerMinute}");
        }
    }
}