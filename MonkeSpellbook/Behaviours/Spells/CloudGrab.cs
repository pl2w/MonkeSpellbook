using MonkeSpellbook.Systems;
using UnityEngine;

namespace MonkeSpellbook.Behaviours.Spells;

public class CloudGrab : Spell
{
    public override string Name => "cloudgrab";

    private GameObject _cloud;
    private GameObject _leftCloud, _rightCloud;
    
    public override void Initialise()
    {
        _cloud = AssetLoader.LoadAsset<GameObject>("TestObject");
    }
i
    public override void Activate()
    {
        if (!_cloud) 
            return;
        
        _leftCloud ??= Object.Instantiate(_cloud);
        _rightCloud ??= Object.Instantiate(_cloud);
        
        _leftCloud.SetActive(false);
        _rightCloud.SetActive(false);
        
        ControllerInputPoller.AddCallbackOnPressStart(EControllerInputPressFlags.Grip, OnGrabInput);
        ControllerInputPoller.AddCallbackOnPressEnd(EControllerInputPressFlags.Grip, OnReleaseInput);
    }

    public override void Deactivate()
    {
        ControllerInputPoller.RemoveCallbackOnPressStart(OnGrabInput); 
        ControllerInputPoller.RemoveCallbackOnPressEnd(OnReleaseInput); 
        
        if (_leftCloud != null) Object.Destroy(_leftCloud); 
        if (_rightCloud != null) Object.Destroy(_rightCloud);
        
        _leftCloud = null; 
        _rightCloud = null;
    }
    
    private void OnGrabInput(EHandednessFlags hand)
    {
        if ((hand & EHandednessFlags.Left) != 0) OnGrab(true); 
        if ((hand & EHandednessFlags.Right) != 0) OnGrab(false);
    }
    
    private void OnReleaseInput(EHandednessFlags hand)
    {
        if ((hand & EHandednessFlags.Left) != 0) OnRelease(true); 
        if ((hand & EHandednessFlags.Right) != 0) OnRelease(false);    
    }

    private void OnGrab(bool isLeftHand)
    {
        var cloud = isLeftHand ? _leftCloud : _rightCloud; 
        if (cloud == null) 
            return; 
        
        var handPos = isLeftHand ? 
            SpellRuntime.Context.Player.LastLeftHandPosition : 
            SpellRuntime.Context.Player.LastRightHandPosition; 
        
        if (SpellRuntime.Context.WandCollider.bounds.Contains(handPos)) 
            return; 
        
        cloud.SetActive(true);
        cloud.transform.position = handPos;
    }

    private void OnRelease(bool isLeftHand)
    {
        var cloud = isLeftHand ? _leftCloud : _rightCloud;
        cloud?.SetActive(false);
    }
}