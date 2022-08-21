using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;
using System;

[RequireComponent(typeof(VisualEffect))]
public class TileBorder : MonoBehaviour, IPoolable<Tile, Vector3, Vector3, Gradient, IMemoryPool>, IDisposable
{
    [SerializeField] string sideStartPosPropertyName = "";
    [SerializeField] string sideEndPosPropertyName = "";
    [SerializeField] string gradientPropertyName = "";
    [SerializeField] string allowedGradientPropertyName = "";
    [SerializeField] string notAllowedGradientPropertyName = "";

    IMemoryPool pool;
    VisualEffect vfx;

    public class DefaultGradients
    {
        public static Gradient allowed;
        public static Gradient notAllowed;
    }


    [Inject]
    public void Construct()
    {
        this.vfx = GetComponent<VisualEffect>();
        DefaultGradients.allowed = vfx.GetGradient(allowedGradientPropertyName);
        DefaultGradients.notAllowed = vfx.GetGradient(notAllowedGradientPropertyName);
    }

    public void OnSpawned(Tile tile, Vector3 start, Vector3 end, Gradient gradient, IMemoryPool pool)
    {
        this.pool = pool;

        vfx.SetVector3(sideStartPosPropertyName, start);
        vfx.SetVector3(sideEndPosPropertyName, end);
        vfx.SetGradient(gradientPropertyName, gradient);

        transform.SetParent(tile.transform);
        transform.localPosition = Vector3.zero;
    }

    public void Dispose() =>
        pool.Despawn(this);

    public void OnDespawned() =>
        pool = null;
}
