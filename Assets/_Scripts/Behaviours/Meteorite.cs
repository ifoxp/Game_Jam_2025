using _Scripts.DataModel;
using UnityEngine;
using Zenject;

namespace _Scripts.Behaviours
{
    public class Meteorite : MonoBehaviour
    {
        public Drone targetedDrone;
        public Vector2Int price;
        private GameResourcesInventory _inventory;
        [Inject]
        private void Construct(GameResourcesInventory inventory)
        {
            _inventory = inventory;
        }
        private void Start()
        {

        }
            private void OnTriggerEnter(Collider other)
        {
            // Перевіряємо, чи тригер має тег DeleteMeteorite
            if (other.CompareTag("DeleteMeteorite"))
            {
                Debug.Log(_inventory);
                if(_inventory!=null)
                _inventory.AddResource(GameResourcesType.Junk, (Random.Range(price.x,price.y)));
                // Видаляємо метеорит
                Destroy(gameObject);
            }
        }
    }
}
