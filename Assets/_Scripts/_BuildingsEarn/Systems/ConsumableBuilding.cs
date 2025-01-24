using System;
using System.Linq;
using _Scripts._BuildingsEarn.Systems.Interfaces;
using _Scripts.DataModel;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace _Scripts._BuildingsEarn.Systems
{
    [RequireComponent(typeof(BuildingCentralizator))]
    public class ConsumableBuilding : MonoBehaviour, IOnBuildingPlaced
    {
        [ReadOnly]
        [SerializeField] private ConsumeContainer[] _levels;
        private int _currentLevel;

        private bool _isActive;

        private GameResourcesInventory _resourcesInventory;

        private float _timeToConsumeOne;

        [Inject]
        private void Construct(GameResourcesInventory resourcesInventory)
        {
            _resourcesInventory = resourcesInventory;
        }

        private void Awake()
        {
            if (_resourcesInventory == null) throw new MissingComponentException("Inventory is null");
            _resourcesInventory.OnResourceChanged += CheckIsEnoughResources;
        }

        private void CheckIsEnoughResources(GameResourceContainer resourceContainer)
        {
            if(_isActive) return;
            if (CanConsume())
            {
                GetComponent<BuildingCentralizator>()._StartWorking();
            }
        }
        
        public void ConstructorGetData(BuildingSetup.AllLevelContainer[] levelContainer)
        {
            _levels = GetConsumeDataFromAllLevelContainer(levelContainer);
        }
        
        private float CalculateWhenConsumeOneResource(float resourceAmountPerMinute)
        {
            var resourcePerSecond = resourceAmountPerMinute / 60;

            // Time to get one resource
            return 1 / resourcePerSecond;
        }

        private ConsumeContainer[] GetConsumeDataFromAllLevelContainer(BuildingSetup.AllLevelContainer[] levelContainers)
        {
            return levelContainers
                .Select(level => new ConsumeContainer(level.Consumable, level.ConsumePerMinute))
                .ToArray();
        }

        public void SetNewLevel(int level)
        {
            CancelInvoke(nameof(Consume));
            
            _currentLevel = level;
            
            if(_isActive) StartConsuming();
        }

        private void StartConsuming()
        {
            _timeToConsumeOne = CalculateWhenConsumeOneResource(_timeToConsumeOne);
            InvokeRepeating(nameof(Consume), _timeToConsumeOne, _timeToConsumeOne);
        }
        
        public void Consume()
        {
            if (!CanConsume())
            {
                GetComponent<BuildingCentralizator>()?._StopWorking();
            }
            else
            {
                foreach (var setting in _levels[_currentLevel].ConsumeSettings)
                {
                    _resourcesInventory.SpendResource(setting.GameResource, setting.ResourceAmount);
                }
            }
        }

        private bool CanConsume()
        {
            var consumeSettings = _levels[_currentLevel].ConsumeSettings;

            foreach (var setting in consumeSettings)
            {
                if (!_resourcesInventory.GameResources.TryGetValue(setting.GameResource, out var currentAmount) ||
                    currentAmount < setting.ResourceAmount)
                {
                    return false;
                }
            }

            return true;
        }
        
        public void OnBuildingPlaced()
        {
            _isActive = true;
            StartConsuming();
        }

        public void StopConsume()
        {
            CancelInvoke(nameof(Consume));
            _isActive = false;
        }

        public void ResumeConsuming()
        {
            _isActive = true;
        }

        [Serializable]
        public class ConsumeContainer
        {
            [field: SerializeField] public float TimeToConsume { get; private set; }
            [field: SerializeField] public BuildingSetup.ResourceContainer[] ConsumeSettings { get; private set; }

            public ConsumeContainer(BuildingSetup.ResourceContainer[] consumeSettings, float timeToConsume)
            {
                ConsumeSettings = consumeSettings;
                TimeToConsume = timeToConsume;
            }
        }
    }
}