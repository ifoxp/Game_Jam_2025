using UnityEngine;

public class SavableObject : MonoBehaviour
{
    [SerializeField] private string prefabName;

    // Масив імен скриптів, які мають бути false після завантаження
    [SerializeField] private string[] scriptsToSetFalse;

    // Масив імен скриптів, які мають бути true після завантаження
    [SerializeField] private string[] scriptsToSetTrue;
    //public Transform parent;
    private void Start()
    {
        // Реєструємо об'єкт у менеджері збереження
        GlobalSaveManager.Instance.RegisterObject(this);
    }

    private void OnDestroy()
    {
        if (GlobalSaveManager.Instance != null)
        {
            GlobalSaveManager.Instance.UnregisterObject(this);
        }
    }

    // Повертає дані для збереження
    public SavableData GetSaveData()
    {
        return new SavableData
        {
            prefabName = prefabName,
            position = transform.position,
            rotation = transform.rotation,
            scale = transform.localScale,
            scriptsToSetFalse = scriptsToSetFalse,
            scriptsToSetTrue = scriptsToSetTrue
        };
    }

    // Встановлює стани скриптів після завантаження
    public void ApplySavedState()
    {
        foreach (string scriptName in scriptsToSetFalse)
        {
            MonoBehaviour script = (MonoBehaviour)GetComponent(scriptName);
            if (script != null)
            {
                script.enabled = false;
            }
        }

        foreach (string scriptName in scriptsToSetTrue)
        {
            MonoBehaviour script = (MonoBehaviour)GetComponent(scriptName);
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }
}

[System.Serializable]
public class SavableData
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string[] scriptsToSetFalse;
    public string[] scriptsToSetTrue;
}
