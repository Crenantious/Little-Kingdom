using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onMouseDown;
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;
    public UnityEvent onMouseUp;
    public UnityEvent onMouseOver;
    public UnityEvent onMouseDrag;
    private void OnMouseDown()
    {
        onMouseDown.Invoke();
    }
    private void OnMouseEnter()
    {
        onMouseEnter.Invoke();
    }
    private void OnMouseExit()
    {
        onMouseExit.Invoke();
    }
    private void OnMouseUp()
    {
        onMouseUp.Invoke();
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
