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
            MoveTo(hub, out _);
            
            if(targetMeteorite)
                targetMeteorite.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.3f, transform.localPosition.z);

            if (Vector3.Distance(transform.position, hub.position) < 0.4f)
            {
                rb.linearVelocity = Vector3.zero;
                targetMeteorite = null;
                
                // TODO: LOGIC FOR RESOURCES IS HERE ->
                
                isMovingToHub = false;
            }
        }

        private void MoveToMeteorite()
        {
            MoveTo(targetMeteorite.transform, out float distanceToMeteorite);

            if (distanceToMeteorite < 0.4f)
            {
                isPickingUpMeteorite = true;
                isMovingToHub = true;

                Destroy(targetMeteorite.transform.GetComponent<Rigidbody>());
            }
        }
        

        private void MoveTo(Transform target, out float distanceToTarget)
        {
            // Визначаємо відстань
            float distance = Vector3.Distance(transform.position, target.position);
            
            distanceToTarget = distance;

            // Лінійна швидкість
            rb.linearVelocity = transform.forward * speed;

            // Напрямок до метеориту
            Vector3 targetDirection = (target.position - transform.position).normalized;
            Vector3 currentDirection = transform.forward;

            // Визначаємо вісь обертання
            Vector3 rotationAxis = Vector3.Cross(currentDirection, targetDirection);
            float angleDifference = Vector3.Angle(currentDirection, targetDirection);

            // Модифікація швидкості обертання в залежності від відстані до метеориту
            float adjustedRotationSpeed;

            if (distance < 7f)
            {
                // Якщо відстань менша ніж 3, збільшуємо швидкість повороту
                adjustedRotationSpeed = rotationSpeed * Mathf.Clamp01(1 / (distance + 1f)) * 2.5f; // Додаємо коефіцієнт, щоб збільшити швидкість повороту
            }
            else
            {
                // Інакше використовуємо стандартну швидкість
                adjustedRotationSpeed = rotationSpeed * Mathf.Clamp01(1 / (distance + 1f));
            }

            // Якщо відстань дуже мала, ми використовуємо RotateTowards для точного наведення
            if (distance < 0.4f)
            {
                // Обертання з максимальною швидкістю, щоб об'єкт точно орієнтувався на метеорит
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), adjustedRotationSpeed * Time.deltaTime);
            }
            else
            {
                // Якщо відстань більша, об'єкт може повертатись з врахуванням кутової швидкості
                rb.angularVelocity = rotationAxis.normalized * (angleDifference * Mathf.Deg2Rad * adjustedRotationSpeed);
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