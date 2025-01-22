using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        public uint foodAmount;
        public uint scrapAmount;
        public uint materialAmount;
        public uint energyAmount;
        
        [HorizontalLine]
        public uint dronePrice;
        
        [HorizontalLine]
        [SerializeField] TMP_Text foodAmountText;
        [SerializeField] TMP_Text scrapAmountText;
        [SerializeField] TMP_Text materialAmountText;
        [SerializeField] TMP_Text energyAmountText;
        
        
        
        public static ResourcesManager One { get; private set; }
        
        private void Awake()
        {
            if (One == null)
                One = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            foodAmountText.text = "Food: " + foodAmount;
            scrapAmountText.text = "Scrap: " + scrapAmount;
            materialAmountText.text = "Material: " + materialAmount;
            energyAmountText.text = "Energy: " + energyAmount;
        }
    }
}