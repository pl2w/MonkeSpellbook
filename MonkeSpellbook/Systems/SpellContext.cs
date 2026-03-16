using GorillaLocomotion;
using MonkeSpellbook.Behaviours.Wand;
using UnityEngine;

namespace MonkeSpellbook.Systems;

public class SpellContext(Collider wandCollider, Transform wandTip, GTPlayer player, MagicWand wand)
{
    public Collider WandCollider { get; } = wandCollider;
    public Transform WandTip { get; } = wandTip;
    public GTPlayer Player { get; } = player;
    public MagicWand Wand { get; } = wand;
}