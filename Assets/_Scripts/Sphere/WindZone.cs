using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float windForce = 20f; // Початкова сила вітру
    public Vector3 windAreaSize = new Vector3(2f, 2f, 10f); // Розміри початкового потоку
    public LayerMask obstacleLayers; // Шари, які визначають перешкоди
    public LayerMask windZoneLayers;
    public float minSegmentLength = 0.5f; // Мінімальна довжина сегмента, при якій потік припиняється
    public bool showWindVisual = true; // Нове поле для включення/виключення візуалізації
    public bool bounveActive = true;

    [SerializeField] private Collider[] windColliders;
    private float windUpdateTimer = 0f; // Лічильник часу
    public float updateInterval = 1f; // Інтервал між оновленнями (в секундах)

    
    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position; // Початкова точка потоку
        Vector3 currentDirection = transform.forward; // Початковий напрямок потоку
        float remainingLength = windAreaSize.z; // Поточна довжина потоку
                                                // Оновлюємо колайдери кожні 0.5 секунди
                                                // Оновлюємо колайдери кожні 0.5 секунди
                                                // Додаємо час між кадрами до таймера
        windUpdateTimer += Time.fixedDeltaTime;

        // Якщо таймер перевищує інтервал оновлення
        if (windUpdateTimer >= updateInterval)
        {
            // Запускаємо метод для створення колайдерів
            GenerateWindColliders();

            // Скидаємо таймер
            windUpdateTimer = 0f;
        }
        while (remainingLength > minSegmentLength)
        {
            // Обчислюємо силу вітру для поточного сегмента в залежності від відстані
            float appliedWindForce = GetWindForceAtDistance(windAreaSize.z - remainingLength);

            // Перевіряємо потік для поточного сегмента
            bool isBlocked = ApplyWindInSegment(currentPosition, currentDirection, remainingLength, appliedWindForce, out Vector3 hitPoint, out Vector3 hitNormal, out float distanceToHit);

            if (isBlocked)
            {
                // Обрізаємо потік до точки зіткнення
                remainingLength = distanceToHit;

                // Якщо залишкової довжини немає, зупиняємо потік
                if (remainingLength <= minSegmentLength)
                    break;

                // Розраховуємо новий напрямок після відбиття
                currentDirection = Vector3.Reflect(currentDirection, hitNormal).normalized;

                // Новий початок сегмента — точка зіткнення
                currentPosition = hitPoint + currentDirection * 0.01f; // Трохи зміщуємо, щоб уникнути повторного зіткнення
            }
            else
            {
                // Якщо перешкоди немає, застосовуємо весь залишковий потік і завершуємо
                ApplyWindInSegment(currentPosition, currentDirection, remainingLength, appliedWindForce, out _, out _, out _);
                break;
            }
        }
    }

    private float GetWindForceAtDistance(float distance)
    {
        // Застосовуємо формулу для зменшення сили вітру на основі відстані
        return windForce * Mathf.Exp((1f / 3f) * (-Mathf.Sqrt(distance)));
        
    }

    private bool ApplyWindInSegment(Vector3 startPosition, Vector3 direction, float segmentLength, float windForce, out Vector3 hitPoint, out Vector3 hitNormal, out float distanceToHit)
    {
        hitPoint = Vector3.zero;
        hitNormal = Vector3.zero;
        distanceToHit = segmentLength;

        // Перевіряємо, чи є перешкода перед сегментом
        RaycastHit hit;
        if (Physics.Raycast(startPosition, direction, out hit, segmentLength, obstacleLayers))
        {
            hitPoint = hit.point;
            hitNormal = hit.normal;
            distanceToHit = hit.distance;
        }

        // Розраховуємо центр і розмір поточного сегмента
        Vector3 center = startPosition + direction * (distanceToHit / 2);
        Vector3 size = new Vector3(windAreaSize.x, windAreaSize.y, distanceToHit);

        // Виявляємо об'єкти всередині сегмента і застосовуємо силу
        Collider[] hitColliders = Physics.OverlapBox(center, size / 2, Quaternion.LookRotation(direction));

        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Розраховуємо відстань від початку сегмента до об'єкта
                Vector3 closestPoint = hitCollider.ClosestPoint(startPosition);
                float distanceFromStart = Vector3.Distance(startPosition, closestPoint);

                // Обчислюємо силу поштовху на основі відстані
                float adjustedForce = windForce * Mathf.Exp((1f / 3f) * (-Mathf.Sqrt(distanceFromStart)));

                // Застосовуємо силу до об'єкта
                rb.AddForce(direction * adjustedForce, ForceMode.Force);
            }
        }

        return distanceToHit < segmentLength; // Якщо сегмент обрізаний, повертаємо true
    }

    /*private void OnDrawGizmos()
    {
        // Малюємо тільки якщо змінна showWindVisual дорівнює true
        if (!showWindVisual) return;

        Gizmos.color = Color.cyan;
        Vector3 currentPosition = transform.position;
        Vector3 currentDirection = transform.forward;
        float remainingLength = windAreaSize.z;
        float currentSegmentLength = remainingLength; // Додаємо змінну для поточного сегмента

        while (remainingLength > minSegmentLength)
        {
            // Обчислюємо силу вітру для поточного сегмента в залежності від відстані
            float appliedWindForce = GetWindForceAtDistance(windAreaSize.z - remainingLength);

            // Малюємо поточний сегмент
            Vector3 center = currentPosition + currentDirection * (currentSegmentLength / 2);
            Vector3 size = new Vector3(windAreaSize.x, windAreaSize.y, currentSegmentLength);
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(currentDirection), Vector3.one);

            // Перевіряємо, чи є перешкода перед сегментом
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, currentDirection, out hit, remainingLength, obstacleLayers))
            {
                // Якщо є перешкода, малюємо лінію до неї
                //Gizmos.color = Color.red;
                //Gizmos.DrawLine(currentPosition, hit.point);

                // Зберігаємо довжину поточного сегмента до перешкоди
                currentSegmentLength = hit.distance;

                // Обрізаємо потік
                remainingLength -= hit.distance;
                size.z -= remainingLength;
                center = currentPosition + currentDirection * (currentSegmentLength / 2);
                Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(currentDirection), Vector3.one);

                // Розраховуємо новий напрямок
                if (bounveActive)
                {
                    currentDirection = Vector3.Reflect(currentDirection, hit.normal).normalized;
                    currentPosition = hit.point + currentDirection * 0.01f;
                }
                else
                {
                    Gizmos.DrawWireCube(Vector3.zero, size);
                    break;
                }
                Gizmos.color = Color.cyan;

                // Оновлюємо довжину наступного сегмента
                currentSegmentLength = remainingLength;

                // Якщо залишкова довжина <= мінімальної, зупиняємо малювання
                if (remainingLength <= minSegmentLength)
                    break;

                Gizmos.DrawWireCube(Vector3.zero, size);
            }
            else
            {
                Gizmos.DrawWireCube(Vector3.zero, size);
                // Якщо перешкоди немає, завершуємо малювання
                currentSegmentLength = remainingLength;
                break;
            }
        }
    }*/

    private void GenerateWindColliders()
    {
        // Очищаємо старі колайдери
        foreach (Transform child in transform)
        {
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
        }

        // Додаємо нові колайдери
        Vector3 currentPosition = transform.position;
        Vector3 currentDirection = transform.forward;
        float remainingLength = windAreaSize.z;

        while (remainingLength > minSegmentLength)
        {
            GameObject windSegment = new GameObject("WindSegment");

            windSegment.layer = LayerMask.NameToLayer("WindZone");

            windSegment.transform.SetParent(transform);

            Vector3 center = currentPosition + currentDirection * (remainingLength / 2);
            Vector3 size = new Vector3(windAreaSize.x, windAreaSize.y, remainingLength);

            windSegment.transform.position = center;
            windSegment.transform.rotation = Quaternion.LookRotation(currentDirection);

            BoxCollider boxCollider = windSegment.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = size;

            if (Physics.Raycast(currentPosition, currentDirection, out RaycastHit hit, remainingLength, obstacleLayers))
            {
                size = new Vector3(windAreaSize.x, windAreaSize.y, (remainingLength - (remainingLength - hit.distance)));
                center = currentPosition + currentDirection * ((remainingLength - (remainingLength - hit.distance)) / 2);
                remainingLength -= hit.distance;
                windSegment.transform.position = center;

                boxCollider.size = size;

                if (remainingLength <= minSegmentLength) break;

                if (bounveActive)
                {
                    currentDirection = Vector3.Reflect(currentDirection, hit.normal).normalized;
                    currentPosition = hit.point + currentDirection * 0.01f;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

}
