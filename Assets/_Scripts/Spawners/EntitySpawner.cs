using _Scripts.Entities.Interfaces;
using _Scripts.Utilities;
using UnityEngine;
using Zenject;

namespace _Scripts.Spawners
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private byte _spawnPerWave = 3;
        [SerializeField] private SpawnSettings _spawnSettings;
        
        private IEntityFactory _entityFactory;
        
        [Inject]
        public void Construct(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        private void Start()
        {
            InvokeRepeating(nameof(SpawnEntity), _spawnSettings.spawnDelay, _spawnSettings.spawnDelay);
        }

        private void SpawnEntity()
        {
            for (var i = 0; i < _spawnPerWave; i++)
            {
                var entity = _entityFactory.CreateEntity(RandomAdditional.
                    GetRandomPositionInBoxCollider(_spawnSettings.spawnArea),transform);
            
                //Debug.Log($"Spawned entity: {entity}");
            }
        }
    }
}