using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    public List<TimedEvent> timedEvents;
    public bool stopQuedEventsIfTriggered;

    public void DoEventsDelayed()
    {
        if (!enabled) return;

        if (stopQuedEventsIfTriggered)
            StopAllCoroutines();

        foreach (TimedEvent ev in timedEvents)
            StartCoroutine(DelayEvents(ev));
    }

    public void CancelEvents()
    {
        StopAllCoroutines();
    }

    IEnumerator DelayEvents(TimedEvent ev)
    {
        yield return new WaitForSeconds(ev.delay);
        ev.delayedEvent?.Invoke();
    }
}

[System.Serializable]
public struct TimedEvent
{
    public UnityEvent delayedEvent;
    public float delay;
}
