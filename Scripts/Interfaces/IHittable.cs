using Godot;
using System;

public interface IHittable
{
    // Effects: [Freeze, Burn, Poison, Purifier]
    void Hit(Node Origin, float damage, int[] Effects);
}
