using Zenject;
using _Scripts.Inputs.Reader;

namespace _Scripts.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputReader>().AsSingle().NonLazy();
        }
    }
}
