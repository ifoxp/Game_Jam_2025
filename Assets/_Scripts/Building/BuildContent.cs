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
        if (currentCollider != null)
        {
            // Перевірка на перетин bounds
            if (!box.bounds.Intersects(currentCollider.bounds))
            {
                currentCollider = null;
                isBuild = true; // Об'єкт більше не перетинається
            }
            else
                isBuild = false;
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
