using UnityEngine;
using Zenject;

public class Tile : MonoBehaviour
{
    public static float Width { get; private set; }
    public static float Height { get; private set; }
    [field: SerializeField, ReadOnly] public TileType TileType { get; private set; }
    [ReadOnly] public Vector2Int boardPosition;

    public Town town;

    [Inject]
    public void Construct(TileType tileType)
    {
        TileType = tileType;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = tileType.material;

        name = tileType.material.ToString() + " tile";

        Width = meshRenderer.bounds.size.x;
        // Height is the z value as models are rotated when imported
        Height = meshRenderer.bounds.size.z;
    }
    public class Factory : PlaceholderFactory<TileType, Tile>
    {

    }
}
