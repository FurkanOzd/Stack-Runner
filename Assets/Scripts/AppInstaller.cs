using PathBlocksModule;
using Zenject;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallFactories();
        InstallControllers();
    }

    private void InstallFactories()
    {
        Container.BindFactory<BlockSpawnOptions, Block, Block.Factory>()
            .FromFactory<BlockFactory>();
    }

    private void InstallControllers()
    {
        Container.BindInterfacesAndSelfTo<BlockController>().AsSingle().NonLazy();
    }
}