using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class Factories
{
    public class Player : PlaceholderFactory<global::Player> { }
    public class Town: PlaceholderFactory<global::Player, global::Town> { }
}
