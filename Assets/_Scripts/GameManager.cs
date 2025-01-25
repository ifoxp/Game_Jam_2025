using NaughtyAttributes;
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

    // Максимальні значення для сховищ
    public int FoodSave = 500;
    public int JunkSave = 10000;
    public int MaterialSave = 1000;
    public int EnergySave = 750;
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
        // Встановлення максимальних значень для сховищ
        PlayerPrefs.SetInt("FoodSave", FoodSave);
        PlayerPrefs.SetInt("JunkSave", JunkSave);
        PlayerPrefs.SetInt("MaterialSave", MaterialSave);
        PlayerPrefs.SetInt("EnergySave", EnergySave);

    }
    [Button]
    private void GiveAllResources()
    {
        

        // Зберігаємо нові значення в PlayerPrefs
        PlayerPrefs.SetInt("Food", 1000);
        PlayerPrefs.SetInt("Junk", 1000);
        PlayerPrefs.SetInt("Material", 1000);
        PlayerPrefs.SetInt("Energy", 1000);
        PlayerPrefs.SetInt("Population", 1000);
        PlayerPrefs.SetInt("PopulationActive", 0);


        // Встановлення максимальних значень для сховищ
        PlayerPrefs.SetInt("FoodSave", FoodSave);
        PlayerPrefs.SetInt("JunkSave", JunkSave);
        PlayerPrefs.SetInt("MaterialSave", MaterialSave);
        PlayerPrefs.SetInt("EnergySave", EnergySave);

        // Не забудьте зберегти PlayerPrefs після змін
        PlayerPrefs.Save();

        // Можна вивести інформацію для відлагодження
        Debug.Log("All resources increased by 1000.");
    }

}

