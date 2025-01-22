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
        [SerializeField] private Transform hubRoad;
        [SerializeField] private Transform hook;

        [HorizontalLine]
        [SerializeField] private bool isMovingToHub;
        [SerializeField] private bool isMovingToHubRoad;

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

            if (isMovingToHubRoad)
                MoveToHubRoad();

            if (isMovingToHub)
                MoveToHub();
        }

        private void MoveToHubRoad()
        {
            MoveTo(hubRoad, out _);

            if (Vector3.Distance(transform.position, hubRoad.position) < 0.4f)
            {
                rb.velocity = Vector3.zero;
                isMovingToHubRoad = false;
                isMovingToHub = true;
            }
        }

        private void MoveToHub()
        {
            MoveTo(hub, out _);

            if (Vector3.Distance(transform.position, hub.position) < 0.4f)
            {
                rb.velocity = Vector3.zero;

                // Завершення логіки повернення до хабу
                isMovingToHub = false;
                isPickingUpMeteorite = false;
                targetMeteorite.gameObject.SetActive(false);
                targetMeteorite = null;
            }
        }

        private void MoveToMeteorite()
        {
            MoveTo(targetMeteorite.transform, out float distanceToMeteorite);

            if (distanceToMeteorite < 0.4f)
            {
                isPickingUpMeteorite = true;
                isMovingToHubRoad = true;

                Destroy(targetMeteorite.transform.GetComponent<Rigidbody>());
                targetMeteorite.transform.SetParent(transform);
                targetMeteorite.transform.localPosition = new Vector3(0, 0, 1);
            }
        }

        private void MoveTo(Transform target, out float distanceToTarget)
        {
            // Визначаємо відстань
            float distance = Vector3.Distance(transform.position, target.position);
            distanceToTarget = distance;

            // Лінійна швидкість
            rb.velocity = transform.forward * speed;

            // Напрямок до цілі
            Vector3 targetDirection = (target.position - transform.position).normalized;
            Vector3 currentDirection = transform.forward;

            // Визначаємо вісь обертання
            Vector3 rotationAxis = Vector3.Cross(currentDirection, targetDirection);
            float angleDifference = Vector3.Angle(currentDirection, targetDirection);

            // Модифікація швидкості обертання в залежності від відстані
            float adjustedRotationSpeed = rotationSpeed * Mathf.Clamp01(1 / (distance + 1f));

            // Якщо відстань дуже мала, використовуємо RotateTowards
            if (distance < 0.4f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), adjustedRotationSpeed * Time.deltaTime);
            }
            else
            {
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
