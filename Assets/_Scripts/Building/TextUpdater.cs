using System.Collections;
using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private TextMeshProUGUI textMesh; // Посилання на TextMeshProUGUI

    // Якщо 'ResourceType' вже існує, використовуйте його без змін
    [SerializeField] private ResourceType resourceType;

    private int lastResourceValue;
    private void Start()
    {
        // Перевірка, чи існує ключ у PlayerPrefs, якщо ні — створити його зі значенням за замовчуванням
        if (!PlayerPrefs.HasKey(resourceType.ToString()))
        {
            PlayerPrefs.SetInt(resourceType.ToString(), 0); // Значення за замовчуванням
        }

        // Ініціалізація тексту
        lastResourceValue = PlayerPrefs.GetInt(resourceType.ToString());
        UpdateText();
    }

    private void Update()
    {
        // Отримуємо нове значення з PlayerPrefs
        int currentResourceValue = PlayerPrefs.GetInt(resourceType.ToString());

        // Оновлюємо текст тільки якщо значення змінилося
        if (currentResourceValue != lastResourceValue)
        {
            lastResourceValue = currentResourceValue;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        // Оновлення тексту TextMeshProUGUI
        textMesh.text = $"{resourceType}: {lastResourceValue}";
    }
}

// Новий enum для типів ресурсів
public enum BuildingResourceType
{
    Food,
    Junk,
    Material,
    Energy,
    Population,
    PopulationActive
}
