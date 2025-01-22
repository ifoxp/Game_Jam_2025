using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Entities;
using _Scripts.Managers;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Behaviours
{
    public class Drone : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private Transform hub;
        [SerializeField] private Transform hubRoad;
        [SerializeField] private Transform hook;
        [SerializeField] private MeteoritesManager meteoritesManager; // Метеорити, які створюються
        [HorizontalLine]
        [SerializeField] private bool isMovingToHub;

        [HorizontalLine]
        [SerializeField] private Meteorite targetMeteorite;
        [SerializeField] private bool isPickingUpMeteorite;

        private Rigidbody rb;

        private void Awake() => rb = GetComponent<Rigidbody>();

        private void Start()
        {
            transform.position = hub.position;
            MeteoritesManager.One.OnMeteoriteSpawned += FindNearestMeteorite;

            // Запускаємо перевірку на наявність метеоритів у радіусі
            StartCoroutine(CheckForNearbyMeteorites());
        }

       


        private void FixedUpdate()
        {
            if (targetMeteorite != null && !isPickingUpMeteorite)
                MoveToMeteorite();

            else if (!isPickingUpMeteorite && targetMeteorite == null )
                if(Vector3.Distance(transform.position, hub.position) > 1.4f)
                    isMovingToHub = true;

            else if(Vector3.Distance(transform.position, hub.position) > 2f && targetMeteorite == null)
                    isMovingToHub = true;


            if (isMovingToHub)
                MoveToHub();
        }

        private void MoveToHub()
        {
            MoveTo(hub, out _);

            if (Vector3.Distance(transform.position, hub.position) < 1.4f)
            {
                // Зупиняємо рух
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero; // Зупиняємо обертання

                // Телепортуємо об'єкт на точку hub
                transform.position = hub.position;

                // Повертаємо об'єкт в сторону hubRoad
                Vector3 directionToHubRoad = (hubRoad.position - hub.position).normalized;
                transform.rotation = Quaternion.LookRotation(directionToHubRoad);

                // Завершення логіки повернення до хабу
                isMovingToHub = false;
                isPickingUpMeteorite = false;
                targetMeteorite = null;
                StartCoroutine(CheckForNearbyMeteorites());
            }


        }

        private void MoveToMeteorite()
        {
            MoveTo(targetMeteorite.transform, out float distanceToMeteorite);
        }

        private void MoveTo(Transform target, out float distanceToTarget)
        {
            // Визначаємо відстань
            float distance = Vector3.Distance(transform.position, target.position);
            distanceToTarget = distance;

            // Центр уявного кола
            Vector3 forbiddenZoneCenter = Vector3.zero;
            float forbiddenZoneRadius = 29f;

            // Напрямок до цілі
            Vector3 targetDirection = (target.position - transform.position).normalized;
            Vector3 currentDirection = transform.forward;

            // Відстань до центру забороненого кола
            float distanceToForbiddenCenter = Vector3.Distance(transform.position, forbiddenZoneCenter);

            // Перевіряємо, чи дрон перетинає заборонене коло
            if (distanceToForbiddenCenter < forbiddenZoneRadius)
            {
                // Коригуємо напрямок: знаходимо точку на межі кола, яка є найближчою до дрона
                Vector3 directionFromCenter = (transform.position - forbiddenZoneCenter).normalized;
                Vector3 avoidancePoint = forbiddenZoneCenter + directionFromCenter * forbiddenZoneRadius;

                // Обчислюємо новий напрямок до цілі, уникаючи забороненої зони
                targetDirection = (avoidancePoint - transform.position).normalized;
            }

            // Лінійна швидкість
            rb.linearVelocity = transform.forward * speed;

            // Визначаємо вісь обертання
            Vector3 rotationAxis = Vector3.Cross(currentDirection, targetDirection);
            float angleDifference = Vector3.Angle(currentDirection, targetDirection);

            // Якщо відстань дуже мала, використовуємо RotateTowards
            if (distance < 0.4f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Поступовий збільшений поворот
                float dynamicRotationSpeed = rotationSpeed * Mathf.Lerp(1f, 2f, Mathf.Clamp01(angleDifference / 180f));
                rb.angularVelocity = rotationAxis.normalized * (angleDifference * Mathf.Deg2Rad * dynamicRotationSpeed);
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            // Перевіряємо, чи зіткнулися з метеоритом
            if (collision.gameObject.CompareTag("Meteorite") && !isPickingUpMeteorite)
            {

                isPickingUpMeteorite = true;
                isMovingToHub = true;
                // Випадкове створення нового метеорита
                int randomIndex = UnityEngine.Random.Range(0, meteoritesManager.meteoritePrefs.Length);
                GameObject newMeteorite = Instantiate(meteoritesManager.meteoritePrefs[randomIndex], transform.position, Quaternion.identity,gameObject.transform);
                newMeteorite.GetComponent<FlyingMeteorite>().enabled = false;

                Vector3 newPosition = transform.position;
                newPosition.z -=1.1f;
                newMeteorite.transform.position = newPosition;
                
                newMeteorite.GetComponent<Collider>().isTrigger= true;
                Destroy(newMeteorite.transform.GetComponent<Rigidbody>());
                newMeteorite.transform.localScale=new Vector3(0.3f,0.3f,0.3f);
                newMeteorite.SetActive(true); // Метеорит буде активним
                targetMeteorite = null;
            }
        }
        private IEnumerator CheckForNearbyMeteorites()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f); // Перевіряємо раз на секунду

                if (targetMeteorite == null && !isPickingUpMeteorite && !isMovingToHub)
                {
                    Meteorite[] meteorites = FindObjectsByType<Meteorite>(FindObjectsSortMode.None);

                    // Фільтруємо метеорити в радіусі від 75 до 125
                    List<Meteorite> validMeteorites = new List<Meteorite>();

                    foreach (Meteorite meteorite in meteorites)
                    {
                        float distance = Vector3.Distance(transform.position, meteorite.transform.position);

                        if (distance >= 75f && distance <= 125f)
                        {
                            validMeteorites.Add(meteorite);
                        }
                    }

                    // Вибираємо випадковий метеорит із доступних
                    if (validMeteorites.Count > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, validMeteorites.Count);
                        Meteorite selectedMeteorite = validMeteorites[randomIndex];

                        selectedMeteorite.gameObject.GetComponent<Meteorite>().enabled = false;
                        targetMeteorite = selectedMeteorite;

                        if (!targetMeteorite.targetedDrone)
                        {
                            targetMeteorite.targetedDrone = this;
                        }
                    }
                }
            }
        }

        [Button]
        public void FindNearestMeteorite()
        {
            Meteorite[] meteorites = FindObjectsByType<Meteorite>(FindObjectsSortMode.None);
            Meteorite nearestMeteorite = null;
            float nearestDistance = float.MaxValue;

            foreach (Meteorite meteorite in meteorites)
            {
                float distance = Vector3.Distance(transform.position, meteorite.transform.position);
                if (distance < nearestDistance)
                {
                    nearestMeteorite = meteorite;
                    nearestDistance = distance;
                }
            }

            targetMeteorite = nearestMeteorite;

            if (targetMeteorite && !targetMeteorite.targetedDrone)
                targetMeteorite.targetedDrone = this;
        }
    }

}
