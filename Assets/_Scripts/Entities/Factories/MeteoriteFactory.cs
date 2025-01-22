using UnityEngine;
using _Scripts.Entities.Interfaces;
using Zenject;

namespace _Scripts.Entities.Factories
{
    public class MeteoriteFactory : IEntityFactory
    {
        private readonly FlyingMeteorite[] _meteoritePrefabs;

        [Inject]
        public MeteoriteFactory(FlyingMeteorite[] meteoritePrefabs)
        {
            _meteoritePrefabs = (FlyingMeteorite[])meteoritePrefabs.Clone();
        }
        
        public IEntity CreateEntity(Vector3 position, Transform parent)
        {
            var meteorite = Object.Instantiate(_meteoritePrefabs[
                Random.Range(0, _meteoritePrefabs.Length)], position, Quaternion.identity,parent);
            
            meteorite.Initialize();
            return meteorite;
        }
    }
}