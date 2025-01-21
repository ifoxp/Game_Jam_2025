using UnityEngine;

namespace _Scripts.Entities.Interfaces
{
    public interface IEntityFactory
    {
        public IEntity CreateEntity(Vector3 position);
    }
}