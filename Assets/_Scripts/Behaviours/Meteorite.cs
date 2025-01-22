using UnityEngine;

namespace _Scripts.Behaviours
{
    public class Meteorite : MonoBehaviour
    {
        public Drone targetedDrone;

        private void OnTriggerEnter(Collider other)
        {
            // Перевіряємо, чи тригер має тег DeleteMeteorite
            if (other.CompareTag("DeleteMeteorite"))
            {
                // Видаляємо метеорит
                Destroy(gameObject);
            }
        }
    }
}
