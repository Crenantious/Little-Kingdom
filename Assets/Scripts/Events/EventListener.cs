using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    public Event @event;
    public EventConstraintsMono constraints;
    public UnityEvent unityEvent;

    private void OnEnable()
    {
        if (@event)
            @event.Subscribe(InvokeUnityEvent,
                constraints ? constraints.eventConstraints : null);
    }
    private void OnValidate()
    {
        if (@event)
            @event.Subscribe(InvokeUnityEvent,
                constraints ? constraints.eventConstraints : null);
    }

    private void OnDestroy() =>
        @event.Unsubscribe(InvokeUnityEvent);

    void InvokeUnityEvent() =>
        unityEvent?.Invoke();
}
