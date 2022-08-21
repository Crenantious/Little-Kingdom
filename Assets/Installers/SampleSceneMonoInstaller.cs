using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class SampleSceneMonoInstaller : MonoInstaller
{
    public GameObject tilePrefab;
    public VisualEffect highlightTilesVFXPrefab;
    public GameObject townPrefab;
    public Transform hideObjectsBehindCamera;
    public BoardManager boardManager;
    public GameStateManager gameStateManager;
    public UIPlayerInfoPanels UIPlayerInfoPanels;
    public InputManager inputManager;
    public UIOptionsMessageManager optionsMessageManager;

    public Event GameObjectSelectedEvent;
    public Event GameObjectDeselectedEvent;
    public Event MousePressedOnGameObject;
    public Event MouseReleasedOnGameObject;

    public override void InstallBindings()
    {
        Container.Bind<BoardManager>().FromComponentInHierarchy(boardManager).AsSingle();
        Container.Bind<GameStateManager>().FromComponentInHierarchy(gameStateManager).AsSingle();
        Container.Bind<UIPlayerInfoPanels>().FromComponentInHierarchy(UIPlayerInfoPanels).AsSingle();
        Container.Bind<InputManager>().FromComponentInHierarchy(UIPlayerInfoPanels).AsSingle();
        Container.Bind<UIOptionsMessageManager>().FromComponentInHierarchy(optionsMessageManager).AsSingle();

        Container.BindInterfacesAndSelfTo<TileBorders>().AsSingle();

        //Container.BindInstance(highlightTilesVFXPrefab).WhenInjectedInto<TileBorder>();
        //Container.BindInstance(hideObjectsBehindCamera).WhenInjectedInto<TileBorder>();

        Container.BindFactory<Resource, Tile, Tile.Factory>().FromComponentInNewPrefab(tilePrefab);
        Container.BindFactory<Tile, Vector3, float, Gradient, TileBorder, Factories.Border>().FromPoolableMemoryPool(
            x => x.WithInitialSize(16).WithArguments(highlightTilesVFXPrefab, hideObjectsBehindCamera));
        Container.BindFactory<Player, Town, Factories.Town>().FromComponentInNewPrefab(townPrefab);
        Container.BindFactory<Player, Factories.Player>();

        SetupReferences();
    }

    void SetupReferences()
    {
        References.gameStateManager = gameStateManager;
        References.GameObjectSelectedEvent = GameObjectSelectedEvent;
        References.GameObjectDeselectedEvent = GameObjectDeselectedEvent;
        References.MousePressedOnGameObject = MousePressedOnGameObject;
        References.MouseReleasedOnGameObject = MouseReleasedOnGameObject;
    }
}