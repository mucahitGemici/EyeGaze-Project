﻿/* 
 * mayra barrera, 2017
 * event manager (based on dali's)
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AppEvent
{

}

public class EventManager : MonoBehaviour
{
    public bool LimitQueueProcesing = false;
    public float QueueProcessTime = 0.0f;
    private static EventManager s_Instance = null;
    private Queue m_eventQueue = new Queue();

    public delegate void EventDelegate<T>(T e) where T : AppEvent;
    private delegate void EventDelegate(AppEvent e);

    private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
    private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
    private Dictionary<System.Delegate, System.Delegate> onceLookups = new Dictionary<System.Delegate, System.Delegate>();

    // override so we don't have to typecast the object
    public static EventManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindObjectOfType(typeof(EventManager)) as EventManager;
            }
            return s_Instance;
        }
    }

    private EventDelegate AddDelegate<T>(EventDelegate<T> del) where T : AppEvent
    {
        // Early-out if we've already registered this delegate
        if (delegateLookup.ContainsKey(del))
            return null;

        // Create a new non-generic delegate which calls our generic one.
        // This is the delegate we actually invoke.
        EventDelegate internalDelegate = (e) => del((T)e);
        delegateLookup[del] = internalDelegate;

        EventDelegate tempDel;
        if (delegates.TryGetValue(typeof(T), out tempDel))
        {
            delegates[typeof(T)] = tempDel += internalDelegate;
        }
        else
        {
            delegates[typeof(T)] = internalDelegate;
        }

        return internalDelegate;
    }

    public void AddListener<T>(EventDelegate<T> del) where T : AppEvent
    {
        AddDelegate<T>(del);
    }

    public void RemoveAll()
    {
        delegates.Clear();
        delegateLookup.Clear();
        onceLookups.Clear();
    }

    public void TriggerEvent(AppEvent e)
    {
        EventDelegate del;
        if (delegates.TryGetValue(e.GetType(), out del))
        {
            del.Invoke(e);

            // remove listeners which should only be called once
            foreach (EventDelegate k in delegates[e.GetType()].GetInvocationList())
            {
                if (onceLookups.ContainsKey(k))
                {
                    delegates[e.GetType()] -= k;

                    if (delegates[e.GetType()] == null)
                    {
                        delegates.Remove(e.GetType());
                    }

                    delegateLookup.Remove(onceLookups[k]);
                    onceLookups.Remove(k);
                }
            }
        }
        else
        {
            Debug.LogWarning("Event: " + e.GetType() + " has no listeners");
        }
    }

    //Inserts the event into the current queue.
    public bool QueueEvent(AppEvent evt)
    {
        if (!delegates.ContainsKey(evt.GetType()))
        {
            Debug.LogWarning("EventManager: QueueEvent failed due to no listeners for event: " + evt.GetType());
            return false;
        }

        m_eventQueue.Enqueue(evt);
        return true;
    }

    //Every update cycle the queue is processed, if the queue processing is limited,
    //a maximum processing time per update can be set after which the events will have
    //to be processed next update loop.
    void Update()
    {
        float timer = 0.0f;
        while (m_eventQueue.Count > 0)
        {
            if (LimitQueueProcesing)
            {
                if (timer > QueueProcessTime)
                    return;
            }

            AppEvent evt = m_eventQueue.Dequeue() as AppEvent;
            TriggerEvent(evt);

            if (LimitQueueProcesing)
                timer += Time.deltaTime;
        }
    }

    public void OnApplicationQuit()
    {
        RemoveAll();
        m_eventQueue.Clear();
        s_Instance = null;
    }
}
