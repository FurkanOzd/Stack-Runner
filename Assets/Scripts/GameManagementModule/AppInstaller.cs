using PathBlocksModule;
using Signals;
using Zenject;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallSignals();
        InstallFactories();
        InstallControllers();
    }

    private void InstallFactories()
    {
        Container.BindFactory<object[], Block, Block.Factory>()
            .FromFactory<BlockFactory>();
    }

    private void InstallControllers()
    {
        Container.BindInterfacesAndSelfTo<BlockController>().AsSingle().NonLazy();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<BlockFitSignal>().OptionalSubscriber();
        Container.DeclareSignal<GameStateChangedSignal>().OptionalSubscriber();
    }
}