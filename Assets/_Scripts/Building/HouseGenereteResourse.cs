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
        // Генеруємо унікальний ID для цієї будівлі
        buildingID = gameObject.name + "_Level";

        // Завантажуємо поточний рівень збереження
        LoadLevel();

        // Ініціалізація PlayerPrefs для всіх ресурсів
        InitializePlayerPrefs();

        // Ініціалізація таймера
        productionTimer = productionInterval;
    }

    private void Update()
    {
        // Зменшуємо таймер
        productionTimer -= Time.deltaTime;

        // Якщо час виробництва настав
        if (productionTimer <= 0f)
        {
            // Скидаємо таймер
            productionTimer = productionInterval;

            // Перевіряємо і виробляємо ресурси
            if (CanProduceResources())
            {
                ProduceResources();
                Debug.Log("Ресурси успішно створені на рівні " + (currentLevel + 1));
            }
            else
            {
                Debug.Log("Недостатньо ресурсів для виробництва на рівні " + (currentLevel + 1));
            }

            // Виведення поточного стану ресурсів
            DisplayResources();
        }
    }

    private void InitializePlayerPrefs()
    {
        // Ініціалізація всіх ресурсів для кожного рівня
        foreach (var level in upgradeLevels)
        {
            foreach (var resource in level.resourcesProduced)
            {
                if (!PlayerPrefs.HasKey(resource.Name))
                {
                    PlayerPrefs.SetInt(resource.Name, 0);
                }
            }

            foreach (var resource in level.resourcesRequired)
            {
                if (!PlayerPrefs.HasKey(resource.Name))
                {
                    PlayerPrefs.SetInt(resource.Name, 0);
                }
            }
        }
    }

    private bool CanProduceResources()
    {
        // Перевірка ресурсів для поточного рівня
        foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
        {
            int currentAmount = PlayerPrefs.GetInt(requirement.Name, 0);
            if (currentAmount < requirement.Amount)
            {
                return false;
            }
        }
        return true;
    }

    private void ProduceResources()
    {
        // Витрата необхідних ресурсів
        foreach (var requirement in upgradeLevels[currentLevel].resourcesRequired)
        {
            int currentAmount = PlayerPrefs.GetInt(requirement.Name);
            PlayerPrefs.SetInt(requirement.Name, currentAmount - requirement.Amount);
        }

        // Додавання вироблених ресурсів
        foreach (var production in upgradeLevels[currentLevel].resourcesProduced)
        {
            int currentAmount = PlayerPrefs.GetInt(production.Name);
            PlayerPrefs.SetInt(production.Name, currentAmount + production.Amount);
        }
    }

    private void DisplayResources()
    {
        Debug.Log("Поточний рівень: " + (currentLevel + 1));
        foreach (var resource in upgradeLevels[currentLevel].resourcesProduced)
        {
            Debug.Log(resource.Name + ": " + PlayerPrefs.GetInt(resource.Name));
        }

        foreach (var resource in upgradeLevels[currentLevel].resourcesRequired)
        {
            Debug.Log(resource.Name + ": " + PlayerPrefs.GetInt(resource.Name));
        }
    }

    public void Upgrade()
    {
        // Переходимо на наступний рівень, якщо можливо
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

    private void LoadLevel()
    {
        if (PlayerPrefs.HasKey(buildingID))
        {
            currentLevel = PlayerPrefs.GetInt(buildingID);
            Debug.Log("Рівень будівлі завантажено: " + currentLevel);
        }
        else
        {
            Debug.Log("Рівень будівлі не знайдено, використовується базовий рівень.");
        }
    }
    [Button]
    public void UppateBuild()
    {
        Upgrade();
    }
}

[System.Serializable]
public class ResourceData
{
    public string Name;  // Назва ресурсу (наприклад, Food, Energy)
    public int Amount;   // Кількість ресурсу
}

[System.Serializable]
public class UpgradeLevel
{
    public List<ResourceData> resourcesProduced; // Ресурси, які виробляються на цьому рівні
    public List<ResourceData> resourcesRequired; // Ресурси, необхідні для виробництва на цьому рівні
}
