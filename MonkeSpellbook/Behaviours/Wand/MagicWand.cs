using System;
using UnityEngine;
using UnityEngine.XR;

namespace MonkeSpellbook.Behaviours.Wand;

public class MagicWand : HoldableObject
{
    public void Update()
    {
        if (ControllerInputPoller.GetIndexPressed(XRNode.RightHand))
        {
            GorillaTagger.Instance.StartVibration(false, 0.25f, 0.1f);
        }
    }

    public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
    {
        throw new NotImplementedException();
    }

    public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
    {
        throw new NotImplementedException();
    }

    public override void DropItemCleanup()
    {
        throw new NotImplementedException();
    }
}