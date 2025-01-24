using UnityEngine;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Food = 50;
    public int Junk = 100;
    public int Material = 150;
    public int Energy = 50;
    public int Population = 40;
    public int PopulationActive = 0;
    private void Start()
    {
        if (PlayerPrefs.GetInt("Population") == 0)
            SetDefaultPrefs();
    }
    public void SetDefaultPrefs()
    {
        // Видалення всіх префабів (це очищає PlayerPrefs)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        // Встановлення кастомних значень
        PlayerPrefs.SetInt("Food", Food);
        PlayerPrefs.SetInt("Junk", Junk);
        PlayerPrefs.SetInt("Material", Material);
        PlayerPrefs.SetInt("Energy", Energy);
        PlayerPrefs.SetInt("Population", Population);
        PlayerPrefs.SetInt("PopulationActive", PopulationActive);
    }
}

