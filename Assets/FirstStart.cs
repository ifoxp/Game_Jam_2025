using UnityEngine;

public class FirstStart : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects; // Масив об'єктів для активації/деактивації

    private const string FirstLaunchKey = "FirstLaunch";

    private void Start()
    {
        // Перевіряємо, чи гра запускається вперше
        if (IsFirstLaunch())
        {
            // Якщо гра запускається вперше, активуємо всі об'єкти
            SetActiveForAllObjects(true);

            // Встановлюємо значення, щоб позначити, що гра була запущена
            PlayerPrefs.SetInt(FirstLaunchKey, 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Якщо це не перший запуск, деактивуємо всі об'єкти
            SetActiveForAllObjects(false);
        }
    }

    private bool IsFirstLaunch()
    {
        // Перевіряємо, чи є ключ у PlayerPrefs. Якщо ні, це перший запуск.
        return !PlayerPrefs.HasKey(FirstLaunchKey);
    }

    private void SetActiveForAllObjects(bool isActive)
    {
        foreach (var obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
