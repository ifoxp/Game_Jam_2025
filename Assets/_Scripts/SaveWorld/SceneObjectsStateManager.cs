using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneObjectsStateManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToTrackTrue; // Список об'єктів для відстеження стану
    [SerializeField] private List<GameObject> objectsToTrackFalse; // Список об'єктів для відстеження стану

    private string saveFilePath;

    private void Awake()
    {
        // Визначаємо шлях до файлу збереження
        saveFilePath = Path.Combine(Application.persistentDataPath, "sceneObjectsState.json");
    }

    private void Start()
    {
        // Завантаження стану об'єктів при старті гри
        LoadObjectStates();
    }

    // Зберігає стан об'єктів у файл
    public void SaveObjectStates()
    {
        List<ObjectState> states = new List<ObjectState>();

        foreach (var obj in objectsToTrackFalse)
        {
            if (obj != null)
            {
                states.Add(new ObjectState
                {
                    objectName = obj.name,
                    isActive = obj.activeSelf
                });
            }
        }
        foreach (var obj in objectsToTrackTrue)
        {
            if (obj != null)
            {
                states.Add(new ObjectState
                {
                    objectName = obj.name,
                    isActive = obj.activeSelf
                });
            }
        }

        string json = JsonUtility.ToJson(new ObjectStatesWrapper { objectStates = states }, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Scene objects state saved to: " + saveFilePath);
    }

    // Завантажує стан об'єктів із файлу
    public void LoadObjectStates()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            ObjectStatesWrapper wrapper = JsonUtility.FromJson<ObjectStatesWrapper>(json);

            foreach (var state in wrapper.objectStates)
            {
                foreach (var obj in objectsToTrackFalse)
                {
                    if (obj != null && obj.name == state.objectName)
                    {
                        obj.SetActive(state.isActive);
                    }
                }
                foreach (var obj in objectsToTrackTrue)
                {
                    if (obj != null && obj.name == state.objectName)
                    {
                        obj.SetActive(state.isActive);
                    }
                }
            }

            Debug.Log("Scene objects state loaded from: " + saveFilePath);
        }
    }

    // Видаляє файл збереження
    public void DeleteObjectStates()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Scene objects state file deleted: " + saveFilePath);
        }
        else
        {
            Debug.Log("No scene objects state file found to delete.");
        }
    }
    public void AllObjFalse()
    {
        foreach(var obj in objectsToTrackFalse)
        {
            obj?.SetActive(false);
        }
        foreach (var obj in objectsToTrackTrue)
        {
            obj?.SetActive(true);
        }
    }
    [System.Serializable]
    private class ObjectState
    {
        public string objectName; // Ім'я об'єкта
        public bool isActive; // Стан активності
    }

    [System.Serializable]
    private class ObjectStatesWrapper
    {
        public List<ObjectState> objectStates; // Список станів об'єктів
    }
}
