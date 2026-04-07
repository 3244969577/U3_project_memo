using UnityEngine;

public interface IEvent { }

public struct TestEvent : IEvent { }

// test
public struct PlayerEvent : IEvent {
    public int health;
    public int mana;
}
