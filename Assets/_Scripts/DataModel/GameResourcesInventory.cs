using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameResourcesType
{
    Food,
    Population,
    Junk,
    Materials,
    Energy
};

namespace _Scripts.DataModel
{
    [Serializable]
    public class GameResourcesInventory : IDisposable
    {
        // I'm sorry. I haven't ever make save system for sessions.
        public event Action<GameResourceContainer> OnResourceChanged;
        
        // <Resource, quantity>
        private Dictionary<GameResourcesType, int> _gameResources = new()
        {
            [GameResourcesType.Food] = 0,
            [GameResourcesType.Population] = 0,
            [GameResourcesType.Junk] = 0,
            [GameResourcesType.Materials] = 0,
            [GameResourcesType.Energy] = 0
        };
        
        public Dictionary<GameResourcesType, int> GameResources => _gameResources;

        public void Dispose()
        {
            OnResourceChanged = null;
        }
        
        /// <summary>
        /// Invokes on game loading
        /// </summary>
        /// <param name="data">data to load</param>
        public void InitializeResources(Dictionary<GameResourcesType, int> data)
        {
            _gameResources = data;
            
            foreach (var resource in _gameResources)
            {
                OnResourceChanged?.Invoke(new GameResourceContainer(resource.Key, resource.Value));
            }
        }
        
        /// <summary>
        /// Method to spend resource
        /// </summary>
        /// <param name="resource">Resource to sped</param>
        /// <param name="amountAdd">POSITIVE amount to add</param>
        public void AddResource(GameResourcesType resource, int amountAdd)
        {
            if (amountAdd <= 0) return;
            _gameResources[resource] += amountAdd;
            
            OnResourceChanged?.Invoke(new GameResourceContainer(resource, _gameResources[resource]));
            Debug.Log($"{resource}: {_gameResources[resource]}");
        }
        
        /// <summary>
        /// Method to spend resource
        /// </summary>
        /// <param name="resource">Resource to sped</param>
        /// <param name="amountToSpend">POSITIVE amount to spend</param>
        public void SpendResource(GameResourcesType resource, int amountToSpend)
        {
            if (amountToSpend <= 0) return;
            _gameResources[resource] -= amountToSpend;
            
            OnResourceChanged?.Invoke(new GameResourceContainer(resource, _gameResources[resource]));
        }

        public GameResourceContainer GetResource(GameResourcesType resource)
        {
            return new GameResourceContainer(resource, _gameResources[resource]);
        }

        public GameResourceContainer[] GetAllResources()
        {
            var result = new GameResourceContainer[_gameResources.Count];
            var index = 0;
            foreach (var resource in _gameResources)
            {
                result[index].ResourceType = resource.Key;
                result[index].Quantity = resource.Value;

                index++;
            }

            return result;
        }
    }
}