using System.Collections.Generic;
using UnityEngine;

public class LocalEventBus {
    private readonly Dictionary<System.Type, object> eventBindings = new Dictionary<System.Type, object>();
    
    public void Register<T>(EventBinding<T> binding) where T : IEvent {
        var eventType = typeof(T);
        if (!eventBindings.ContainsKey(eventType)) {
            eventBindings[eventType] = new HashSet<IEventBinding<T>>();
        }
        
        var bindings = (HashSet<IEventBinding<T>>)eventBindings[eventType];
        bindings.Add(binding);
    }
    
    public void Deregister<T>(EventBinding<T> binding) where T : IEvent {
        var eventType = typeof(T);
        if (eventBindings.ContainsKey(eventType)) {
            var bindings = (HashSet<IEventBinding<T>>)eventBindings[eventType];
            bindings.Remove(binding);
        }
    }
    
    public void Raise<T>(T @event) where T : IEvent {
        var eventType = typeof(T);
        if (eventBindings.ContainsKey(eventType)) {
            var bindings = (HashSet<IEventBinding<T>>)eventBindings[eventType];
            var snapshot = new HashSet<IEventBinding<T>>(bindings);
            
            foreach (var binding in snapshot) {
                if (bindings.Contains(binding)) {
                    binding.OnEvent.Invoke(@event);
                    binding.OnEventNoArgs.Invoke();
                }
            }
        }
    }
    
    public void Clear() {
        eventBindings.Clear();
    }
    
    public void Clear<T>() where T : IEvent {
        var eventType = typeof(T);
        if (eventBindings.ContainsKey(eventType)) {
            eventBindings.Remove(eventType);
        }
    }
}
