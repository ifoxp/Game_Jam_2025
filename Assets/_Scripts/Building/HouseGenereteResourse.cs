using _Scripts.Interact;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class HouseGenerateResource : MonoBehaviour
{
    [Header("Рівні апгрейду")]
    [SerializeField] private List<UpgradeLevel> upgradeLevels;

    [Header("Інтервал виробництва")]
    [SerializeField] private float productionInterval = 5f; // Час між виробництвом (у секундах)

    private float productionTimer;
    private int currentLevel = 0; // Поточний рівень (0 - базовий рівень)
    [SerializeField] private string buildingID; // Унікальний ідентифікатор будівлі

    private void Start()
    {
        buildingID = gameObject.name + "_Level";

        LoadLevel();

        InitializePlayerPrefs();

        productionTimer = productionInterval;
    }

    private void Update()
    {
        productionTimer -= Time.deltaTime;

        if (productionTimer <= 0f)
        {
            productionTimer = productionInterval;

            if (currentLevel >= 0 && CanProduceResources())
            {
                ProduceResources();
            }

            DisplayResources();
        }
    }

    private void InitializePlayerPrefs()
    {
        foreach (var level in upgradeLevels)
        {
            foreach (var resource in level.resourcesProduced)
            {
                if (!PlayerPrefs.HasKey(resource.Name.ToString()))
                {
                    PlayerPrefs.SetInt(resource.Name.ToString(), 0);
                }

                // Ініціалізуємо сховище ресурсу
                if (!PlayerPrefs.HasKey(resource.Name.ToString() + "Save"))
                {
                    PlayerPrefs.SetInt(resource.Name.ToString() + "Save", 100); // 100 - стандартний розмір сховища
                }
            }

            foreach (var resource in level.resourcesRequired)
            {
                if (!PlayerPrefs.HasKey(resource.Name.ToString()))
                {
                    PlayerPrefs.SetInt(resource.Name.ToString(), 0);
                }
            }
        }
    }


    private bool CanProduceResources()
    {
        if (currentLevel < 0) return false;

        foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
        {
            int currentAmount = PlayerPrefs.GetInt(requirement.Name.ToString(), 0);
            if (currentAmount < requirement.Amount)
            {
                return false;
            }
        }
        return true;
    }

    private void ProduceResources()
    {
        // Віднімаємо ресурси, необхідні для виробництва
        foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
        {
            int currentAmount = PlayerPrefs.GetInt(requirement.Name.ToString());
            PlayerPrefs.SetInt(requirement.Name.ToString(), currentAmount - requirement.Amount);
        }

        // Додаємо ресурси, які виробляються
        foreach (var production in upgradeLevels[currentLevel].resourcesProduced)
        {
            int currentAmount = PlayerPrefs.GetInt(production.Name.ToString());
            int maxStorage = PlayerPrefs.GetInt(production.Name.ToString() + "Save", 0); // Максимальний об'єм сховища

            // Перевірка, чи є місце в сховищі
            if (currentAmount + production.Amount <= maxStorage)
            {
                PlayerPrefs.SetInt(production.Name.ToString(), currentAmount + production.Amount);
            }
            else
            {
                // Якщо сховище заповнене, додаємо тільки те, що залишилось до максимального об'єму
                int availableSpace = maxStorage - currentAmount;
                if (availableSpace > 0)
                {
                    PlayerPrefs.SetInt(production.Name.ToString(), currentAmount + availableSpace);
                }
                Debug.Log($"Сховище для ресурсу {production.Name} заповнене!");
            }
        }
    }


    private void DisplayResources()
    {
        foreach (var resource in upgradeLevels[currentLevel].resourcesProduced)
        {
        }

        foreach (var resource in upgradeLevels[currentLevel].resourcesRequired)
        {
        }
    }

    public void Upgrade()
    {
        if (currentLevel + 1 < upgradeLevels.Count)
        {
            currentLevel++;
            SaveLevel();
            Debug.Log("Рівень підвищено до: " + (currentLevel + 1));
        }
        else
        {
            Debug.Log("Максимальний рівень досягнуто!");
        }
    }

    private void SaveLevel()
    {
        PlayerPrefs.SetInt(buildingID, currentLevel);
        PlayerPrefs.Save();
        Debug.Log("Рівень будівлі збережено: " + currentLevel);
    }
    public void GetResourcesForCurrentAndNextLevel(out List<ResourceData> currentLevelResources, out List<ResourceData> nextLevelResources)
    {
        currentLevelResources = new List<ResourceData>();
        nextLevelResources = new List<ResourceData>();

        // Отримуємо ресурси поточного рівня
        if (currentLevel >= 0 && currentLevel < upgradeLevels.Count)
        {
            currentLevelResources.AddRange(upgradeLevels[currentLevel].resourcesProduced);
            currentLevelResources.AddRange(upgradeLevels[currentLevel].resourcesRequired);
        }

        // Якщо наступний рівень існує, додаємо ресурси наступного рівня
        if (currentLevel + 1 < upgradeLevels.Count)
        {
            nextLevelResources.AddRange(upgradeLevels[currentLevel + 1].resourcesProduced);
            nextLevelResources.AddRange(upgradeLevels[currentLevel + 1].resourcesRequired);
        }
    }




    private void LoadLevel()
    {
        if (PlayerPrefs.HasKey(buildingID))
        {
            currentLevel = PlayerPrefs.GetInt(buildingID);
            //Debug.Log("Рівень будівлі завантажено: " + currentLevel);
        }
        else
        {
            //Debug.Log("Рівень будівлі не знайдено, використовується базовий рівень.");
        }
    }

    [Button]
    public void UppateBuild()
    {
        Upgrade();
    }
}

public enum ResourceType
{
    Food,
    Junk,
    Material,
    Energy,
    Population,
    PopulationActive
}

[System.Serializable]
public class ResourceData
{
    [SerializeField]
    public ResourceType Name;

    public int Amount;   // Кількість ресурсу
}

[System.Serializable]
public class UpgradeLevel
{
    public List<ResourceData> resourcesProduced; // Ресурси, які виробляються на цьому рівні
    public List<ResourceData> resourcesRequired; // Ресурси, необхідні для виробництва на цьому рівні
}
