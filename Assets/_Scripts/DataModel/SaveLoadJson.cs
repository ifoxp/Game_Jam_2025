using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using File = System.IO.File;

namespace _Scripts.DataModel
{
    public class SaveLoadJson : MonoBehaviour
    {
        [SerializeField] private int _saveEveryNSeconds = 60;
        private GameResourcesInventory _gameResourcesInventory;
        private string _saveFilePath;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _gameResourcesInventory = gameResourcesInventory;
        }

        private void Awake()
        {
            _saveFilePath = Application.persistentDataPath + "/save.json";
            LoadGame();
        }

        private void Start()
        {
            InvokeRepeating(nameof(SaveGame), _saveEveryNSeconds, _saveEveryNSeconds);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public void SaveGame()
        {
            var quantities = new int[_gameResourcesInventory.GameResources.Count];
            var index = 0;
            foreach (var value in _gameResourcesInventory.GameResources.Values)
            {
                quantities[index] = value;
                index++;
            }

            var savedData = JsonUtility.ToJson(new ResourceQuantitiesWrapper { Quantities = quantities });
            File.WriteAllText(_saveFilePath, savedData);
        }

        public void LoadGame()
        {
            if (File.Exists(_saveFilePath))
            {
                var savedData = File.ReadAllText(_saveFilePath);
                var wrapper = JsonUtility.FromJson<ResourceQuantitiesWrapper>(savedData);

                if (wrapper != null && wrapper.Quantities.Length == _gameResourcesInventory.GameResources.Count)
                {
                    var restoredResources = new Dictionary<GameResourcesType, int>();
                    var keys = Enum.GetValues(typeof(GameResourcesType));

                    var index = 0;
                    foreach (GameResourcesType resourceType in keys)
                    {
                        restoredResources[resourceType] = wrapper.Quantities[index];
                        index++;
                    }

                    _gameResourcesInventory.InitializeResources(restoredResources);
                }
            }
        }
        public void DeleteSave()
        {
            if (File.Exists(_saveFilePath))
            {
                File.Delete(_saveFilePath);
                SaveGame();
            }
        }
        [Serializable]
        private class ResourceQuantitiesWrapper
        {
            public int[] Quantities;
        }
    }
}
