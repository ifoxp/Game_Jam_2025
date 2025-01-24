using System;
using System.Linq;
using _Scripts.Controllers;
using _Scripts.DataModel;
using Mono.Cecil;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Scripts._BuildingsEarn
{
    [RequireComponent(typeof(ResourceEarnerBuilding))]
    [RequireComponent(typeof(BuildingContainerInfo))]
    public class UpgradableBuilding : MonoBehaviour
    {
        [SerializeField] private LevelStats[] _eachLevelSettings;
        private ResourceEarnerBuilding _currentBuilding;

        [SerializeField] private ParticlesPlayer _dustParticle;
        
        private uint _maxLevelArrayIndex;
        [field: SerializeField] public uint CurrentLevelArrayIndex { get; private set; }
        
        private GameResourcesInventory _gameResourcesInventory;
        
        private const byte START_LEVEL = 0;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _gameResourcesInventory = gameResourcesInventory;
        }
        
        // For test!!! Use OnBuildingPlaced instead of Start()
        // Also erase this when test completed
        private void Start() =>
            OnBuildingPlaced();
        
        public void OnBuildingPlaced()
        {
            _currentBuilding = GetComponent<ResourceEarnerBuilding>();
            _dustParticle.Construct(GetComponentInChildren<ParticleSystem>());
            
            _maxLevelArrayIndex = (uint)_eachLevelSettings.Length - 1;
            SetLevel(START_LEVEL);
        }
        
        [Button]
        public void UpgradeBuilding()
        { 
            if(CurrentLevelArrayIndex == _maxLevelArrayIndex) return;
            
            var nextLevel = (uint)Mathf.Clamp(
                CurrentLevelArrayIndex + 1, 0, _maxLevelArrayIndex);
            
            if(!CanUpgrade(nextLevel)) return;
            
            CurrentLevelArrayIndex = nextLevel;
            SetLevel(CurrentLevelArrayIndex);
        }

        public bool CanUpgrade(uint levelIndex)
        {
            if(CurrentLevelArrayIndex >= _maxLevelArrayIndex) return false;
            
            var requirements = _eachLevelSettings[levelIndex].RequiredToUpgrade;
            
            return requirements.All(requirement => _gameResourcesInventory.
                GameResources[requirement.RequiredResource] >= requirement.RequiredAmount);
        }

        private void SetLevel(uint levelByArrayIndex)
        {
            if (levelByArrayIndex > _maxLevelArrayIndex)
            {
                Debug.LogWarning("You cannot set level higher than Max Level");
                return;
            }
            
            var newStats = _eachLevelSettings[levelByArrayIndex];
            _currentBuilding.ChangeBuildingStats(newStats);

            _dustParticle?.Play();
        }
        
        [Serializable]
        public class LevelStats
        {
            [field: SerializeField] public int NewResourceAmountPerMinute { get; private set; }
            
            [field: SerializeField] public UpgradeRequirements[] RequiredToUpgrade { get; private set; }

            [Serializable]
            public class UpgradeRequirements
            {
                [field: SerializeField] public GameResourcesType RequiredResource { get; private set; }
                [field: SerializeField] public int RequiredAmount { get; private set; }
            }
        }
    }
}