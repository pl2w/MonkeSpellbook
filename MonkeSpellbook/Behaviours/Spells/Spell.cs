using GorillaLocomotion;
using UnityEngine;
using MonkeSpellbook.Systems;

namespace MonkeSpellbook.Behaviours.Spells;

public abstract class Spell
{
    public abstract string Name { get; }

    public bool IsActive = false;
    
    public virtual void Initialise() { }
    public abstract void Activate();
    public abstract void Deactivate();

    protected GTPlayer Player => SpellRuntime.Context.Player;
    protected Collider WandCollider => SpellRuntime.Context.WandCollider;
    protected Transform WandTip => SpellRuntime.Context.WandTip;
}