using System.Collections.Generic;
using MonkeSpellbook.Systems;
using UnityEngine;

namespace MonkeSpellbook.Behaviours.Spells;

public class IcePath : Spell
{
    public override string Name => "icepath";

    private GameObject _baseIcePath;
    private ObjectPool _pool;
    
    private Vector3 _lastSpawnPosLeft;
    private Vector3 _lastSpawnPosRight;
    private bool _isDrawingLeft;
    private bool _isDrawingRight;
    
    private readonly List<GameObject> _activeSegments = [];

    private const float SpawnSpacing = 0.1f;

    private GameObject _poolParent;
    
    private readonly Vector3 _positionOffset = new(0f, 0.05f, 0f);
    
    public override void Initialise()
    {
        _baseIcePath = AssetLoader.LoadAsset<GameObject>("Ice");
        _baseIcePath.AddComponent<GorillaSurfaceOverride>().overrideIndex = 59; // Ice index
        _baseIcePath.SetActive(false);
        
        _poolParent = new GameObject("IcePathPool");
        _pool = new ObjectPool(_baseIcePath, 500, _poolParent.transform);
    }

    public override void Activate()
    {
        if (!_baseIcePath) 
            return;
        
        ControllerInputPoller.AddCallbackOnPressStart(EControllerInputPressFlags.Grip, OnGrabInput);
        ControllerInputPoller.AddCallbackOnPressEnd(EControllerInputPressFlags.Grip, OnReleaseInput);
        ControllerInputPoller.AddCallbackOnPressUpdate(EControllerInputPressFlags.Grip, OnHeldInput);
    }

    public override void Deactivate()
    {
        ControllerInputPoller.RemoveCallbackOnPressStart(OnGrabInput); 
        ControllerInputPoller.RemoveCallbackOnPressEnd(OnReleaseInput); 
        ControllerInputPoller.RemoveCallbackOnPressUpdate(OnHeldInput);
        
        foreach (var obj in _activeSegments)
            _pool.Return(obj);
        
        _activeSegments.Clear();
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

    private void OnHeldInput(EHandednessFlags hand)
    {
        if ((hand & EHandednessFlags.Left) != 0) OnHeld(true);
        if ((hand & EHandednessFlags.Right) != 0) OnHeld(false);
    }

    private void OnGrab(bool isLeftHand)
    {
        var hand = isLeftHand ? 
            SpellRuntime.Context.Player.leftHand : 
            SpellRuntime.Context.Player.rightHand;
        
        var handPos = hand.GetCurrentHandPosition();
        if (IsNearWand(handPos)) 
            return; 

        if (isLeftHand) { _isDrawingLeft = true; _lastSpawnPosLeft = handPos; }
        else { _isDrawingRight = true; _lastSpawnPosRight = handPos; }

        SpawnSegment(hand.controllerTransform.rotation, handPos);
    }
    
    private void OnHeld(bool isLeftHand)
    {
        var drawing = isLeftHand ? _isDrawingLeft : _isDrawingRight;
        if (!drawing) return;

        var hand = isLeftHand ? SpellRuntime.Context.Player.leftHand : SpellRuntime.Context.Player.rightHand;
        var handPos = hand.GetCurrentHandPosition();
        var lastPos = isLeftHand ? _lastSpawnPosLeft : _lastSpawnPosRight;

        var delta = handPos - lastPos;
        var dist = delta.magnitude;
        if (dist < SpawnSpacing) return;

        var dir = delta / dist;
        var steps = Mathf.FloorToInt(dist / SpawnSpacing);

        for (var i = 1; i <= steps; i++)
        {
            var pos = lastPos + dir * (SpawnSpacing * i);
            SpawnSegment(hand.controllerTransform.rotation, pos);
        }

        var newLast = lastPos + dir * (SpawnSpacing * steps);
        if (isLeftHand) _lastSpawnPosLeft = newLast;
        else _lastSpawnPosRight = newLast;
    }
    
    private void OnRelease(bool isLeftHand)
    {
        if (isLeftHand) _isDrawingLeft = false;
        else _isDrawingRight = false;
    }
    
    private void SpawnSegment(Quaternion handRot, Vector3 handPos)
    {
        var obj = _pool.Get(handPos + _positionOffset, handRot);
        _activeSegments.Add(obj);
    }
}