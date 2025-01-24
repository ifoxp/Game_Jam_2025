using _Scripts._BuildingsEarn;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.DataModel
{
    public class SavableObject : MonoBehaviour
    {
        [SerializeField] private string prefabName; // Назва префабу або об'єкта

        // Масив імен скриптів, які мають бути false після завантаження
        [SerializeField] private string[] scriptsToSetFalse;

        // Масив імен скриптів, які мають бути true після завантаження
        [SerializeField] private string[] scriptsToSetTrue;

        public UpgradableBuilding UpdateResourceAmountUI;

        private void Start()
        {
            // Реєструємо об'єкт у менеджері збереження
            GlobalSaveManager.Instance.RegisterObject(this);

            // Перевіряємо, чи збережена назва збігається з поточною
            if (string.IsNullOrEmpty(prefabName))
            {
                prefabName = gameObject.name; // Якщо назва не задана, встановлюємо ім'я об'єкта
            }
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
                scriptsToSetTrue = scriptsToSetTrue,
                objectName = gameObject.name // Додаємо назву об'єкта
            };
        }

        // Застосовує збережений стан до об'єкта
        public void ApplySavedState(SavableData savedData)
        {
            prefabName = savedData.prefabName;
            transform.position = savedData.position;
            transform.rotation = savedData.rotation;
            transform.localScale = savedData.scale;

            // Встановлюємо стан скриптів
            SetScriptsState(savedData.scriptsToSetFalse, false);
            SetScriptsState(savedData.scriptsToSetTrue, true);

            // Встановлюємо ім'я об'єкта після завантаження
            gameObject.name = savedData.objectName;
        }

        // Допоміжний метод для встановлення стану скриптів
        private void SetScriptsState(string[] scriptNames, bool enable)
        {
            foreach (string scriptName in scriptNames)
            {
                System.Type scriptType = System.Type.GetType(scriptName);
                if (scriptType != null)
                {
                    MonoBehaviour script = (MonoBehaviour)GetComponent(scriptType);
                    if (script != null)
                    {
                        script.enabled = enable;
                    }
                    else
                    {
                        Debug.LogWarning($"Скрипт {scriptName} не знайдено на об'єкті {gameObject.name}.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Скрипт {scriptName} не існує в проекті.");
                }
            }
        }
    }

    [System.Serializable]
    public class SavableData
    {
        public string prefabName; // Назва префабу
        public Vector3 position; // Позиція
        public Quaternion rotation; // Ротація
        public Vector3 scale; // Масштаб
        public string[] scriptsToSetFalse; // Скрипти для вимкнення
        public string[] scriptsToSetTrue; // Скрипти для увімкнення
        public string objectName; // Назва об'єкта
    }
}
