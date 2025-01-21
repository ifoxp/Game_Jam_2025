using UnityEngine;
using _Scripts.Entities.Interfaces;
using Zenject;

namespace _Scripts.Entities.Factories
{
    public class MeteoriteFactory : IEntityFactory
    {
        private readonly Meteorite[] _meteoritePrefabs;

        [Inject]
        public MeteoriteFactory(Meteorite[] meteoritePrefabs)
        {
            _meteoritePrefabs = (Meteorite[])meteoritePrefabs.Clone();
        }
        
        public IEntity CreateEntity(Vector3 position)
        {
            var meteorite = Object.Instantiate(_meteoritePrefabs[
                Random.Range(0, _meteoritePrefabs.Length)], position, Quaternion.identity);
            
            meteorite.Initialize();
            return meteorite;
        }
    }
}