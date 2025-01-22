using Zenject;
using _Scripts.Inputs.Reader;
using UnityEngine;

namespace _Scripts.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private Camera _playerCamera;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputReader>().AsSingle().NonLazy();
            Container.Bind<Camera>().FromInstance(_playerCamera).AsSingle();
        }
    }
}
