using UnityEngine;

namespace _Scripts.Utilities
{
    public static class RandomAdditional
    {
        public static Vector3 GetRandomPositionInBoxCollider(BoxCollider boxCollider)
        {
            var center = boxCollider.center;
            var size = boxCollider.size * 0.5f;
            
            var randomX = Random.Range(-size.x, size.x);
            var randomY = Random.Range(-size.y, size.y);
            var randomZ = Random.Range(-size.z, size.z);

            var randomPointLocal = center + new Vector3(randomX, randomY, randomZ);

            return boxCollider.transform.TransformPoint(randomPointLocal);
        }
    }
}