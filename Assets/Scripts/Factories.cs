using UnityEngine.VFX;
using Zenject;
using UnityEngine;
public class Factories
{
    public class Player : PlaceholderFactory<global::Player> { }
    public class Town: PlaceholderFactory<global::Player, global::Town> { }
    public class Border : PlaceholderFactory<Tile, Vector3, Vector3, Gradient, global::TileBorder> { }
}
