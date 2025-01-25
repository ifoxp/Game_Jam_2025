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

    public HouseGenerateResource houseGenerateResource; // Змінна для зберігання посилання на HouseGenerateResource

    // Масив спрайтів для різних типів ресурсів
    [SerializeField] private Sprite[] resourceSprites;

    // Оновлюємо ресурси та створюємо елементи
    List<ResourceData> currentLevelResources, nextLevelResources;
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
        SetResources();
    }

    public void SetTotalElements(int total)
    {
        // Оновлюємо кількість елементів для створення
        resources.Clear();
    }

    public void SetResources()
    {
        if (houseGenerateResource == null) return;
        houseGenerateResource.GetResourcesForCurrentAndNextLevel(
            out List<ResourceData> currentLevelProduced,
            out List<ResourceData> currentLevelRequired,
            out List<ResourceData> nextLevelProduced,
            out List<ResourceData> nextLevelRequired);

        // Якщо наступного рівня немає, спавнимо тільки поточний рівень
        if (nextLevelProduced.Count == 0 && nextLevelRequired.Count == 0)
        {
            CreateElementsForSingleLevel(currentLevelProduced, currentLevelRequired);
        }
        else
        {
            // Об'єднуємо ресурси поточного та наступного рівня, але уникати дублювання
            resources.Clear();
            HashSet<string> addedResources = new HashSet<string>(); // Множина для зберігання унікальних імен ресурсів

            // Додаємо ресурси поточного рівня для resourcesProduced
            foreach (var resource in currentLevelProduced)
            {
                if (!addedResources.Contains(resource.Name.ToString()))
                {
                    resources.Add(resource);
                    addedResources.Add(resource.Name.ToString());
                }
            }

            // Додаємо ресурси наступного рівня для resourcesProduced
            foreach (var resource in nextLevelProduced)
            {
                if (!addedResources.Contains(resource.Name.ToString()))
                {
                    resources.Add(resource);
                    addedResources.Add(resource.Name.ToString());
                }
            }

            // Додаємо ресурси поточного рівня для resourcesRequired
            foreach (var resource in currentLevelRequired)
            {
                if (!addedResources.Contains(resource.Name.ToString()))
                {
                    resources.Add(resource);
                    addedResources.Add(resource.Name.ToString());
                }
            }

            // Додаємо ресурси наступного рівня для resourcesRequired
            foreach (var resource in nextLevelRequired)
            {
                if (!addedResources.Contains(resource.Name.ToString()))
                {
                    resources.Add(resource);
                    addedResources.Add(resource.Name.ToString());
                }
            }

            // Створюємо елементи для поточного та наступного рівня
           /* CreateElements(currentLevelProduced, currentLevelRequired, nextLevelProduced, nextLevelRequired);*/
        }
    }


    // Метод для створення елементів
   /* private void CreateElements(
     List<ResourceData> currentLevelProduced,
     List<ResourceData> currentLevelRequired,
     List<ResourceData> nextLevelProduced,
     List<ResourceData> nextLevelRequired)
    {
        // Спавнимо лише ту кількість елементів, яку можна відобразити на панелі
        int maxElements = columnsCount * rowsCount;
        int elementsToSpawn = Mathf.Min(resources.Count, maxElements);

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
            Image resourceImage = newElement.GetComponentInChildren<Image>(); // Отримуємо компонент Image
            if (resourceText != null && resourceImage != null)
            {
                ResourceData resource = resources[i];

                // Поточний рівень ресурсів
                int currentAmount = resource.Amount;

                // Шукаємо ресурс на наступному рівні
                int nextAmount = currentAmount; // Спочатку рівно поточному значенню

                // Шукаємо ресурс у списку для наступного рівня
                ResourceData nextLevelResource = nextLevelProduced.Find(r => r.Name == resource.Name);
                if (nextLevelResource != null)
                {
                    nextAmount = nextLevelResource.Amount;
                }

                // Перевіряємо тип ресурсу: produced або required
                if (currentLevelProduced.Contains(resource) || nextLevelProduced.Contains(resource))
                {
                    // Якщо ресурс належить до resourcesProduced
                    string displayText = (currentAmount != nextAmount)
                        ? $"+{currentAmount}->{GetColoredAmount(currentAmount, nextAmount)}"
                        : $"+{currentAmount}";

                    resourceText.text = displayText;
                }
                else
                {
                    // Якщо ресурс належить до resourcesRequired
                    ResourceData nextRequiredResource = nextLevelRequired.Find(r => r.Name == resource.Name);
                    int nextRequiredAmount = nextRequiredResource != null ? nextRequiredResource.Amount : currentAmount;

                    string displayText = (currentAmount != nextRequiredAmount)
                        ? $"-{currentAmount}->{GetColoredAmountR(currentAmount, nextRequiredAmount)}"
                        : $"-{currentAmount}";

                    resourceText.text = displayText;
                }

                // Встановлюємо спрайт для ресурсу
                if (newElement.name != "Image") // Перевірка на об'єкт Table
                {
                    int spriteIndex = GetResourceTypeIndex(resource.Name); // Отримуємо індекс спрайта за типом ресурсу
                    resourceImage.sprite = resourceSprites[spriteIndex]; // Встановлюємо відповідний спрайт
                }
            }
        }
    }
   */
    private void CreateElementsForSingleLevel(
    List<ResourceData> currentLevelProduced,
    List<ResourceData> currentLevelRequired)
    {
        // Спавнимо лише ту кількість елементів, яку можна відобразити на панелі
        int maxElements = columnsCount * rowsCount;
        int elementsToSpawn = Mathf.Min(currentLevelProduced.Count + currentLevelRequired.Count, maxElements);

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
            Image resourceImage = newElement.GetComponentInChildren<Image>(); // Отримуємо компонент для картинки

            if (resourceText != null && resourceImage != null)
            {
                ResourceData resource;
                bool isProducedResource = i < currentLevelProduced.Count;

                // Вибираємо ресурс з поточного рівня (produced або required)
                if (isProducedResource)
                {
                    resource = currentLevelProduced[i];
                    resourceText.color = Color.green; // Встановлюємо зелений колір для produced ресурсів
                    resourceText.text = $"+{resource.Amount}"; // Текст для produced
                }
                else
                {
                    resource = currentLevelRequired[i - currentLevelProduced.Count];
                    resourceText.color = Color.red; // Встановлюємо червоний колір для required ресурсів
                    resourceText.text = $"-{resource.Amount}"; // Текст для required
                }

                // Встановлюємо спрайт для ресурсу
                if (newElement.name != "Table") // Перевірка, щоб не змінювати картинку для "Table"
                {
                    int spriteIndex = GetResourceTypeIndex(resource.Name); // Отримуємо індекс спрайта
                    resourceImage.sprite = resourceSprites[spriteIndex]; // Встановлюємо спрайт з масиву
                }
            }
        }
    }



    // Метод для отримання індексу спрайта за типом ресурсу
    private int GetResourceTypeIndex(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                return 0; // Food
            case ResourceType.Junk:
                return 1; // Junk
            case ResourceType.Material:
                return 2; // Material
            case ResourceType.Energy:
                return 3; // Energy
            case ResourceType.Population:
                return 4; // Population
            default:
                return 0; // Default to Food
        }
    }

    // Метод для отримання кольору для кількості ресурсу
    private string GetColoredAmount(int currentAmount, int nextAmount)
    {
        if (currentAmount < nextAmount)
        {
            return $"<color=green>{nextAmount}</color>";
        }
        else if (currentAmount > nextAmount)
        {
            return $"<color=red>{nextAmount}</color>";
        }
        else
        {
            return nextAmount.ToString();
        }
    }

    // Метод для отримання кольору для кількості ресурсу (required)
    private string GetColoredAmountR(int currentAmount, int nextAmount)
    {
        if (currentAmount < nextAmount)
        {
            return $"<color=red>{nextAmount}</color>";
        }
        else if (currentAmount > nextAmount)
        {
            return $"<color=green>{nextAmount}</color>";
        }
        else
        {
            return nextAmount.ToString();
        }
    }
}
