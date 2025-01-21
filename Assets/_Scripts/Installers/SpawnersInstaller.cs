using _Scripts.Entities;
using _Scripts.Entities.Factories;
using _Scripts.Entities.Interfaces;
using _Scripts.Spawners;
using UnityEngine;
using Zenject;

namespace _Scripts.Installers
{
    public class SpawnersInstaller : MonoInstaller
    {
        [SerializeField] private Meteorite[] _meteorites;

        public override void InstallBindings()
        {
            Container.Bind<IEntityFactory>()
                .To<MeteoriteFactory>()
                .AsTransient()
                .WithArguments(_meteorites)
                .WhenInjectedInto<EntitySpawner>();
        }
    }
}