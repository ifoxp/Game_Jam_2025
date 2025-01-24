using _Scripts._BuildingsEarn;
using _Scripts.DataModel;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Scripts.Building
{
    public class BuildingSystem : MonoBehaviour
    {
        public GameObject[] beamPrefab; // ������ �����
        private int WhoBuild;
        private GameObject previewBeam; // ����� � ����� ������������ ���������
        private bool isPlacing = false; // ����� ���������
        public LayerMask buildingLayer; // ��� ��� ��'���� �� ����� "Building"
        private Vector3 lastValidPosition; // ������� ������ ������� ���������� ��'����
        private Quaternion lastValidRotation; // ������� ������� ������� ���������� ��'����
        private bool hasValidPosition = false; // �� � ����� ������� ��� ���������� ��'����
        public float size;
        BoxCollider boxCollider;
        BoxCollider finish;
        private bool isBuild = true, isTriggers = true;
        [Inject] private DiContainer _container;
        public float overlapThreshold = 0.1f;

        // ������ ��� ��������� ��� ��������� "Finish"
        public List<BoxCollider> finishColliders = new List<BoxCollider>();
        private int currentFinishIndex = 0; // ������ ��������� ��������� "Finish"

        private Vector3 positionOffset; // ������� ��� ��������� ��'����
        private Quaternion rotationnOffset;


        private int Junk,JunkBuy;
        private int Materials,MaterialsBuy;
        private int Population, PopulationBuy;
        private GameResourcesInventory _inventory;
        [Inject]
        private void Construct(GameResourcesInventory inventory)
        {
            _inventory = inventory;
        }
        void Update()
        {

            // ��������� ������ ��������� �� ������ "Q"
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isPlacing)
                {
                    CancelPlacing();
                }
                /*else
                {
                    StartPlacing();
                }*/
            }

            // ����������� �� ����������� "Finish" �� ��������� "R"
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
            // ��������� ��'���� ��� ������������ ���������
            isPlacing = true;
            previewBeam = Instantiate(beamPrefab[WhoBuild]);
            size = previewBeam.GetComponent<BuildContent>().size;

            // ������� �� ��������� � ����� "Finish"
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

            // �������� �� ���������, ��� �������
            if (finishColliders.Count > 0)
            {
                finishColliders[0].enabled = true; // �������� ������ �������� � "Finish"
                finish = finishColliders[0]; // ������������ ������ �������� �� ��������
                positionOffset = finish.transform.position - previewBeam.transform.position; // ���������� �������
            }
            positionOffset = finishColliders[currentFinishIndex].transform.position - finishColliders[0].transform.position; // ��������� �������
            // ������� ���� ��� ����������
            Renderer[] renderers = previewBeam.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.color = new Color(0, 1, 0, 0.5f);
            }
        }


        void MoveToFinishCollider()
        {
            // Оновлення кута на 90 градусів
            Vector3 rotationAngles = rotationnOffset.eulerAngles;
            rotationAngles.z = (rotationAngles.z + 90f) % 360f; // Обертання на 90 градусів, враховуючи межі (0-360)
            rotationnOffset.eulerAngles = rotationAngles; // Оновлюємо значення
        }


        void CancelPlacing()
        {
            // ���������� ���������
            isPlacing = false;
            if (previewBeam != null)
            {
                Destroy(previewBeam);
            }
        }

        void HandleBeamPlacement()
        {
            // �������� ������� ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildingLayer))
            {
                // ����������, �� ��'��� �� ���� ����������, �������� �� ��������� ����
                if (((1 << hit.collider.gameObject.layer) & buildingLayer) != 0)
                {
                    boxCollider = hit.collider.gameObject.GetComponent<BoxCollider>();
                    // �������� �������� � ����� "Finish" �� ��'��� ������������ ���������
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
                        // ���������� ���� ������� ��� ��'���� ������������ ���������
                        Vector3 targetPosition = hit.collider.bounds.center;

                        // ������ ������ ������� �� �������� ��������
                        // Додаємо зсув у напрямку вперед
                        Vector3 forwardOffset = lastValidRotation * Vector3.forward * size;

                        // Розрахунок кінцевої позиції
                        targetPosition += forwardOffset;
                        targetPosition -= positionOffset;

                        // Оновлення позиції та обертання
                        lastValidPosition = targetPosition;

                        // Оновлюємо `lastValidRotation` із використанням оновленого кута
                        lastValidRotation = Quaternion.Euler(lastValidRotation.x, lastValidRotation.y, rotationnOffset.eulerAngles.z);

                        // Використання LookRotation для орієнтації, якщо потрібно
                        if (hit.normal != Vector3.zero) // Перевірка, чи нормаль не нульова
                        {
                            // Отримуємо ротацію з нормаллю
                            Quaternion normalRotation = Quaternion.LookRotation(hit.normal);

                            // Поєднуємо обертання з новим кутом `z` з rotationnOffset
                            Vector3 finalEulerAngles = normalRotation.eulerAngles;
                            finalEulerAngles.z = rotationnOffset.eulerAngles.z; // Застосовуємо кут обертання по `z`

                            // Оновлюємо останню дійсну ротацію
                            lastValidRotation = Quaternion.Euler(finalEulerAngles);
                        }

                        // ��������� ��'��� ������������ ���������
                        previewBeam.transform.position = lastValidPosition;
                        previewBeam.transform.rotation = lastValidRotation;

                        bool tar = previewBeam.gameObject.GetComponent<BuildContent>().isBuild;
                        BoxCollider[] colliders = previewBeam.gameObject.GetComponent<BuildContent>().sizeBuild;

                        if (colliders != null && colliders.Length > 0 && isTriggers)
                        {
                            bool allTouchDrone = true; // ��������, �� �� ��������� ���������� ������� �� ����� "Drone"

                            foreach (BoxCollider boxCollider in colliders)
                            {
                                // �������� �� ��'����, �� ������������� � ��� BoxCollider
                                Collider[] overlappingColliders = Physics.OverlapBox(
                                    boxCollider.bounds.center,
                                    boxCollider.bounds.extents,
                                    boxCollider.transform.rotation);

                                // ��������, �� �������� �������� ��������� ���� � ������ ������� � ����� "Drone"
                                bool currentTouchesDrone = false;
                                foreach (Collider overlap in overlappingColliders)
                                {
                                    if ((overlap.gameObject.layer == LayerMask.NameToLayer("Drone")) && overlap.isTrigger)
                                    {

                                        currentTouchesDrone = true;
                                        break; // �������� ��������� �������, ����� ��������� �������� ��� ����� boxCollider
                                    }
                                }

                                // ���� ���� � ���� boxCollider �� ��������� "Drone", ������������ allTouchDrone = false
                                if (!currentTouchesDrone)
                                {
                                    allTouchDrone = false;
                                    break; // �������� � �����, ���� ����-���� boxCollider �� ��������� ��������
                                }
                            }

                            // ��������� �������� isBuild �� ����� ���������� ��������
                            isBuild = allTouchDrone;
                        }
                        else if (colliders == null || colliders.Length == 0)
                        {
                            isBuild = true; // ���� ���� ������� ���������, isBuild = false
                        }
                        else
                            isBuild = false;

                        // ������� ��'��� ��� ��������� ���
                        if (Input.GetMouseButtonDown(0) && isBuild && isTriggers)
                        {
                            // Додаємо перевірку на відстань
                            float distanceToOrigin = Vector3.Distance(lastValidPosition, Vector3.zero);
                            if (distanceToOrigin <= 23f)
                            {
                                if (JunkBuy <= Junk && MaterialsBuy <= Materials && Population + PopulationBuy <= PlayerPrefs.GetInt("Population", 0))
                                {
                                    //Debug.Log("Junk:" + Junk + "\tMaterail:" + Materials);

                                    PlayerPrefs.SetInt("Junk", PlayerPrefs.GetInt("Junk") -JunkBuy);
                                    PlayerPrefs.SetInt("Material", PlayerPrefs.GetInt("Material") -MaterialsBuy);
                                    PlayerPrefs.SetInt("PopulationActive", PlayerPrefs.GetInt("PopulationActive") + PopulationBuy);
                                    Junk = PlayerPrefs.GetInt("Junk");
                                    Materials = PlayerPrefs.GetInt("Material");
                                    Population=PlayerPrefs.GetInt("PopulationActive");
                                    PlaceBeam(lastValidPosition, lastValidRotation);
                                }
                                }
                                else
                            {
                                Debug.LogWarning("Об'єкт занадто далеко від початку координат і не може бути розміщений.");
                            }
                        }

                        else if (!isBuild || !isTriggers)
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

        // �������� �� ������� �����
        public void BuildUGUI(int who,int junk, int material,int population)
        {
            WhoBuild = who;
            Junk= PlayerPrefs.GetInt("Junk");
            Materials= PlayerPrefs.GetInt("Material"); 
            Population= PlayerPrefs.GetInt("PopulationActive",0);
            JunkBuy = junk;
            MaterialsBuy = material;
            PopulationBuy = population;
            Debug.Log(Materials+"\t"+Junk);
            if (junk <=Junk && material <= Materials && Population+PopulationBuy<= PlayerPrefs.GetInt("Population", 0))
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
        }
        void PlaceBeam(Vector3 position, Quaternion rotation)
        {
            // �������� �������� ��������� ����� �������� �� �����
            Collider beamCollider = finish;
            if (beamCollider == null)
            {
                Debug.LogError("Collider not found on the beam prefab!");
                return;
            }

            // ����� ������������� beamCollider, �� �� �
            //GameObject placedBeam = Instantiate(beamPrefab, position, rotation, transform);
            var instantiated = _container.InstantiatePrefab(beamPrefab[WhoBuild], position, rotation, transform);
            instantiated.gameObject.name = instantiated.gameObject.name + PlayerPrefs.GetInt("Index", 0);
            PlayerPrefs.SetInt("Index", PlayerPrefs.GetInt("Index")+1);

            instantiated.GetComponent<BuildContent>().enabled = false;
            if (instantiated.GetComponent<HouseGenerateResource>())
                instantiated.GetComponent<HouseGenerateResource>().enabled = true;
            if (instantiated.GetComponent<PopulationSave>())
                instantiated.GetComponent<PopulationSave>().enabled = true;
            // ��������� ��� ������� ���������
            Collider[] childColliders = instantiated.GetComponentsInChildren<Collider>();
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
            StartPlacing();
        }

        Collider GetFinishCollider(GameObject obj)
        {
            // ��������� �������� �� ����� "Finish" � ������� ��'�����
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
    
}