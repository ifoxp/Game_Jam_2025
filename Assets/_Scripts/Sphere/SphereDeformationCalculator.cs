using TMPro;
using UnityEngine;

public class SphereDeformationCalculator : MonoBehaviour
{
    [Tooltip("Масив дочірніх об'єктів, які мають компонент Rigidbody")]
    public Rigidbody[] childRigidbodies; // Масив дочірніх об'єктів із Rigidbody

    private Vector3[] initialPositions; // Початкові позиції дочірніх об'єктів
    public float totalDeformation; // Загальне значення деформації сфери
    public TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        // Знаходимо всі дочірні об'єкти з компонентом Rigidbody
        childRigidbodies = GetComponentsInChildren<Rigidbody>();

        // Зберігаємо початкові позиції дочірніх об'єктів
        initialPositions = new Vector3[childRigidbodies.Length];
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            initialPositions[i] = childRigidbodies[i].transform.position;
        }
    }

    private void FixedUpdate()
    {
        CalculateDeformation();

        // Оновлюємо текст у TextMeshProUGUI
        if (textMeshProUGUI != null)
        {
            float roundedDeformation = Mathf.Round(totalDeformation * 100f) / 10f; // Множимо на 10, округлюємо до 0.1
            textMeshProUGUI.text = $"Deformation: {roundedDeformation}";
        }
    }

    private void CalculateDeformation()
    {
        totalDeformation = 0f;

        // Обчислюємо загальне зміщення всіх об'єктів
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            float displacement = Vector3.Distance(initialPositions[i], childRigidbodies[i].transform.position);
            totalDeformation += displacement;
        }
    }

    private void OnDrawGizmos()
    {
        // Малюємо початкові та поточні позиції для наглядності
        if (initialPositions != null && childRigidbodies != null)
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
