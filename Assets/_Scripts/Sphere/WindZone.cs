using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float windForce = 20f; // Початкова сила вітру
    public Vector3 windAreaSize = new Vector3(2f, 2f, 10f); // Розміри початкового потоку
    public LayerMask obstacleLayers; // Шари, які визначають перешкоди
    public float minSegmentLength = 0.5f; // Мінімальна довжина сегмента
    public bool showWindVisual = true; // Візуалізація вітру
                                       // public KeyCode toggleVisualKey = KeyCode.V; // Кнопка активації/деактивації візуалізації
    public bool showGizmos = false;
    private LineRenderer _lineRenderer; // Для малювання траєкторії вітру
    private List<Vector3> _windPoints = new List<Vector3>(); // Точки траєкторії вітру

    public bool bounveActive = true;
    private void Start()
    {
        // Ініціалізація LineRenderer
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 0;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.blue;
        _lineRenderer.endColor = Color.cyan;
    }

    private void FixedUpdate()
    {
        // Активація/деактивація візуалізації
        showWindVisual = PlayerPrefs.GetInt("WindZoneShow", 0) == 1;
        _lineRenderer.enabled = showWindVisual;

        Vector3 currentPosition = transform.position; // Початкова точка потоку
        Vector3 currentDirection = transform.forward; // Початковий напрямок потоку
        float remainingLength = windAreaSize.z; // Поточна довжина потоку

        _windPoints.Clear(); // Очищуємо попередні точки
        _windPoints.Add(currentPosition); // Додаємо стартову точку

        while (remainingLength > minSegmentLength)
        {
            // Обчислюємо силу вітру для поточного сегмента в залежності від відстані
            float appliedWindForce = GetWindForceAtDistance(windAreaSize.z - remainingLength);

            // Перевіряємо потік для поточного сегмента
            bool isBlocked = ApplyWindInSegment(currentPosition, currentDirection, remainingLength, appliedWindForce, out Vector3 hitPoint, out Vector3 hitNormal, out float distanceToHit);

            if (isBlocked)
            {
                // Обрізаємо потік до точки зіткнення
                remainingLength = remainingLength - distanceToHit;

                // Якщо залишкової довжини немає, зупиняємо потік
                if (remainingLength <= minSegmentLength)
                    break;

                // Розраховуємо новий напрямок після відбиття
                currentDirection = Vector3.Reflect(currentDirection, hitNormal).normalized;

                // Новий початок сегмента — точка зіткнення
                currentPosition = hitPoint + currentDirection * 0.01f; // Трохи зміщуємо, щоб уникнути повторного зіткнення
                _windPoints.Add(currentPosition); // Додаємо точку
            }
            else
            {
                // Якщо перешкоди немає, додаємо останню точку
                currentPosition += currentDirection * remainingLength;
                _windPoints.Add(currentPosition);
                break;
            }
        }
    }

    private void Update()
    {
        // Оновлюємо візуалізацію
        if (showWindVisual)
        {
            UpdateWindVisual();
        }
    }
    private void UpdateWindVisual()
    {
        // Оновлюємо LineRenderer
        _lineRenderer.positionCount = _windPoints.Count;
        _lineRenderer.SetPositions(_windPoints.ToArray());
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

  
    private void OnDrawGizmos()
    {
        // Малюємо тільки якщо змінна showWindVisual дорівнює true
        if (!showGizmos) return;

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
                currentSegmentLength = hit.distance;
                remainingLength -= hit.distance;
                size.z -= remainingLength;
                center = currentPosition + currentDirection * (currentSegmentLength / 2);
                Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(currentDirection), Vector3.one);

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

                currentSegmentLength = remainingLength;

                if (remainingLength <= minSegmentLength)
                    break;

                Gizmos.DrawWireCube(Vector3.zero, size);
            }
            else
            {
                Gizmos.DrawWireCube(Vector3.zero, size);
                currentSegmentLength = remainingLength;
                break;
            }
        }
    }

}
