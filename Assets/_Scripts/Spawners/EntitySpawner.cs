using _Scripts.Entities.Interfaces;
using _Scripts.Utilities;
using UnityEngine;
using Zenject;

namespace _Scripts.Spawners
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private Vector2Int _spawnPerWave=new Vector2Int(3,5);

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
            byte spawner = (byte)(Random.Range(_spawnPerWave.x, _spawnPerWave.y));
            for (var i = 0; i < spawner; i++)
            {
                var entity = _entityFactory.CreateEntity(RandomAdditional.
                    GetRandomPositionInBoxCollider(_spawnSettings.spawnArea),transform);
            
                //Debug.Log($"Spawned entity: {entity}");
            }
        }
    }
}