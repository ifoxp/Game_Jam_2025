using _Scripts.Interact;
using _Scripts.Managers;
using UnityEngine;
using Zenject;

namespace _Scripts.Installers
{
    public class ManagersInstaller : MonoInstaller
    {
        [SerializeField] private InteractByPointer _interactByPointer;
        [SerializeField] private BuildingManagerSelectorUI _buildingManagerSelectorUI;

        public override void InstallBindings()
        {
            Container.Bind<InteractByPointer>().FromInstance(_interactByPointer).AsSingle();
            Container.Bind<BuildingManagerSelectorUI>().FromInstance(_buildingManagerSelectorUI).AsSingle();
        }
    }
}