using _Scripts._BuildingsEarn;
using _Scripts._BuildingsEarn.Systems;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.DataModel
{
    public class SavableObject : MonoBehaviour
    {
        [SerializeField] private string prefabName;

        // ����� ���� �������, �� ����� ���� false ���� ������������
        [SerializeField] private string[] scriptsToSetFalse;

        // ����� ���� �������, �� ����� ���� true ���� ������������
        [SerializeField] private string[] scriptsToSetTrue;
        public UpgradableBuilding UpdateResourceAmountUI;
        //public Transform parent;
        private void Start()
        {
            // �������� ��'��� � �������� ����������
            GlobalSaveManager.Instance.RegisterObject(this);
          
        }

        private void OnDestroy()
        {
            if (GlobalSaveManager.Instance != null)
            {
                GlobalSaveManager.Instance.UnregisterObject(this);
            }
        }

        // ������� ���� ��� ����������
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

        // ���������� ����� ������� ���� ������������
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
}