using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class SampleSceneMonoInstaller : MonoInstaller
{
    public GameObject tilePrefab;
    public GameObject highlightTilesVFXPrefab;
    public override void InstallBindings()
    {
        Container.BindFactory<TileType, Tile, Tile.Factory>().FromComponentInNewPrefab(tilePrefab);
        Container.BindFactory<VisualEffect, TileBorders.BorderFactory>().FromComponentInNewPrefab(highlightTilesVFXPrefab);
        Container.Bind<BoardManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<TileBorders>().AsSingle();
    }
}