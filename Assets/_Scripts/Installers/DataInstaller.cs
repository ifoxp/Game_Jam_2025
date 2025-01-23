using _Scripts.DataModel;
using Zenject;

namespace _Scripts.Installers
{
    public class DataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameResourcesInventory>()
                .AsSingle()
                .NonLazy();
        }
    }
}