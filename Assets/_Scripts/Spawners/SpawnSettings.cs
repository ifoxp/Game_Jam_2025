using System;
using UnityEngine;

namespace _Scripts.Spawners
{
    [Serializable]
    public class SpawnSettings
    {
        public BoxCollider spawnArea;
        public float spawnDelay;
    }
}