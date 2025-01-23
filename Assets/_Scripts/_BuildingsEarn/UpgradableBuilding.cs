using System;
using _Scripts.Controllers;
using NaughtyAttributes;
using UnityEngine;

namespace _Scripts._BuildingsEarn
{
    [RequireComponent(typeof(ResourceEarnerBuilding))]
    public class UpgradableBuilding : MonoBehaviour
    {
        [SerializeField] private LevelStats[] _eachLevelSettings;
        private ResourceEarnerBuilding _currentBuilding;

        [SerializeField] private ParticlesPlayer _dustParticle;
        
        private uint _maxLevelArrayIndex;
        private uint _currentLevelArrayIndex;
        
        private const byte START_LEVEL = 0;

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
            if(_currentLevelArrayIndex == _maxLevelArrayIndex) return;
            
            _currentLevelArrayIndex = (uint)Mathf.Clamp(
                _currentLevelArrayIndex + 1, 0, _maxLevelArrayIndex);
            
            SetLevel(_currentLevelArrayIndex);
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
        }
    }
}