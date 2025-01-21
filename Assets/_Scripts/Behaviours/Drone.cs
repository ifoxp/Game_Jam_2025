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
            Vector3 distance = targetMeteorite.transform.position + transform.position;
            Vector3 absoluteDistance = new Vector3(Mathf.Abs(distance.x), Mathf.Abs(distance.y), Mathf.Abs(distance.z));
            float distanceToHub = Vector3.Distance(absoluteDistance, hub.position);

            
            print(distanceToHub);
            
            rb.linearVelocity = transform.forward * speed;
            
            Vector3 targetDirection = (targetMeteorite.transform.position - transform.position).normalized;
            Vector3 currentDirection = transform.forward;

            Vector3 rotationAxis = Vector3.Cross(currentDirection, targetDirection);
            float angleDifference = Vector3.Angle(currentDirection, targetDirection);
            
            rb.angularVelocity = rotationAxis.normalized * (angleDifference * Mathf.Deg2Rad * rotationSpeed * distanceToHub);
            
            if (Vector3.Distance(transform.position, targetMeteorite.transform.position) < 0.4f)
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