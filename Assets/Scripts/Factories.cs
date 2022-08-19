using UnityEngine.VFX;
using Zenject;
public class Factories
{
    public class Player : PlaceholderFactory<global::Player> { }
    public class Town: PlaceholderFactory<global::Player, global::Town> { }
    public class Border : PlaceholderFactory<VisualEffect> { }
}
