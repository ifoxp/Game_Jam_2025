using UnityEngine;

public class PopulationSave : MonoBehaviour
{
    [SerializeField] private int population;
    private float timer; // Таймер для відслідковування часу
    private bool isAdded = false; // Перевірка, чи вже додано значення

    private void Update()
    {
        if (!isAdded) // Якщо ще не додано значення
        {
            timer += Time.deltaTime; // Додаємо час, що пройшов з останнього кадру

            if (timer >= 1f) // Якщо пройшло 2 секунди
            {
                // Додаємо значення до Population
                PlayerPrefs.SetInt("Population", PlayerPrefs.GetInt("Population") + population);
                this.enabled=false;
                // Задаємо прапор, що значення вже додано
                isAdded = true;
            }
        }
    }
}
