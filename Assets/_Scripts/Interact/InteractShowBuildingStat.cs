using System;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace _Scripts.Interact
{
    public class InteractShowBuildingStat : MonoBehaviour, IInteractableByPointer
    {
        [SerializeField] private TextContainer[] _textContainers;
        
        private HouseGenerateResource _houseGenerateResource;
        
        private const string PRODUCES_LABEL = "Produces";
        private const string REQUIRES_LABEL = "Requires";
        private const string NONE_LABEL = "None";
        private const string ARROW_SYMBOL = "->";

        private const string GREEN_COLOR = "green";
        private const string RED_COLOR = "red";
        private const string WHITE_COLOR = "white";

        private void Awake() => _houseGenerateResource = GetComponent<HouseGenerateResource>();

        public void OnInteractByPointer()
        {
            UpdateText(_houseGenerateResource.GetCurrentUpgradeLevel());
            ToggleTextContainers(true);
        }

        public void OnStopInteractByPointer() => ToggleTextContainers(false);

        public void UpdateText(UpgradeLevel currentUpgrade)
        {
            var nextUpgrade = _houseGenerateResource.GetNextUpgradeLevel();
            UpdateLevelText(currentUpgrade, nextUpgrade);
        }

        private void UpdateLevelText(UpgradeLevel currentUpgrade, UpgradeLevel nextUpgrade)
        {
            foreach (var container in _textContainers)
            {
                container.PlaceText.text = GetFormattedText(container, currentUpgrade, nextUpgrade);
            }
        }

        private string GetFormattedText(TextContainer container, UpgradeLevel currentUpgrade, UpgradeLevel nextUpgrade)
        {
            return container.ShowTextType switch
            {
                TextContainer.TextType.Produce => FormatResourceList(currentUpgrade.resourcesProduced,
                    nextUpgrade?.resourcesProduced, PRODUCES_LABEL, true),
                TextContainer.TextType.Require => FormatResourceList(currentUpgrade.resourcesRequired,
                    nextUpgrade?.resourcesRequired, REQUIRES_LABEL, false),
                _ => container.Text
            };
        }

        private string FormatResourceList(List<ResourceData> resources, List<ResourceData> nextResources, 
            string label, bool isProduce)
        {
            if (resources == null || resources.Count == 0) return $"{label}: {NONE_LABEL}";

            var resourcePairs = GenerateResourcePairs(resources, nextResources, isProduce);
            var nextResourcePairs = GenerateNextResourcePairs(resources, nextResources, isProduce);

            var allResourceText = string.Join("\n", resourcePairs.Concat(nextResourcePairs ?? Enumerable.Empty<string>()));

            return $"{label}:\n{allResourceText}";
        }

        private IEnumerable<string> GenerateResourcePairs(List<ResourceData> resources, List<ResourceData> nextResources, bool isProduce)
        {
            return resources.Select(resource =>
            {
                var nextResource = nextResources?.FirstOrDefault(r => r.Name == resource.Name);
                var color = GetResourceColor(resource.Amount, nextResource?.Amount ?? 0, isProduce);
                return $"- {resource.Name}: {resource.Amount} {ARROW_SYMBOL} <color={color}>{nextResource?.Amount ?? 0}</color>";
            });
        }

        private IEnumerable<string> GenerateNextResourcePairs(List<ResourceData> resources, List<ResourceData> nextResources, bool isProduce)
        {
            return nextResources
                ?.Where(nextResource => resources.All(r => r.Name != nextResource.Name))
                .Select(nextResource =>
                {
                    var color = GetResourceColor(0, nextResource.Amount, isProduce);
                    return $"- {nextResource.Name}: 0 {ARROW_SYMBOL} <color={color}>{nextResource.Amount}</color>";
                });
        }

        private string GetResourceColor(int currentAmount, int nextAmount, bool isProduce)
        {
            if (isProduce)
            {
                return nextAmount > currentAmount ? GREEN_COLOR : nextAmount < currentAmount ? RED_COLOR : WHITE_COLOR;
            }
            return nextAmount < currentAmount ? GREEN_COLOR : nextAmount > currentAmount ? RED_COLOR : WHITE_COLOR;
        }

        private void ToggleTextContainers(bool isActive)
        {
            foreach (var container in _textContainers)
            {
                container.PlaceText.gameObject.SetActive(isActive);
            }
        }

        [Serializable]
        public class TextContainer
        {
            public string Text;
            [field: SerializeField] public TextMeshProUGUI PlaceText { get; private set; }
            [field: SerializeField] public TextType ShowTextType { get; private set; }

            public enum TextType
            {
                JustText,
                Produce,
                Require
            };
        }
    }
}
