using System.Collections;
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

        // Запускаємо корутину для оновлення тексту кожні 2 секунди
        StartCoroutine(UpdateTextEveryTwoSeconds());
    }

    private IEnumerator UpdateTextEveryTwoSeconds()
    {
        while (true)
        {
            // Оновлюємо текст
            UpdateText();

            // Чекаємо 2 секунди перед наступним оновленням
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateText()
    {
        // Отримання значення з PlayerPrefs
        int resourceValue = PlayerPrefs.GetInt(resourceKey);

        // Оновлення тексту TextMeshProUGUI
        textMesh.text = $"{resourceKey}: {resourceValue}";
    }

}
