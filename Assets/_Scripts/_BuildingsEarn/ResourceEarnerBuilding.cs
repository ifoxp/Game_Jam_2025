using System;
using _Scripts.DataModel;
using UnityEngine;
using Zenject;

namespace _Scripts._BuildingsEarn
{
    public class ResourceEarnerBuilding : MonoBehaviour
    {
        [SerializeField] private GameResourcesType _resourceToEarn;
        [SerializeField] private int _resourceAmountPerMinute;
        
        private float _timeToGetOneResource;
        
        private GameResourcesInventory _gameResourcesInventory;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _gameResourcesInventory = gameResourcesInventory;
        }
        
        private void Start()
        {
            if(_resourceAmountPerMinute <= 0) 
                throw new Exception("Resource amount per minute must be greater than 0");
            
            _timeToGetOneResource = CalculateWhenGotOneResource();
            InvokeRepeating(nameof(GetOneResource), _timeToGetOneResource, _timeToGetOneResource);
        }

        private void GetOneResource()
        {
            _gameResourcesInventory.AddResource(_resourceToEarn, 1);
        }
        
        private float CalculateWhenGotOneResource()
        {
            var resourcePerSecond = (float)_resourceAmountPerMinute / 60;

            // Time to get one resource
            return 1 / resourcePerSecond;
        }
    }
}