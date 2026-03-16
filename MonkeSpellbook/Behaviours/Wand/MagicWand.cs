using System;
using MonkeSpellbook.Behaviours.Gestures;
using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.XR;

namespace MonkeSpellbook.Behaviours.Wand;

public class MagicWand : GrabbableObject
{
    public Transform WandTip { get; private set; }
    public Collider WandCollider => GetComponent<Collider>();
    
    private GestureTracker _gestureTracker;
    private GestureRecognizer _gestureRecognizer;
    
    public GestureRecognizer GestureRecognizer => _gestureRecognizer;
    
    public event Action<Result> OnGestureRecognized;
    
    public bool creationMode = false;
    public string newGestureName = string.Empty;
    
    protected override void Awake()
    {
        base.Awake();

        transform.position = new Vector3(-66.0177f, 11.64f, -82.4992f);
        
        SetupMaterial();
        SetupGrabPoint();
        SetupDropPoint();
        FindWandTip();

        SetupGestureTracking();
    }

    private void SetupMaterial()
    {
        var mr = transform.Find("WandModel")?.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            Plugin.Log.LogError("Failed to get MeshRenderer from WandModel!");
            return;
        }
        
        var texture2D = mr.material.mainTexture;
        var wandMaterial = new Material(UberShader.GetShader());
        wandMaterial.EnableKeyword("_USE_TEXTURE");
        wandMaterial.DisableKeyword("USE_TEXTURE__AS_MASK");
        wandMaterial.SetInt("_ColorSource", 1);
        wandMaterial.mainTexture = texture2D;

        mr.material = wandMaterial;
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
            _gestureTracker.StopGesture();

            var points = _gestureTracker.GetTrackedPoints();
            var gesture = new Gesture(points);

            if (creationMode)
            {
                gesture.Name = newGestureName;
                _gestureRecognizer.ExportGesture(newGestureName, gesture);
            }
            else
            {
                Result result = _gestureRecognizer.Recognize(gesture);
                Plugin.Log.LogInfo(result.GestureClass + " " + result.Score);
                OnGestureRecognized?.Invoke(result);
            }
            
            return;
        }

        if (_gestureTracker.isActive)
        {
            _gestureTracker.UpdateGesture();   
        }
    }
    
    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(10, 10, 250, 150), "Gesture Creator", GUI.skin.window);

        creationMode = GUI.Toggle(new Rect(10, 25, 200, 25), creationMode, "Creation Mode");

        GUI.Label(new Rect(10, 55, 200, 20), "New Gesture Name:");
        newGestureName = GUI.TextField(new Rect(10, 75, 200, 25), newGestureName);

        GUI.EndGroup();
    }
}