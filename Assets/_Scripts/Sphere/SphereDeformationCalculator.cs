using _Scripts.DataModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Для роботи з Image

public class SphereDeformationCalculator : MonoBehaviour
{
    [Tooltip("Масив дочірніх об'єктів, які мають компонент Rigidbody")]
    public Rigidbody[] childRigidbodies; // Масив дочірніх об'єктів із Rigidbody

    private Vector3[] initialPositions; // Початкові позиції дочірніх об'єктів
    public float totalDeformation; // Загальне значення деформації
    public TextMeshProUGUI textMeshProUGUI;
    public Image animationImage; // Компонент Image для анімації
    public float maxDeformation = 100f; // Поріг деформації для перезавантаження сцени
    private bool hasSceneReloaded = false; // Чи була сцена перезавантажена
    public int Mnoznik = 10;
    public bool seeLine = true;

    // Масив спрайтів, які будуть змінюватися в залежності від деформації
    public Sprite[] deformationSprites;

    private void Start()
    {
        // Знаходимо всі дочірні об'єкти з компонентом Rigidbody та зберігаємо їх початкові позиції
        childRigidbodies = GetComponentsInChildren<Rigidbody>();
        initialPositions = new Vector3[childRigidbodies.Length];

        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            initialPositions[i] = childRigidbodies[i].transform.position;
        }
    }

    private void FixedUpdate()
    {
        // Обчислюємо та оновлюємо деформацію, якщо об'єкти змістилися
        float newDeformation = CalculateDeformation();
        if (Mathf.Abs(newDeformation - totalDeformation) > 0.01f) // Якщо змінилася деформація
        {
            totalDeformation = newDeformation;
            UpdateText();
            UpdateImageAnimation();
        }

        // Перевірка на перевищення деформації 100 і перезавантаження сцени
        if (!hasSceneReloaded && totalDeformation > maxDeformation)
        {
            hasSceneReloaded = true;
            ReloadScene(); // Перезавантаження сцени
        }
    }

    // Обчислюємо загальне зміщення всіх об'єктів
    private float CalculateDeformation()
    {
        float deformation = 0f;
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            deformation += Vector3.Distance(initialPositions[i], childRigidbodies[i].transform.position);
        }
        return deformation;
    }

    // Оновлення тексту деформації в UI
    private void UpdateText()
    {
        if (textMeshProUGUI != null)
        {
            // Округляємо до найближчого цілого
            float roundedDeformation = Mathf.Round(totalDeformation * 100f) / Mnoznik;

            // Округлюємо до цілих чисел перед виведенням
            textMeshProUGUI.text = Mathf.RoundToInt(roundedDeformation).ToString();
        }
    }


    // Оновлення спрайту анімації
    private void UpdateImageAnimation()
    {
        if (animationImage != null && deformationSprites.Length > 0)
        {
            // Нормалізуємо деформацію від 0 до maxDeformation
            float normalizedDeformation = Mathf.Clamp(totalDeformation / maxDeformation, 0f, 1f);

            // Обчислюємо індекс для вибору спрайта на основі нормалізованої деформації
            int spriteIndex = Mathf.FloorToInt(normalizedDeformation * (deformationSprites.Length - 1));

            // Оновлюємо спрайт Image в залежності від деформації
            animationImage.sprite = deformationSprites[spriteIndex];
        }
    }

    // Перезавантаження поточної сцени
    private void ReloadScene()
    {
        GlobalSaveManager globalSaveManager=FindAnyObjectByType<GlobalSaveManager>();
        globalSaveManager.DeleteSave(true);
    }

    // Малюємо лінії між початковими і поточними позиціями об'єктів для наглядності
    private void OnDrawGizmos()
    {
        if (initialPositions != null && childRigidbodies != null && seeLine)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < initialPositions.Length; i++)
            {
                if (i < childRigidbodies.Length)
                {
                    Gizmos.DrawLine(initialPositions[i], childRigidbodies[i].transform.position);
                }
            }
        }
    }
}
