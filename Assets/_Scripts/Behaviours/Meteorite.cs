using _Scripts.DataModel;
using UnityEngine;
using Zenject;

namespace _Scripts.Behaviours
{
    public class Meteorite : MonoBehaviour
    {
        public Drone targetedDrone;
        public Vector2Int price;
     
        private void Start()
        {

        }
            private void OnTriggerEnter(Collider other)
        {
            // Перевіряємо, чи тригер має тег DeleteMeteorite
            if (other.CompareTag("DeleteMeteorite"))
            {
              
         
                PlayerPrefs.SetInt("Junk", PlayerPrefs.GetInt("Junk")+ (Random.Range(price.x, price.y)));
                // Видаляємо метеорит
                Destroy(gameObject);
            }
        }
    }
}
