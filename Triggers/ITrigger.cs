using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    NoCrouch = 1,
    Particle = 2,
    Event = 3,
    Sound = 4,
    ObjectSpawn = 5,
    DoorStateChange = 6,
    EndOfLevel = 7,
    Damage = 8,
    Empty = 0
}

public interface ITrigger
{
    public TriggerType Type { get; set; }
    public void TriggerEnter(PlayerMovement player);
    public void TriggerExit(PlayerMovement player);
}
