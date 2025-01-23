using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BuildContent : MonoBehaviour
{
    public float size;
    public bool isBuild = true;
    public BoxCollider box;
    public Collider currentCollider = null;
    private Rigidbody rb;
    public BoxCollider[] sizeBuild;
    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other != currentCollider)
        {
            currentCollider = other;
            isBuild = false; // Об'єкт у зоні тригера
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other == currentCollider)
        {
            currentCollider = null;
            isBuild = true; // Об'єкт вийшов із зони тригера
        }
    }

    private void FixedUpdate()
    {
        // Маска для перевірки тригерів з двох шарів (LayerMask за потреби можна змінити)
        int layerMask = LayerMask.GetMask("Drone", "Builder"); // Об'єднуємо маски для обох шарів

        // Отримуємо фактичні розміри BoxCollider
        Vector3 size = box.size;  // Реальний розмір BoxCollider (не bounds.extents)
        Vector3 halfExtents = size / 2f;  // Піврозміри для використання в OverlapBox

        // Збільшуємо точність перевірок для маленьких колайдерів
        // Можна збільшити перевірку кілька разів, щоб не втратити перетин
        float overlapScaleFactor = Mathf.Max(size.x, size.y, size.z) * 10f; // Множимо розміри для збільшення точності

        // Перевірка на перетин тригерів з фактичними розмірами та орієнтацією колайдера
        Collider[] overlappingColliders = Physics.OverlapBox(box.transform.position, halfExtents * overlapScaleFactor, transform.rotation, layerMask);

        bool isAnyTriggerInside = false;

        // Перевірка перетину для кожного колайдера, ігноруючи сам об'єкт і його дочірні елементи
        foreach (var collider in overlappingColliders)
        {
            // Ігноруємо колайдер цього об'єкта та його дочірні елементи
            if (collider.transform != transform && !collider.transform.IsChildOf(transform))
            {
                // Використовуємо Intersects для перевірки точного перетину
                if (box.bounds.Intersects(collider.bounds))
                {
                    isAnyTriggerInside = true;
                    currentCollider = collider;  // Оновлюємо currentCollider
                    break; // Якщо хоча б один тригер знайдений, зупиняємо перевірку
                }
            }
        }

        // Якщо знайдений хоча б один тригер, об'єкт у зоні тригера
        if (isAnyTriggerInside)
        {
            isBuild = false; // Об'єкт у зоні тригера
        }
        else
        {
            currentCollider = null;
            isBuild = true; // Об'єкт вийшов із зони тригера
        }
    }



    private void Reset()
    {
        // Ініціалізація компонентів під час скидання
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (box != null)
        {
            box.isTrigger = true;
        }

        gameObject.layer = LayerMask.NameToLayer("Drone");
    }
}
