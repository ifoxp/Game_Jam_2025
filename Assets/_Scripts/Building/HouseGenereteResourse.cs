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
    private Dictionary<ResourceType, float> resourceProgress = new Dictionary<ResourceType, float>();


    private void Start()
    {
        buildingID = gameObject.name + "_Level";

        LoadLevel();
        InitializeResourceProgress();

        InitializePlayerPrefs();
        productionTimer = productionInterval;

        productionTimer = productionInterval;
    }
    private void InitializeResourceProgress()
    {
        foreach (var resource in upgradeLevels[currentLevel].resourcesProduced)
        {
            if (!resourceProgress.ContainsKey(resource.Name))
            {
                resourceProgress[resource.Name] = 0f;
            }
        }

        foreach (var resource in upgradeLevels[currentLevel].resourcesRequired)
        {
            if (!resourceProgress.ContainsKey(resource.Name))
            {
                resourceProgress[resource.Name] = 0f;
            }
        }
    }


    private void Update()
    {
        if (currentLevel >= 0 && CanUseRequiredResources())
        {
            float deltaTime = Time.deltaTime; // Час між кадрами

            // Процес виробництва ресурсів
            foreach (var production in upgradeLevels[currentLevel].resourcesProduced)
            {
                float resourcePerSecond = (float)production.Amount / 60f; // Кількість на секунду

                // Оновлюємо прогрес для ресурсу
                resourceProgress[production.Name] += resourcePerSecond * deltaTime;

                // Якщо прогрес досяг 1 або більше, додаємо ресурс
                if (resourceProgress[production.Name] >= 1f)
                {
                    int integerResource = Mathf.FloorToInt(resourceProgress[production.Name]);
                    AddResource(production.Name, integerResource); // Додаємо ресурси
                    resourceProgress[production.Name] -= integerResource; // Залишок прогресу
                }
            }

            // Процес віднімання необхідних ресурсів
            foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
            {
                float resourcePerSecond = (float)requirement.Amount / 60f; // Кількість на секунду для віднімання

                // Оновлюємо прогрес для віднімання ресурсу
                resourceProgress[requirement.Name] -= resourcePerSecond * deltaTime;

                // Якщо прогрес досяг 1 або більше, віднімаємо ресурс
                if (resourceProgress[requirement.Name] <= -1f)
                {
                    int integerResource = Mathf.FloorToInt(-resourceProgress[requirement.Name]);
                    SubtractResource(requirement.Name, integerResource); // Віднімаємо ресурси
                    resourceProgress[requirement.Name] += integerResource; // Залишок прогресу
                }
            }
        }

        DisplayResources();
    }

    private bool CanUseRequiredResources()
{
    foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
    {
        int currentAmount = PlayerPrefs.GetInt(requirement.Name.ToString(), 0);
        if (currentAmount<= 0)
        {
            return false;
        }
    }
    return true;
}

    private void AddResource(ResourceType resourceType, int amount)
    {
        int currentAmount = PlayerPrefs.GetInt(resourceType.ToString());
        int maxStorage = PlayerPrefs.GetInt(resourceType.ToString() + "Save", 0); // Максимальний об'єм сховища

        if (currentAmount + amount <= maxStorage)
        {
            PlayerPrefs.SetInt(resourceType.ToString(), currentAmount + amount);
        }
        else
        {
            int availableSpace = maxStorage - currentAmount;
            if (availableSpace > 0)
            {
                PlayerPrefs.SetInt(resourceType.ToString(), currentAmount + availableSpace);
            }
            Debug.Log($"Сховище для ресурсу {resourceType} заповнене!");
        }
    }

    private void SubtractResource(ResourceType resourceType, int amount)
    {
        // Перевірка, чи є сховище для цього ресурсу
        int maxStorage = PlayerPrefs.GetInt(resourceType.ToString() + "Save", 0);

        // Якщо сховище не порожнє, можна віднімати
        if (maxStorage >= 0)
        {
            int currentAmount = PlayerPrefs.GetInt(resourceType.ToString());
            int newAmount = currentAmount - amount;

            if (newAmount < 0)
            {
                PlayerPrefs.SetInt(resourceType.ToString(), 0); // Якщо ресурсів менше, ніж потрібно, скидаємо до 0
            }
            else
            {
                PlayerPrefs.SetInt(resourceType.ToString(), newAmount);
            }
        }
        else
        {
            Debug.Log($"Не можна віднімати ресурси {resourceType}, оскільки сховище для цього ресурсу порожнє.");
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
        // Перевіряємо, чи є необхідні ресурси для виробництва
        if (CanUseRequiredResources())
        {
            // Віднімаємо ресурси, необхідні для виробництва
            foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
            {
                int currentAmount = PlayerPrefs.GetInt(requirement.Name.ToString());
                int newAmount = currentAmount - requirement.Amount;
                PlayerPrefs.SetInt(requirement.Name.ToString(), newAmount);
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
        else
        {
            Debug.Log("Недостатньо ресурсів для виробництва!");
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
    public void GetResourcesForCurrentAndNextLevel(
     out List<ResourceData> currentLevelProducedResources,
     out List<ResourceData> currentLevelRequiredResources,
     out List<ResourceData> nextLevelProducedResources,
     out List<ResourceData> nextLevelRequiredResources)
    {
        // Ініціалізуємо списки
        currentLevelProducedResources = new List<ResourceData>();
        currentLevelRequiredResources = new List<ResourceData>();
        nextLevelProducedResources = new List<ResourceData>();
        nextLevelRequiredResources = new List<ResourceData>();

        // Отримуємо ресурси поточного рівня
        if (currentLevel >= 0 && currentLevel < upgradeLevels.Count)
        {
            currentLevelProducedResources.AddRange(upgradeLevels[currentLevel].resourcesProduced);
            currentLevelRequiredResources.AddRange(upgradeLevels[currentLevel].resourcesRequired);
        }

        // Якщо наступний рівень існує, додаємо ресурси наступного рівня
        if (currentLevel + 1 < upgradeLevels.Count)
        {
            nextLevelProducedResources.AddRange(upgradeLevels[currentLevel + 1].resourcesProduced);
            nextLevelRequiredResources.AddRange(upgradeLevels[currentLevel + 1].resourcesRequired);
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
