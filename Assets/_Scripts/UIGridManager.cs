using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIGridManager : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject uiPanel; // Панель, в яку будуть додаватись елементи
    [SerializeField] private GameObject uiElementPrefab; // Префаб елемента, який буде додаватись до UI
    [SerializeField] private float elementSpacing = 10f; // Відступ між елементами

    // Відступи від країв панелі
    [SerializeField] private float paddingTop = 20f; // Відступ зверху
    [SerializeField] private float paddingBottom = 20f; // Відступ знизу
    [SerializeField] private float paddingLeft = 20f; // Відступ зліва
    [SerializeField] private float paddingRight = 20f; // Відступ справа

    private int columnsCount; // Кількість стовпців, які можна вмістити
    private int rowsCount; // Кількість рядків, які можна вмістити
    private float elementHeight; // Висота кожного елемента
    private float elementWidth; // Ширина кожного елемента

    private List<ResourceData> resources = new List<ResourceData>(); // Список ресурсів

    // Додано змінну для зберігання посилання на HouseGenerateResource
    public HouseGenerateResource houseGenerateResource;

    void Start()
    {
        // Отримуємо розміри елемента для правильного розташування
        RectTransform elementRect = uiElementPrefab.GetComponent<RectTransform>();
        elementHeight = elementRect.rect.height;
        elementWidth = elementRect.rect.width;

        // Отримуємо розміри панелі
        RectTransform panelRect = uiPanel.GetComponent<RectTransform>();
        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        // Розрахунок кількості стовпців і рядків на основі розміру панелі та елементів
        columnsCount = Mathf.FloorToInt((panelWidth - paddingLeft - paddingRight) / (elementWidth + elementSpacing));
        rowsCount = Mathf.FloorToInt((panelHeight - paddingTop - paddingBottom) / (elementHeight + elementSpacing));

        // Створюємо елементи та додаємо їх до панелі
        CreateElements();
    }

    public void SetTotalElements(int total)
    {
        // Оновлюємо кількість елементів для створення
        resources.Clear();
    }

    public void SetResources()
    {
        CreateElements();
    }

    // Метод для створення елементів
    private void CreateElements()
    {
        // Спавнимо лише ту кількість елементів, яку можна відобразити на панелі
        int maxElements = columnsCount * rowsCount;
        int elementsToSpawn = Mathf.Min(resources.Count, maxElements);

        // Отримуємо ресурси поточного і наступного рівня
        List<ResourceData> currentLevelResources, nextLevelResources;
        houseGenerateResource.GetResourcesForCurrentAndNextLevel(out currentLevelResources, out nextLevelResources);

        // Створюємо елементи для кожного ресурсу
        for (int i = 0; i < elementsToSpawn; i++)
        {
            // Створюємо новий елемент з префабу
            GameObject newElement = Instantiate(uiElementPrefab, uiPanel.transform);

            // Розраховуємо позицію елемента на основі його індексу
            int column = i / rowsCount; // Поточний стовпець
            int row = i % rowsCount; // Поточний рядок

            // Розрахунок позиції елемента, з урахуванням відступів
            float xPosition = paddingLeft + column * (elementWidth + elementSpacing);
            float yPosition = -paddingTop - row * (elementHeight + elementSpacing);

            // Встановлюємо позицію елемента
            RectTransform elementRectTransform = newElement.GetComponent<RectTransform>();
            elementRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);

            // Встановлюємо текст для ресурсу
            TextMeshProUGUI resourceText = newElement.GetComponentInChildren<TextMeshProUGUI>();
            if (resourceText != null)
            {
                ResourceData resource = resources[i];

                // Поточний рівень ресурсів
                int currentAmount = resource.Amount;

                // Шукаємо ресурс на наступному рівні
                int nextAmount = currentAmount; // Спочатку рівно поточному значенню

                // Шукаємо ресурс у списку для наступного рівня
                ResourceData nextLevelResource = nextLevelResources.Find(r => r.Name == resource.Name);
                if (nextLevelResource != null)
                {
                    nextAmount = nextLevelResource.Amount;
                }

                // Формуємо текст у вигляді "Food: 10->15" або "Food: 10", якщо зміни немає
                string resourceName = resource.Name.ToString(); // Отримуємо ім'я ресурсу
                resourceText.text = (currentAmount != nextAmount)
                    ? $"{resourceName}: {currentAmount}->{nextAmount}"
                    : $"{resourceName}: {currentAmount}";
            }
        }
    }
}
