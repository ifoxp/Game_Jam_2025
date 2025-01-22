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
        
        #if UNITY_EDITOR
        private void Reset()
        {
            if (_playerCamera == null)
            {
                Debug.Log("Trying to set _playerCamera into Camera.main");
                _playerCamera = Camera.main;

                if (_playerCamera == null)
                {
                    Debug.LogWarning("Camera main is null! Set _playerCamera in PlayerInstaller");
                }
            }
        }
        #endif
    }
}
