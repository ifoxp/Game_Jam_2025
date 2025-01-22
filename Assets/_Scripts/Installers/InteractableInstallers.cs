using _Scripts.Utilities;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.Installers
{
    public class InteractableInstallers : MonoInstaller
    {
        [SerializeField] private GameObject _buildingInfoParent;
        [SerializeField] private TextMeshProUGUI _buildingNamePlace;

        public override void InstallBindings()
        {
            Container.Bind<GameObject>()
                .WithId(Keys.INTERACTABLE_INSTALLER)
                .FromInstance(_buildingInfoParent)
                .AsSingle();
            
            Container.Bind<TextMeshProUGUI>()
                .WithId(Keys.INTERACTABLE_INSTALLER)
                .FromInstance(_buildingNamePlace)
                .AsSingle();
        }
    }
}