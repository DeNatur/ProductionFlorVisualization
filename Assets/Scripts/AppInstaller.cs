using Microsoft.Azure.SpatialAnchors.Unity;
using Zenject;

public class AppInstaller : MonoInstaller
{

    public AzureSessionCoordinator sessionCoordinator;
    public AddAnchorUseCase addAnchorUseCase;
    public SpatialAnchorManager cloudManager;
    public ObjectsCreatorImpl objectsCreator;
    public SceneAwarnessValidator sceneAwarnessValidator;

    public override void InstallBindings()
    {
        Container.Bind<AnchorsRepository>()
            .To<AzureAnchorsReporitory>()
            .AsSingle();

        Container.Bind<AddAnchorUseCase>()
            .AsSingle();

        Container.Bind<RemoveAnchorUseCase>()
            .AsSingle();

        Container.Bind<AzureSessionCoordinator>()
            .FromInstance(sessionCoordinator)
            .NonLazy();

        Container.Bind<SpatialAnchorManager>()
            .FromInstance(cloudManager)
            .AsSingle();

        Container.Bind<ObjectCreator>()
            .FromInstance(objectsCreator)
            .AsSingle();

        Container.Bind<AwarnessValidator>()
            .FromInstance(sceneAwarnessValidator)
            .AsSingle();

        Container.Bind<AnchorCreator>()
            .To<AzureCloudManager>()
            .AsCached();

        Container.Bind<AnchorRemover>()
            .To<AzureCloudManager>()
            .AsCached();

        Container.Bind<StartAzureSession>()
            .To<AzureCloudManager>()
            .AsCached();

        Container.Bind<GameObjectEditor>()
            .To<GameObjectEditorImpl>()
            .AsSingle();

        Container.Bind<AnchorLocator>()
            .To<AzureAnchorLocator>()
            .AsSingle();

        Container.Bind<IBoundsControlEditor>()
            .To<BoundsControlRepository>()
            .AsCached();

        Container.Bind<IBoundsControlProvider>()
            .To<BoundsControlRepository>()
            .AsCached();

        Container.Bind<UserMenuPresenter>()
            .AsSingle();

        Container.Bind<UserMenuView>()
            .AsSingle();

        Container.BindFactory<int, AnchorPresenter, AnchorPresenter.Factory>();

        Container.BindFactory<UnityEngine.Object, AnchorPresenter, MachineView, MachineView.Factory>()
            .FromFactory<MachineViewFactory>();

    }
}
