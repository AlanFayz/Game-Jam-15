using Godot;
using System;

public interface IHittable
{
    void Hit(Node Origin, float damage);
}
