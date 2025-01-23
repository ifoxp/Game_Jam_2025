using System;
using _Scripts.DataModel;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.UI
{
    public class UpdateResourceAmountUI : MonoBehaviour
    {
        [SerializeField] private UpdateContainerUI[] _itemsContainerUI;
        private GameResourcesInventory _resourcesInventory;

        [Inject]
        private void Construct(GameResourcesInventory gameResourcesInventory)
        {
            _resourcesInventory = gameResourcesInventory;
        }

        private void Awake()
        {
            if(_resourcesInventory == null) 
                throw new MissingComponentException("Missing GameResourcesInventory. Add data installer to the scene.");
            
            _resourcesInventory.OnResourceChanged += UpdateResourceUI;
        }

        private void OnDestroy()
        {
            if(_resourcesInventory != null) _resourcesInventory.OnResourceChanged -= UpdateResourceUI;
        }

        private void UpdateResourceUI(GameResourceContainer newResource)
        {
            var amountText = GetTextByResourceType(newResource.ResourceType);
            if(amountText == null) 
                throw new MissingComponentException("Missing text for resource type: " + newResource.ResourceType);
            
            amountText.text = $"{newResource.ResourceType.ToString()}: {newResource.Quantity}";
        }

        private TextMeshProUGUI GetTextByResourceType(GameResourcesType resourcesType)
        {
            foreach (var container in _itemsContainerUI)
            {
                if (container.TypeOfResource == resourcesType)
                {
                    return container.AmountOfResourceTypeText;
                }
            }
            
            return null;
        }

        [Serializable]
        public class UpdateContainerUI
        {
            [field: SerializeField] public GameResourcesType TypeOfResource { get; private set; }
            [field: SerializeField] public TextMeshProUGUI AmountOfResourceTypeText { get; private set; }
        }
    }
}
