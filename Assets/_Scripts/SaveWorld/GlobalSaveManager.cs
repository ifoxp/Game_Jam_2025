using _Scripts._BuildingsEarn;
using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using _Scripts._BuildingsEarn.Systems;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace _Scripts.DataModel
{
    public class GlobalSaveManager : MonoBehaviour
    {
        public Transform parent;
        public static GlobalSaveManager Instance;
        public SaveLoadJson saveLoadJson;
        private string saveFilePath;
        [Inject] private DiContainer _container;
        public float saveTime;
        // ������ ��� ���������� ��'����
        private List<SavableObject> savableObjects = new List<SavableObject>();
        public SceneObjectsStateManager sceneObjectsStateManager;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // ������������ ��� ��'���� ��� ������� �����
            LoadAllObjects();

            // �������������� ����� 2 �������
            InvokeRepeating(nameof(SaveAllObjects), saveTime, saveTime);
        }
        public void DeleteSave()
        {
            if (File.Exists(saveFilePath))
            {
                sceneObjectsStateManager.DeleteObjectStates();
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted: " + saveFilePath);
                // ��������� �� ������� ��'���� � parent
                foreach (Transform child in parent)
                {
                    Destroy(child.gameObject);
                }
                sceneObjectsStateManager.AllObjFalse();
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save(); // ������ ����
                saveLoadJson.DeleteSave();
            }
            else
            {
                Debug.Log("No save file found to delete.");
            }
        }
        // ������ ��'��� �� ������
        public void RegisterObject(SavableObject savableObject)
        {
            if (!savableObjects.Contains(savableObject))
            {
                savableObjects.Add(savableObject);
            }
        }

        // �������� ��'��� � ������
        public void UnregisterObject(SavableObject savableObject)
        {
            if (savableObjects.Contains(savableObject))
            {
                savableObjects.Remove(savableObject);

            }
        }

        // �������� �� ��'���� � ����
        public void SaveAllObjects()
        {
            List<SavableData> saveDataList = new List<SavableData>();

            foreach (var obj in savableObjects)
            {
                if (obj != null)
                {
                    saveDataList.Add(obj.GetSaveData());
                }
            }

            string json = JsonUtility.ToJson(new SaveDataWrapper { objects = saveDataList }, true);
            File.WriteAllText(saveFilePath, json);
            sceneObjectsStateManager.SaveObjectStates();
            Debug.Log("Game saved to: " + saveFilePath);
        }

        // ����������� ��'���� � �����
        public void LoadAllObjects()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);

                foreach (var data in wrapper.objects)
                {
                    GameObject prefab = Resources.Load<GameObject>(data.prefabName);
                    if (prefab != null)
                    {
                        //GameObject instance = Instantiate(prefab, data.position, data.rotation);
                        GameObject instance = _container.InstantiatePrefab(prefab, data.position, data.rotation, parent);
                        instance.transform.localScale = data.scale;
                        instance.GetComponent<UpgradableBuilding>()?.OnBuildingPlaced();
                        // ����������� ����� �������
                        SavableObject savableObject = instance.GetComponent<SavableObject>();
                        if (savableObject != null)
                        {
                            savableObject.ApplySavedState();
                        }
                    }
                }

                Debug.Log("Game loaded from: " + saveFilePath);
            }
        }


        [System.Serializable]
        private class SaveDataWrapper
        {
            public List<SavableData> objects;
        }
        [Button]
        void SaveButton()
        {
            SaveAllObjects();
        }
        [Button]
        void DeleteSaveButtone()
        {
            DeleteSave();
        }
    }
}
