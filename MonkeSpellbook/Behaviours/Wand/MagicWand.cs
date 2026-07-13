using System;
using MonkeSpellbook.Behaviours.Gestures;
using PDollarGestureRecognizer;
using UnityEngine;

namespace MonkeSpellbook.Behaviours.Wand;

public class MagicWand : GrabbableObject
{
    public Transform WandTip { get; private set; }
    public Collider WandCollider => GetComponent<Collider>();
    
    private GestureTracker _gestureTracker;
    private GestureRecognizer _gestureRecognizer;
    private GestureCreatorUI _creatorUI;
    
    public GestureRecognizer GestureRecognizer => _gestureRecognizer;
    
    public event Action<Result> OnGestureRecognized;
    
    protected override void Awake()
    {
        base.Awake();

        transform.position = new Vector3(-66.0177f, 11.64f, -82.4992f);
        
        SetupGrabPoint();
        SetupDropPoint();
        FindWandTip();

        SetupGestureTracking();
    }

    private void SetupGrabPoint()
    {
        grabPoint = transform.Find("GrabPoint");
        if (grabPoint == null)
        {
            Plugin.Log.LogError("Failed to find GrabPoint!");
        }
    }

    private void SetupDropPoint()
    {
        dropPoint = new GameObject("DropPoint_Wand").transform;

        var playerPivot = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body_pivot/")?.transform;
        if (playerPivot != null)
        {
            dropPoint.SetParent(playerPivot);
            dropPoint.localPosition = Vector3.forward * 0.5f;
            dropPoint.localRotation = Quaternion.identity;
            dropPoint.localScale = Vector3.one;
        }
        else
        {
            Plugin.Log.LogWarning("Failed to find player pivot for DropPoint!");
        }
    }
    
    private void FindWandTip()
    {
        WandTip = transform.Find("WandTip");
        if (WandTip == null)
        {
            Plugin.Log.LogWarning("Failed to find WandTip!");
        }
    }

    private void SetupGestureTracking()
    {
        _gestureTracker = gameObject.AddComponent<GestureTracker>();
        _gestureRecognizer = new GestureRecognizer();
        _creatorUI = gameObject.AddComponent<GestureCreatorUI>();
    }

    protected override void Update()
    {
        base.Update();
        
        if(!isInHand)
            return;

        var isPressed = isInLeftHand
            ? ControllerInputPoller.instance.leftIndexPressedThisFrame
            : ControllerInputPoller.instance.rightIndexPressedThisFrame;
        
        var isReleased = isInLeftHand
            ? ControllerInputPoller.instance.leftIndexReleasedThisFrame
            : ControllerInputPoller.instance.rightIndexReleasedThisFrame;

        if (isPressed)
        {
            _gestureTracker.StartGesture();
            return;
        }

        if (isReleased)
        {
            FinishGesture();
            return;
        }

        if (_gestureTracker.isActive)
        {
            _gestureTracker.UpdateGesture();   
        }
    }
    
    public override void OnDrop(bool isLeft)
    {
        if (_gestureTracker.isActive)
        {
            _gestureTracker.StopGesture();
        }
    }

    private void FinishGesture()
    {
        _gestureTracker.StopGesture();

        var points = _gestureTracker.GetTrackedPoints();
        var gesture = new Gesture(points);

        if (_creatorUI.CreationMode)
        {
            gesture.Name = _creatorUI.NewGestureName;
            _gestureRecognizer.ExportGesture(_creatorUI.NewGestureName, gesture);
        }
        else
        {
            var result = _gestureRecognizer.Recognize(gesture);
            Plugin.Log.LogInfo(result.GestureClass + " " + result.Score);
            OnGestureRecognized?.Invoke(result);
        }
    }
}