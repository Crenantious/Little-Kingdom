using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class SampleSceneMonoInstaller : MonoInstaller
{
    public GameObject tilePrefab;
    public GameObject highlightTilesVFXPrefab;
    public GameObject townPrefab;
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

        Container.BindFactory<Resource, Tile, Tile.Factory>().FromComponentInNewPrefab(tilePrefab);
        Container.BindFactory<Tile, Vector3, Vector3, Gradient, TileBorder, Factories.Border>().FromMonoPoolableMemoryPool(
            x => x.WithInitialSize(16).FromComponentInNewPrefab(highlightTilesVFXPrefab));
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