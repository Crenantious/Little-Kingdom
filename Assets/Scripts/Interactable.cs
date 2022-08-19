using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] bool invokeDefaultEvents = true;
    [SerializeField] UnityEvent onMouseDown;
    [SerializeField] UnityEvent onMouseEnter;
    [SerializeField] UnityEvent onMouseExit;
    [SerializeField] UnityEvent onMouseUp;
    [SerializeField] UnityEvent onMouseOver;
    [SerializeField] UnityEvent onMouseDrag;
    [SerializeField] UnityEvent onMouseDownUp;

    bool mouseDown = false;
    bool mouseOver = false;

    private void OnMouseDown()
    {
        mouseDown = true;
        onMouseDown.Invoke();
    }
    private void OnMouseEnter()
    {
        mouseOver = true;
        onMouseEnter.Invoke();
    }
    private void OnMouseExit()
    {
        mouseOver = false;
        onMouseExit.Invoke();
    }
    private void OnMouseUp()
    {
        onMouseUp.Invoke();
        if (mouseDown && mouseOver)
            onMouseDownUp.Invoke();
        mouseDown = false;
    }
    private void OnMouseOver()
    {
        onMouseOver.Invoke();
    }
    private void OnMouseDrag()
    {
        onMouseDrag.Invoke();
    }
}
