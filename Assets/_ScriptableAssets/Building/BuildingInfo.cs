using UnityEngine;

namespace _ScriptableAssets.Building
{
    [CreateAssetMenu(fileName = "New Building Info", menuName = "GJ/Building/Info")]
    public class BuildingInfo : ScriptableObject
    {
        [field: SerializeField] public string BuildingName { get; private set; }
    }
}