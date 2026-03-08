using UnityEngine;

namespace MonkeSpellbook.Behaviours.Wand;

public class MagicWand : GrabbableObject
{
    private Transform _wandTip;

    protected override void Awake()
    {
        base.Awake();

        returnToDropPointOnDrop = true;
        
        grabPoint = transform.Find("GrabPoint");
        
        dropPoint = new GameObject("DropPoint_Wand").transform;
        dropPoint.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body_pivot/").transform);
        dropPoint.localPosition = Vector3.forward * 0.5f;
        dropPoint.localRotation = Quaternion.identity;
        dropPoint.localScale = Vector3.one;
        
        _wandTip = transform.Find("WandTip");
    }
    
    protected override void Update()
    {
        base.Update();

        if (isInHand)
        {
            
        }
    }
}