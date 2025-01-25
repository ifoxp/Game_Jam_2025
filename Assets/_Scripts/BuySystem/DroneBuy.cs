using _Scripts.DataModel;
using _Scripts.Interact;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DroneBuy : MonoBehaviour
{
    public Button buttonBuy; // Кнопка покупки
    public TextMeshProUGUI priceText; // Текст з ціною
    public GameObject Drone; // Дрон, який потрібно активувати
    public InteractToggleGameObject interactToggleGameObject; // Контролер активації об'єкта

    [SerializeField] private uint[] _prices; // Масив цін для кожного рівня прокачки
    private int currentLevel; // Поточний рівень прокачки дрона
    private GameResourcesInventory _inventory;
    [Inject]
    private void Construct(GameResourcesInventory inventory)
    {
        _inventory = inventory;
    }
    private void Start()
    {
        // Завантаження поточного рівня прокачки з PlayerPrefs
        currentLevel = PlayerPrefs.GetInt("DroneBuyLevel", 0);

        // Оновлюємо текст ціни
        UpdatePriceText();

        // Відключаємо кнопку покупки, якщо об'єкт не активний
        buttonBuy.interactable = false;

        // Додаємо дію на кнопку покупки
        buttonBuy.onClick.AddListener(OnBuyButtonClick);
    }

    private void Update()
    {
        // Перевіряємо, чи активний _gameObjectToToggle
        if (interactToggleGameObject.IsActive() && currentLevel < _prices.Length)
        {
            // Якщо активний, оновлюємо текст ціни та дозволяємо натискати кнопку
            UpdatePriceText();
            buttonBuy.interactable = true;
        }
        else
        {
            // Якщо неактивний або максимальний рівень досягнуто, відключаємо кнопку
            buttonBuy.interactable = false;
        }
    }

    private void UpdatePriceText()
    {
        // Оновлюємо текст ціни відповідно до поточного рівня
        if (currentLevel < _prices.Length)
        {
            priceText.text = _prices[currentLevel].ToString();
        }
        else
        {
            priceText.text = "Max Level";
        }
    }

    private void OnBuyButtonClick()
    {
        if (currentLevel >= _prices.Length)
        {
            Debug.Log("Drone is already at max level.");
            return;
        }

        int currentMaterials=PlayerPrefs.GetInt("Material");

        if (currentMaterials >= _prices[currentLevel])
        {
            currentMaterials = PlayerPrefs.GetInt("Material") - (int)_prices[currentLevel];
            Debug.Log(currentMaterials + "+=" + (int)_prices[currentLevel]);
            PlayerPrefs.SetInt("Material", currentMaterials);

            // Підвищуємо рівень прокачки
            currentLevel++;
            PlayerPrefs.SetInt("DroneBuyLevel", currentLevel);
            PlayerPrefs.Save();

            // Активуємо дрон
            Drone.SetActive(true);
            interactToggleGameObject.OnStopInteractByPointer();
            gameObject.SetActive(false);

            Debug.Log($"Drone upgraded to level {currentLevel} successfully!");
        }
        else
        {
            Debug.Log("Not enough materials to upgrade the drone.");
        }
    }
}
