using UnityEngine;
using UnityEngine.VFX;
using Zenject;
using System;

public class TileBorder : IPoolable<Tile, Vector3, float, Gradient, IMemoryPool>, IDisposable
{
    const string borderLengthPropertyName = "Border length";
    const string gradientPropertyName = "Gradient";
    const string allowedGradientPropertyName = "Allowed";
    const string notAllowedGradientPropertyName = "Not allowed";

    IMemoryPool pool;
    VisualEffect vfx;

    public class DefaultGradients
    {
        public static Gradient allowed;
        public static Gradient notAllowed;
    }

    [Inject]
    public void Construct(VisualEffect vfxPrefab, Transform hideBehindCamera)
    {
        if (!hideBehindCamera)
            throw new ArgumentNullException($"Must set a value for {hideBehindCamera} otherwise disabled particles could still be visible.");

        vfx = UnityEngine.Object.Instantiate(vfxPrefab);
        vfx.transform.SetParent(hideBehindCamera);
        Utilities.ResetTransformLocally(vfx.transform);

        DefaultGradients.allowed = vfx.GetGradient(allowedGradientPropertyName);
        DefaultGradients.notAllowed = vfx.GetGradient(notAllowedGradientPropertyName);
    }

    public void OnSpawned(Tile tile, Vector3 positionOnTile, float localYRotation, Gradient gradient, IMemoryPool pool)
    {
        this.pool = pool;
        vfx.pause = false;

        // This is assuming the tiles are of equal width and height. If no, do some math to work out
        // the length based on the rotation of the border, the size and shape of the tile.
        vfx.SetFloat(borderLengthPropertyName, Tile.Width);
        vfx.SetGradient(gradientPropertyName, gradient);

        vfx.transform.position = positionOnTile + tile.transform.position;
        vfx.transform.rotation = Quaternion.Euler(0, localYRotation, 0);
        vfx.transform.localScale = Vector3.one;
    }

    public void Dispose() =>
        pool.Despawn(this);

    public void OnDespawned()
    {
        pool = null;
        vfx.pause = true;
        // Reset transform to make sure it is hidden from the camera view
        Utilities.ResetTransformLocally(vfx.transform);
    }
}
