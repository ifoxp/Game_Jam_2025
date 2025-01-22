using System;
using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Behaviours
{
    public class DroneBuyPanel : MonoBehaviour
    {
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text titleText;

        public GameObject droneToBuy;
        
        private void Start()
        {
            cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
            buyButton.onClick.AddListener(Buy);
        }

        private void Buy()
        {
            if(!droneToBuy && ResourcesManager.One.materialAmount < ResourcesManager.One.dronePrice)
                return;
            
            ResourcesManager.One.materialAmount -= ResourcesManager.One.dronePrice;
            droneToBuy.SetActive(true);
            gameObject.SetActive(false);
            ResourcesManager.One.dronePrice *= 2;
        }

        private void Update()
        {
            titleText.text = $"Buy new drone for {ResourcesManager.One.dronePrice} material?";
        }

        public static DroneBuyPanel One { get; private set; }
        
        private void Awake()
        {
            if (One == null)
                One = this;
            else
                Destroy(gameObject);
        }
    }
}