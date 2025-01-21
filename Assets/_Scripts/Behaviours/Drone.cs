using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers;
using NaughtyAttributes;
using UnityEngine;

namespace _Scripts.Behaviours
{
    public class Drone : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private Transform hub;
        [SerializeField] private Transform hook;
        
        [HorizontalLine]
        [SerializeField] private bool isMovingToHub;
        // [SerializeField] private bool isMovingToMeteorite;
        
        [HorizontalLine]
        [SerializeField] private Meteorite targetMeteorite;
        [SerializeField] private bool isPickingUpMeteorite;
        
        private Rigidbody rb;

        private void Awake() => rb = GetComponent<Rigidbody>();
        private void Start()
        {
            transform.position = hub.position;
            MeteoritesManager.One.OnMeteoriteSpawned += FindNearestMeteorite;
        }

        private void Update()
        {
            if (targetMeteorite && !isPickingUpMeteorite)
                MoveToMeteorite();
            if (isMovingToHub)
                MoveToHub();
        }
        
        private void MoveToHub()
        {
            Vector3 velocity = (hub.position - transform.position).normalized * speed;
            rb.linearVelocity = velocity;

            if (Vector3.Distance(transform.position, hub.position) < 0.4f)
            {
                rb.linearVelocity = Vector3.zero;
                targetMeteorite = null;
                
                // GET RESOURCES
                
                isMovingToHub = false;
            }
        }

        private void MoveToMeteorite()
        {
            // Визначаємо відстань до метеориту
            float distanceToMeteorite = Vector3.Distance(transform.position, targetMeteorite.transform.position);

            // Друкуємо відстань для перевірки
            print(distanceToMeteorite);

            // Лінійна швидкість
            rb.linearVelocity = transform.forward * speed;

            // Напрямок до метеориту
            Vector3 targetDirection = (targetMeteorite.transform.position - transform.position).normalized;
            Vector3 currentDirection = transform.forward;

            // Визначаємо вісь обертання
            Vector3 rotationAxis = Vector3.Cross(currentDirection, targetDirection);
            float angleDifference = Vector3.Angle(currentDirection, targetDirection);

            // Модифікація швидкості обертання в залежності від відстані до метеориту
            float adjustedRotationSpeed;

            if (distanceToMeteorite < 7f)
            {
                // Якщо відстань менша ніж 3, збільшуємо швидкість повороту
                adjustedRotationSpeed = rotationSpeed * Mathf.Clamp01(1 / (distanceToMeteorite + 1f)) * 2.5f; // Додаємо коефіцієнт, щоб збільшити швидкість повороту
            }
            else
            {
                // Інакше використовуємо стандартну швидкість
                adjustedRotationSpeed = rotationSpeed * Mathf.Clamp01(1 / (distanceToMeteorite + 1f));
            }

            // Якщо відстань дуже мала, ми використовуємо RotateTowards для точного наведення
            if (distanceToMeteorite < 0.4f)
            {
                // Обертання з максимальною швидкістю, щоб об'єкт точно орієнтувався на метеорит
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), adjustedRotationSpeed * Time.deltaTime);
            }
            else
            {
                // Якщо відстань більша, об'єкт може повертатись з врахуванням кутової швидкості
                rb.angularVelocity = rotationAxis.normalized * (angleDifference * Mathf.Deg2Rad * adjustedRotationSpeed);
            }

            // Якщо відстань до метеориту менша за 0.4, об'єкт починає рухатись до хабу
            if (distanceToMeteorite < 0.4f)
            {
                isPickingUpMeteorite = true;
                isMovingToHub = true;
                targetMeteorite.transform.SetParent(hook);
                targetMeteorite.transform.localPosition = Vector3.zero;
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