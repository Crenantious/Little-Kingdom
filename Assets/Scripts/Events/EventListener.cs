using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    public Event @event;
    public EventConstraintsMono constraints;
    public UnityEvent unityEvent;

    private void OnEnable()
    {
        Debug.Log(0);
        if (@event)
            @event.Subscribe(InvokeUnityEvent, constraints.eventConstraints);
    }
    private void OnValidate()
    {
        if (@event)
            @event.Subscribe(InvokeUnityEvent, constraints.eventConstraints);
    }

    private void OnDestroy() =>
        @event.Unsubscribe(InvokeUnityEvent);

    void InvokeUnityEvent() =>
        unityEvent?.Invoke();
}
