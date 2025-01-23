using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
    public class GameResourcesInventory : IInitializable, IDisposable
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

        public void Initialize()
        {
        }

        public void Dispose()
        {
            OnResourceChanged = null;
        }
        
        public void InitializeResources(int food, int population, int junk, int materials, int energy)
        {
            _gameResources[GameResourcesType.Food] = food;
            _gameResources[GameResourcesType.Population] = population;
            _gameResources[GameResourcesType.Junk] = junk;
            _gameResources[GameResourcesType.Materials] = materials;
            _gameResources[GameResourcesType.Energy] = energy;
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