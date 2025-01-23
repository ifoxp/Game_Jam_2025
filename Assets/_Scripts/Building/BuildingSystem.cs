using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static Zenject.CheatSheet;

public class BuildingSystem : MonoBehaviour
{
    public GameObject beamPrefab; // Префаб балки
    private GameObject previewBeam; // Балка в режимі попереднього перегляду
    private bool isPlacing = false; // Режим будування
    public LayerMask buildingLayer; // Шар для об'єктів із тегом "Building"
    private Vector3 lastValidPosition; // Остання валідна позиція фантомного об'єкта
    private Quaternion lastValidRotation; // Останній валідний поворот фантомного об'єкта
    private bool hasValidPosition = false; // Чи є дійсна позиція для фантомного об'єкта
    public float size;
    BoxCollider boxCollider;
    BoxCollider finish;
    private bool isBuild=true, isTriggers=true;

    public float overlapThreshold = 0.1f;

    // Список для зберігання всіх колайдерів "Finish"
    public List<BoxCollider> finishColliders = new List<BoxCollider>();
    private int currentFinishIndex = 0; // Індекс активного колайдера "Finish"

    private Vector3 positionOffset; // Зміщення для розміщення об'єкта
    private Quaternion rotationnOffset;
    void Update()
    {
        
        // Включення режиму будування на клавішу "Q"
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlacing)
            {
                CancelPlacing();
            }
            else
            {
                StartPlacing();
            }
        }

        // Перемикання між колайдерами "Finish" за допомогою "R"
        if (Input.GetKeyDown(KeyCode.R) && finishColliders.Count > 0)
        {
            Debug.Log("R");
            MoveToFinishCollider();
        }

        if (isPlacing)
        {
            HandleBeamPlacement();
        }
    }
    private void FixedUpdate()
    {
        if (previewBeam != null)
        {
            bool tar = previewBeam.gameObject.GetComponent<BuildContent>().isBuild;
            if (tar)
            {
                isTriggers = true;
            }
            else
            {
                isTriggers = false;
                Renderer[] renderers = previewBeam.GetComponentsInChildren<Renderer>();
                renderers = previewBeam.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.material.color = new Color(1, 0, 0, 0.5f);
                }
            }
        }
    }

    void StartPlacing()
    {
        // Створення об'єкта для попереднього перегляду
        isPlacing = true;
        previewBeam = Instantiate(beamPrefab);
        size = previewBeam.GetComponent<BuildContent>().size;

        // Збираємо всі колайдери з тегом "Finish"
        finishColliders.Clear();
        BoxCollider[] childColliders = previewBeam.GetComponentsInChildren<BoxCollider>();
        foreach (var collider in childColliders)
        {
            if (collider.CompareTag("Finish") && collider.gameObject.layer == LayerMask.NameToLayer("Build"))
            {
                finishColliders.Add(collider);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Build"))
            {
                collider.enabled = false;
            }
        }

        // Вимикаємо всі колайдери, крім першого
        if (finishColliders.Count > 0)
        {
            finishColliders[0].enabled = true; // Увімкнути перший колайдер з "Finish"
            finish = finishColliders[0]; // Встановлюємо перший колайдер як поточний
            positionOffset = finish.transform.position - previewBeam.transform.position; // Обчислюємо зміщення
        }
        positionOffset = finishColliders[currentFinishIndex].transform.position - finishColliders[0].transform.position; // Оновлюємо зміщення
        // Змінюємо колір для візуалізації
        Renderer[] renderers = previewBeam.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material.color = new Color(0, 1, 0, 0.5f);
        }
    }


    void MoveToFinishCollider()
    {
        // Перевіряємо, чи є поточний фініш
        if (currentFinishIndex < 0 || currentFinishIndex >= finishColliders.Count)
        {
            Debug.LogWarning("Індекс фінішного колайдера виходить за межі списку!");
            return;
        }

        finish = finishColliders[currentFinishIndex];

        // Встановлюємо позицію об'єкта відповідно до фінішного колайдера
        previewBeam.transform.position = finish.transform.position;

        // Встановлюємо тільки поворот об'єкта відповідно до колайдера
        Quaternion targetRotation = finish.transform.rotation;
        previewBeam.transform.rotation = Quaternion.Euler(
            Mathf.Round(targetRotation.eulerAngles.x / 90) * 90,
            Mathf.Round(targetRotation.eulerAngles.y / 90) * 90,
            Mathf.Round(targetRotation.eulerAngles.z / 90) * 90
        );

        // Оновлюємо зміщення повороту
        rotationnOffset = targetRotation;

        // Додатковий лог для відстеження
        Debug.Log($"Об'єкт переміщено до фінішного колайдера: {finish.name}, Позиція: {finish.transform.position}, Поворот: {targetRotation.eulerAngles}");
    }


    void CancelPlacing()
    {
        // Скасування будування
        isPlacing = false;
        if (previewBeam != null)
        {
            Destroy(previewBeam);
        }
    }

    void HandleBeamPlacement()
    {
        // Отримуємо позицію миші
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildingLayer))
        {
            // Перевіряємо, чи об'єкт на який наводимося, належить до потрібного шару
            if (((1 << hit.collider.gameObject.layer) & buildingLayer) != 0)
            {
                boxCollider = hit.collider.gameObject.GetComponent<BoxCollider>();
                // Отримуємо колайдер з тегом "Finish" на об'єкті попереднього перегляду
                Collider finishCollider = GetFinishCollider(previewBeam);
                if (finishCollider != null && !hit.collider.CompareTag("Finish"))
                {
                    Renderer[] renderers = previewBeam.GetComponentsInChildren<Renderer>();
                    if (isBuild && isTriggers)
                    {
                        foreach (var renderer in renderers)
                        {
                            renderer.material.color = new Color(0, 1, 0, 0.5f);
                        }
                    }
                    // Обчислюємо нову позицію для об'єкта попереднього перегляду
                    Vector3 targetPosition = hit.collider.bounds.center;

                    // Додаємо відступ залежно від напрямку повороту
                    Vector3 forwardOffset = lastValidRotation * Vector3.forward * size;
                    targetPosition += forwardOffset;
                    targetPosition -= positionOffset;

                        // Оновлюємо позицію та поворот фантомного об'єкта
                        lastValidPosition = targetPosition;
                    // Додаємо зміщення до останнього валідного повороту
                    lastValidRotation = lastValidRotation * rotationnOffset;

                    // Оновлюємо останній валідний поворот на основі напрямку хіта
                    lastValidRotation = Quaternion.LookRotation(hit.normal);

                    hasValidPosition = true;
                    
                        // Переміщаємо об'єкт попереднього перегляду
                        previewBeam.transform.position = lastValidPosition;
                        previewBeam.transform.rotation = lastValidRotation;

                    bool tar = previewBeam.gameObject.GetComponent<BuildContent>().isBuild;
                    BoxCollider[] colliders = previewBeam.gameObject.GetComponent<BuildContent>().sizeBuild;
                    
                    if (colliders != null && colliders.Length > 0 && isTriggers)
                    {
                        bool allTouchDrone = true; // Перевірка, чи всі колайдери торкаються тригерів із тегом "Drone"

                        foreach (BoxCollider boxCollider in colliders)
                        {
                            // Отримуємо всі об'єкти, що перетинаються з цим BoxCollider
                            Collider[] overlappingColliders = Physics.OverlapBox(
                                boxCollider.bounds.center,
                                boxCollider.bounds.extents,
                                boxCollider.transform.rotation);

                            // Перевірка, чи поточний колайдер торкається хоча б одного тригера з тегом "Drone"
                            bool currentTouchesDrone = false;
                            foreach (Collider overlap in overlappingColliders)
                            {
                                if ((overlap.gameObject.layer == LayerMask.NameToLayer("Drone")) && overlap.isTrigger)
                                {
                                    
                                    currentTouchesDrone = true;
                                    break; // Колайдер торкається тригера, можна припинити перевірку для цього boxCollider
                                }
                            }
                            
                            // Якщо хоча б один boxCollider не торкається "Drone", встановлюємо allTouchDrone = false
                            if (!currentTouchesDrone)
                            {
                                allTouchDrone = false;
                                break; // Виходимо з циклу, якщо будь-який boxCollider не проходить перевірку
                            }
                        }

                        // Оновлюємо значення isBuild на основі результату перевірки
                        isBuild = allTouchDrone;
                    }
                    else if(colliders == null || colliders.Length == 0)
                    {
                        isBuild = true; // Якщо немає жодного колайдера, isBuild = false
                    }
                    else
                        isBuild = false;

                    // Ставимо об'єкт при натисканні ЛКМ
                    if (Input.GetMouseButtonDown(0) && isBuild && isTriggers)
                    {
                        if(isBuild && isTriggers)
                        PlaceBeam(lastValidPosition, lastValidRotation);
                    }
                    else if(!isBuild || !isTriggers) 
                    {
                        renderers = previewBeam.GetComponentsInChildren<Renderer>();
                        foreach (var renderer in renderers)
                        {
                            renderer.material.color = new Color(1, 0, 0, 0.5f);
                        }
                    }
                }
            }
        }
    }

    // Перевірка на перетин моделі
   
    void PlaceBeam(Vector3 position, Quaternion rotation)
    {
        // Перевірка наявності колайдера перед доступом до нього
        Collider beamCollider = finish;
        if (beamCollider == null)
        {
            Debug.LogError("Collider not found on the beam prefab!");
            return;
        }

        // Тепер використовуємо beamCollider, бо він є
            GameObject placedBeam = Instantiate(beamPrefab, position, rotation);
            // Увімкнення всіх дочірніх колайдерів
            Collider[] childColliders = placedBeam.GetComponentsInChildren<Collider>();
            foreach (var collider in childColliders)
            {
                collider.enabled = true;
                if (collider.CompareTag("Finish"))
                    collider.enabled = false;
            }

            boxCollider.enabled = false;
            isPlacing = false;
        positionOffset = Vector3.zero;
            Destroy(previewBeam);
    }

    Collider GetFinishCollider(GameObject obj)
    {
        // Знаходимо колайдер із тегом "Finish" у дочірніх об'єктах
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Finish"))
            {
                return collider;
            }
        }
        return null;
    }
}
