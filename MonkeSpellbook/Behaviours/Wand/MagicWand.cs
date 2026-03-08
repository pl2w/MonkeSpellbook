using System;
using UnityEngine;
using UnityEngine.XR;

namespace MonkeSpellbook.Behaviours.Wand;

public class MagicWand : MonoBehaviour
{
    public void Update()
    {
        if (ControllerInputPoller.GetIndexPressed(XRNode.RightHand))
        {
            GorillaTagger.Instance.StartVibration(false, 0.25f, 0.1f);
        }
    }
}