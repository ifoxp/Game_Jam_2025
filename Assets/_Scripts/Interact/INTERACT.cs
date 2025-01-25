using _Scripts.Interact;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class INTERACT : MonoBehaviour, IInteractableByPointer
{
    [Header("UI Prefab")]
    [SerializeField] private GameObject uiPrefab; // Префаб UI, який буде спавнитися
    [SerializeField] private string objectName; // Ім'я для відображення в TextMeshProUGUI
    private GameObject spawnedUI; // Посилання на створений UI
    private Canvas canvas; // Посилання на Canvas
    private bool isHouseClick=true;
    [SerializeField] private GameObject uiElementPrefab; // Префаб елемента, який буде додаватись до UI
    private void Start()
    {
        // Знаходимо Canvas у сцені
        canvas = FindObjectOfType<Canvas>();

        // Перевіряємо, чи Canvas знайдений
        if (canvas == null)
        {
            Debug.LogError("Canvas не знайдено у сцені! Переконайтеся, що він існує.");
        }
    }

    public void OnInteractByPointer()
    {
        // Перевіряємо, чи Canvas та префаб існують
        if (uiPrefab != null && spawnedUI == null && canvas != null)
        {
            // Спавнимо UI як дочірній елемент Canvas
            spawnedUI = Instantiate(uiPrefab, canvas.transform);

            // Встановлюємо позицію, яка визначена у префабі
            RectTransform uiRectTransform = spawnedUI.GetComponent<RectTransform>();
            if (uiRectTransform != null)
            {
                uiRectTransform.anchoredPosition = uiPrefab.GetComponent<RectTransform>().anchoredPosition;
            }

            // Знаходимо TextMeshProUGUI і встановлюємо текст для імені об'єкта
            TextMeshProUGUI nameText = spawnedUI.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = objectName; // Встановлюємо ім'я
            }

            // Отримуємо дані про ресурси для поточного рівня будівлі
            HouseGenerateResource houseGenerateResource = GetComponent<HouseGenerateResource>();
            if (houseGenerateResource != null)
            {
                List<ResourceData> resources = houseGenerateResource.GetResourcesForCurrentLevel();

                // Тепер передаємо ці ресурси в UIGridManager
                UIGridManager gridManager = spawnedUI.GetComponentInChildren<UIGridManager>();
                if (gridManager != null)
                {
                    // Встановлюємо загальну кількість елементів (totalElements)
                    gridManager.SetTotalElements(resources.Count);

                    // Передаємо список ресурсів для створення елементів
                    gridManager.SetResources(resources);
                }
            }

            // Знаходимо кнопки і прив'язуємо до них функції
            Button[] buttons = spawnedUI.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.name == "Remove")
                {
                    button.onClick.AddListener(OnRemoveButtonClicked);
                }
                else if (button.name == "Upgrade")
                {
                    button.onClick.AddListener(OnUpgradeButtonClicked);
                }
            }
        }
    }



    public void OnStopInteractByPointer()
    {
        // Перевіряємо, чи натиснуто на UI, щоб не викликати OnStopInteractByPointer
        if (spawnedUI != null && !IsPointerOverUI())
        {
            // Видаляємо створений UI, якщо ми не натискаємо на нього
            Destroy(spawnedUI);
            spawnedUI = null;
        }

    }

    // Перевірка, чи був натиснутий UI елемент
    private bool IsPointerOverUI()
    {
        // Створюємо рэйкаст для визначення, чи натискається на UI
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Ліст елементів, які потрапляють під рэйкаст
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        // Перевіряємо, чи натискається саме на спавнений UI
        foreach (var result in raycastResults)
        {
            if (result.gameObject == spawnedUI || result.gameObject.transform.IsChildOf(spawnedUI.transform))
            {
                
                return true; // Якщо натиснуто на спавнений UI
            }
        }
        return false; // Якщо натиснуто не на UI
    }


    // Функція, що викликається при натисканні кнопки Remove
    private void OnRemoveButtonClicked()
    {
        Debug.Log($"{objectName}: Remove button clicked!");
        // Видаляємо сам UI (батьківський об'єкт)
        Destroy(spawnedUI);
        Destroy(gameObject);
        spawnedUI = null;
    }

    // Функція, що викликається при натисканні кнопки Upgrade
    private void OnUpgradeButtonClicked()
    {
        if (GetComponent<HouseGenerateResource>() != null)
        {
            HouseGenerateResource houseGenerateResource = GetComponent<HouseGenerateResource>();
            houseGenerateResource.Upgrade();
        }
        // Тут реалізуйте функцію для апгрейду
    }
}
