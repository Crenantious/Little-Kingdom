using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : MonoBehaviour
{
    [Inject] readonly BoardManager boardManager;
    readonly PointerEventData pointerData = new(EventSystem.current);

    [SerializeField] new Camera camera;
    [SerializeField] LayerMask closestTileToMouse;
    GameObject gameObjectMousePressedOn;
    GameObject gameObjectMouseReleasedOn;
    GameObject selectedGameObject;

    public void LeftMouseButtonPressed(InputAction.CallbackContext cc)
    {
        if (!cc.performed)
            return;

        gameObjectMousePressedOn = GetUIObjectUnderMouse() ? null : GetNonUIObjectUnderMouse();
    }

    public void LeftMouseButtonReleased(InputAction.CallbackContext cc)
    {
        if (!cc.performed)
            return;

        gameObjectMouseReleasedOn = GetUIObjectUnderMouse() ? null : GetNonUIObjectUnderMouse();

        if (IsNewGameObjectSelected())
        {
            if (selectedGameObject)
                References.GameObjectDeselectedEvent.Invoke(new EventInfo(gameObject: selectedGameObject));
            References.GameObjectSelectedEvent.Invoke(new EventInfo(gameObject: gameObjectMousePressedOn));
            selectedGameObject = gameObjectMousePressedOn;
        }
    }

    /// <summary>
    /// Ignores UI elements.
    /// </summary>
    /// <returns>The tile closest to the mouse, or null if there was no collider under the mouse.</returns>
    public Tile GetTileClosestToMouse()
    {
        Tile tile = null;
        Vector2 mousePos = (Vector2)Mouse.current.position.ReadValueAsObject();

        if (Physics.Raycast(camera.ScreenPointToRay(mousePos), out RaycastHit hit, Mathf.Infinity, closestTileToMouse))
        {
            int x = Mathf.RoundToInt(hit.point.x / Tile.Width);
            int y = Mathf.RoundToInt(hit.point.z / Tile.Height);

            x = Mathf.Clamp(x, 0, boardManager.widthInTiles - 2);
            y = Mathf.Clamp(y, 0, boardManager.heightInTiles - 2);

            tile = boardManager.TryGetTileAtPosition(x, y);
        }
        return tile;
    }

    bool IsNewGameObjectSelected() =>
        gameObjectMouseReleasedOn &&
        gameObjectMouseReleasedOn == gameObjectMousePressedOn &&
        gameObjectMouseReleasedOn != selectedGameObject;

    GameObject GetNonUIObjectUnderMouse()
    {
        Vector2 mousePos = (Vector2)Mouse.current.position.ReadValueAsObject();

        if (Physics.Raycast(camera.ScreenPointToRay(mousePos), out RaycastHit hit))
            return hit.collider.gameObject;
        return null;
    }

    GameObject GetUIObjectUnderMouse()
    {
        pointerData.position = (Vector2)Mouse.current.position.ReadValueAsObject();
        List<RaycastResult> results = new();

        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
            return results[0].gameObject;
        return null;
    }
}
