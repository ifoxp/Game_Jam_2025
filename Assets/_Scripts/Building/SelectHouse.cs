using _Scripts.Building;
using TMPro;
using UnityEngine;

public class SelectHouse: MonoBehaviour
{
    public int junk;
    public int materials;
    public int population;
    public int indexBuild;
    public BuildingSystem buildingSystem;
    public void BuyUI()
    {
        buildingSystem.BuildUGUI(indexBuild, junk, materials,population);
    }
}
