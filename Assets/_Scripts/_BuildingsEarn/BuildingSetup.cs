using System;
using _Scripts._BuildingsEarn.Systems;
using NaughtyAttributes;
using UnityEngine;

namespace _Scripts._BuildingsEarn
{
    public class BuildingSetup : MonoBehaviour
    {
        // Mixed
        private bool _showEarnResourcesPerMinute;
        
        // Earn
        private bool _addedEarnSystem;

        [ShowIf("_addedEarnSystem")] [SerializeField]
        private GameResourcesType _resourceToEarn;

        [ShowIf("_showEarnResourcesPerMinute")] [SerializeField]
        private int _earnResourcesPerMinute;
        
        // Upgradable
        private bool _addedUpgradable;

        [ShowIf("_addedUpgradable")] [SerializeField]
        private AllLevelContainer[] _upgradeLevelsStats;
        
        #region Add-Remove specific components
        
        [Button]
        private void AddEarnSystem()
        {
            AddComponent<ResourceEarnerBuilding>(true);
            _addedEarnSystem = true;

            _showEarnResourcesPerMinute = _addedEarnSystem && !_addedUpgradable;
        }
        
        [Button]
        private void RemoveEarnSystem()
        {
            RemoveComponent<ResourceEarnerBuilding>();
            _addedEarnSystem = false;
            
            _showEarnResourcesPerMinute = _addedEarnSystem && !_addedUpgradable;
        }

        [Button]
        private void AddUpgradeSystem()
        {
            AddComponent<UpgradableBuilding>(true);
            AddComponent<ResourceEarnerBuilding>(false);
            
            _addedUpgradable = true;
            _addedEarnSystem = true;
            
            Debug.Log("Upgradable depends on Resource earner");
            
            _showEarnResourcesPerMinute = _addedEarnSystem && !_addedUpgradable;
        }

        [Button]
        private void RemoveUpgradeSystem()
        {
            RemoveComponent<UpgradableBuilding>();
            _addedUpgradable = false;

            _showEarnResourcesPerMinute = _addedEarnSystem && !_addedUpgradable;
        }

        [Button]
        private void AddConsumableSystem()
        {
            AddComponent<ConsumableBuilding>(true);
        }

        [Button]
        private void RemoveConsumableSystem()
        {
            RemoveComponent<ConsumableBuilding>();
        }
        
        #endregion

        #region T-Adder/Remover
        
        private void RemoveComponent<T>() where T : Component
        {
            if (TryGetComponent(out T earner))
            {
                    DestroyImmediate(earner);
                Debug.Log($"<color=green>{typeof(T).Name} deleted successfully!</color>");
            }
            else
            {
                Debug.Log($"<color=yellow>Building doesn't have an {typeof(T).Name}</color>");
            }
        }

        private void AddComponent<T>(bool showExistWarn) where T : Component
        {
            if (TryGetComponent(out T _))
            {
                if(showExistWarn)
                    Debug.Log($"<color=yellow>Building already has an {typeof(T).Name}</color>");
            }
            else
            {
                gameObject.AddComponent<T>();
                
                if(showExistWarn) 
                    Debug.Log($"<color=green>{typeof(T).Name} added successfully!</color>");
            }
        }
        
        #endregion

        [Button]
        private void InitializeAll()
        {
            var upgradable = GetComponent<UpgradableBuilding>();
            var earner = GetComponent<ResourceEarnerBuilding>();
            var consumable = GetComponent<ConsumableBuilding>();
            
            if (!upgradable && !earner)
            {
                Debug.LogWarning("No components added!");
                return;
            }

            if (consumable)
            {
                consumable.ConstructorGetData(_upgradeLevelsStats);
            }

            earner.SetResourceToEarn(_resourceToEarn);

            if (upgradable)
            {
                upgradable.ConstructorGetData(_upgradeLevelsStats);
            }
            else
            {
                earner.ConstructorNoUpgrades(_earnResourcesPerMinute);
            }
        }
        
        private void HideWarnings()
        {
            if (_addedEarnSystem) return;
            if (_addedUpgradable) return;
            if (_showEarnResourcesPerMinute) return;
        }

        [Serializable]
        public class AllLevelContainer
        {
            [field: SerializeField] public uint NewResourcePerMinute { get; private set; }
            [field: SerializeField] public ResourceContainer[] RequirementsToUpgrade { get; private set; }
            
            [field: Space]
            [field: SerializeField] public int ConsumePerMinute { get; private set; }
            [field: SerializeField] public ResourceContainer[] Consumable { get; private set; }
        }

        [Serializable]
        public class ResourceContainer
        {
            [field: SerializeField] public GameResourcesType GameResource { get; private set; }
            [field: SerializeField] public int ResourceAmount { get; private set; }
        }
    }
}