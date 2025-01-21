using System;
using NaughtyAttributes;
using UnityEngine;

namespace _Scripts.Managers
{
    /// <summary>
    /// Singleton class that manages meteorites spawn.
    /// </summary>
    public class MeteoritesManager : MonoBehaviour
    {
        public static MeteoritesManager One { get; private set; }
        
        private void Awake()
        {
            if (One == null)
                One = this;
            else
                Destroy(gameObject);
        }
        
        
        public event Action OnMeteoriteSpawned;
        
        [Button]
        public void SpawnMeteorite()
        {
            // Spawn a meteorite
            OnMeteoriteSpawned?.Invoke();
        }
    }
}