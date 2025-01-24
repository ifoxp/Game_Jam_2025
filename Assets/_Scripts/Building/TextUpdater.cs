using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private TextMeshProUGUI textMesh; // Посилання на TextMeshProUGUI
    [SerializeField] private string resourceKey;       // Назва ресурсу в PlayerPrefs

    private void Start()
    {
        // Перевірка, чи існує ключ у PlayerPrefs, якщо ні — створити його зі значенням за замовчуванням
        if (!PlayerPrefs.HasKey(resourceKey))
        {
            PlayerPrefs.SetInt(resourceKey, 0); // Значення за замовчуванням
        }

        // Ініціалізація тексту
        UpdateText();
    }

    private void FixedUpdate()
    {
        // Оновлення тексту в кожному кадрі
        UpdateText();
    }

    private void UpdateText()
    {
        // Отримання значення з PlayerPrefs
        int resourceValue = PlayerPrefs.GetInt(resourceKey);

        // Оновлення тексту TextMeshProUGUI
        textMesh.text = $"{resourceKey}: {resourceValue}";
    }
}
