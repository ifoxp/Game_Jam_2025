using UnityEngine;

public class AirFlow : MonoBehaviour
{
    public Vector3 flowDirection;
    public float maxStrength = 10f;
    public Vector3 targetPosition;  // Точка, до якої ми вимірюємо відстань

    void Update()
    {
        // Обчислення сили потоку в залежності від відстані
        float distance = Vector3.Distance(transform.position, targetPosition);
        float strength = maxStrength / (distance + 0.1f);

        // Отримання напрямку потоку
        Vector3 force = flowDirection.normalized * strength;

        // Нанесення сили (можна використовувати для фізичних об'єктів)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(force);
        }
    }

    // Функція для злиття потоків
    public static Vector3 MergeAirFlows(Vector3 direction1, float strength1, Vector3 direction2, float strength2)
    {
        // Обчислення кута між напрямками
        float angle = Mathf.Acos(Vector3.Dot(direction1.normalized, direction2.normalized));

        // Якщо кути маленькі — з'єднуємо потоки, якщо великі — компенсуємо
        if (angle < Mathf.PI / 4) // Кут менший ніж 45 градусів
        {
            // Потоки підсилюються
            return (direction1.normalized * strength1 + direction2.normalized * strength2).normalized;
        }
        else
        {
            // Потоки компенсуються
            return direction1.normalized * Mathf.Max(strength1, strength2);
        }
    }

    // Візуалізація потоків за допомогою Gizmos
    void OnDrawGizmos()
    {
        // Налаштування кольору Gizmos
        Gizmos.color = Color.blue;

        // Малюємо напрямок потоку як лінію від позиції об'єкта
        Gizmos.DrawLine(transform.position, transform.position + flowDirection.normalized * maxStrength);

        // Малюємо точку, куди направлений потік
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.1f);  // Точка, до якої направлений потік
    }
}
