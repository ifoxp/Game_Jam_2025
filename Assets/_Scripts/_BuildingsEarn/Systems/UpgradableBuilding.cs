using System.Linq;
using _Scripts._BuildingsEarn.Systems.Interfaces;
using _Scripts.Controllers;
using _Scripts.DataModel;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace _Scripts._BuildingsEarn.Systems
{
    public class UpgradableBuilding : MonoBehaviour, IOnBuildingPlaced
    {
        [SerializeField] private BuildingSetup.AllLevelContainer[] _levelsStats;

        [Space] [SerializeField] 
        private bool _giveResourceWhenPlaced;
        
        [SerializeField] [ShowIf("_giveResourceWhenPlaced")]
        private BuildingSetup.ResourceContainer _giftOnBuildingPlaced;

        [Space, Header("UX")] [SerializeField] 
        private ParticlesPlayer _dustParticles;

        private GameResourcesInventory _gameResourcesInventory;

        private int _currentLevelIndex;
        private int _maxLevelIndex;
        
        private const byte START_LEVEL = 0;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _gameResourcesInventory = gameResourcesInventory;
        }

        public void ConstructorGetData(BuildingSetup.AllLevelContainer[] levels)
        {
            _levelsStats = (BuildingSetup.AllLevelContainer[])levels.Clone();
            
            SetLevel(START_LEVEL);
        }
        
        // For test!!! Use OnBuildingPlaced instead of Start()
        // Also erase this when test completed
        // private void Start() =>
        //     OnBuildingPlaced();
        
        public void OnBuildingPlaced()
        {
            // If building must give resources, when placed
            if (_giveResourceWhenPlaced)
            {
                if(_giftOnBuildingPlaced.ResourceAmount > 0) 
                    _gameResourcesInventory.AddResource(
                        _giftOnBuildingPlaced.GameResource, _giftOnBuildingPlaced.ResourceAmount);
            }
                
            _maxLevelIndex = _levelsStats.Length - 1;
            
            SetLevel(START_LEVEL);
        }
        
        [Button]
        public void UpgradeBuilding()
        { 
            if(_currentLevelIndex == _maxLevelIndex) return;
            
            var nextLevel = Mathf.Clamp(
                _currentLevelIndex + 1, 0, _maxLevelIndex);
            
            if(!CanUpgrade(nextLevel)) return;
            
            _currentLevelIndex = nextLevel;
            SetLevel(_currentLevelIndex);
        }

        public bool CanUpgrade(int levelIndex)
        {
            if(_currentLevelIndex >= _maxLevelIndex) return false;
            if(_levelsStats[levelIndex].RequirementsToUpgrade.Length == 0) return true;
            
            var requirements = _levelsStats[levelIndex].RequirementsToUpgrade;
            
            return requirements.All(requirement => _gameResourcesInventory.
                GameResources[requirement.GameResource] >= requirement.ResourceAmount);
        }

        private void SetLevel(int levelByArrayIndex)
        {
            if (levelByArrayIndex > _maxLevelIndex)
            {
                Debug.LogWarning("You cannot set level higher than Max Level");
                return;
            }
            
            var newStats = _levelsStats[levelByArrayIndex];
            GetComponent<ResourceEarnerBuilding>()?.ChangeBuildingStats(newStats);
            GetComponent<ConsumableBuilding>()?.SetNewLevel(levelByArrayIndex);

            _dustParticles?.Play();
        }
    }
}