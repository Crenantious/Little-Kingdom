using System.Collections.Generic;
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

    public override void InstallBindings()
    {
        Container.Bind<BoardManager>().FromComponentInHierarchy(boardManager).AsSingle();
        Container.Bind<GameStateManager>().FromComponentInHierarchy(gameStateManager).AsSingle();
        Container.BindInterfacesAndSelfTo<TileBorders>().AsSingle();

        Container.BindFactory<TileType, Tile, Tile.Factory>().FromComponentInNewPrefab(tilePrefab);
        Container.BindFactory<VisualEffect, TileBorders.BorderFactory>().FromComponentInNewPrefab(highlightTilesVFXPrefab);
        Container.BindFactory<Player, Town, Town.Factory>().FromComponentInNewPrefab(townPrefab);
        Container.BindFactory<Player, Player.Factory>();

        SetupReferences();
    }

    void SetupReferences()
    {
        References.gameStateManager = gameStateManager;
    }
}