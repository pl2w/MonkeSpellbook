using System.Collections;
using GorillaLocomotion;
using UnityEngine;

namespace MonkeSpellbook.Behaviours;

public class GrabbableObject : HoldableObject
{
    public bool isInHand; 
    public bool isInLeftHand;
    public bool canBePickedUp = true;
    public bool returnToDropPointOnDrop = false;
    public float returnSpeed = 5f;

    public Transform grabPoint;
    public Transform dropPoint;
    
    private bool _didSwap;
    private Collider _collider;
    private Coroutine _returnRoutine;
    
    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        
        gameObject.layer = 18;
    }
    
    protected virtual void Update()
    {
        if (!_collider) 
            return;
        
        var leftGrip = ControllerInputPoller.instance.leftGrab;
        var rightGrip = ControllerInputPoller.instance.rightGrab;

        if (_didSwap && !leftGrip && !rightGrip) 
            _didSwap = false;

        HandleHandInteraction(true, leftGrip, rightGrip);
        HandleHandInteraction(false, rightGrip, leftGrip);
    }

    private void HandleHandInteraction(bool isLeft, bool gripActive, bool otherGripActive)
    {
        Vector3 handPos = isLeft 
            ? GTPlayer.Instance.LeftHand.controllerTransform.position 
            : GTPlayer.Instance.RightHand.controllerTransform.position;

        bool isInside = _collider.bounds.Contains(handPos);
        bool currentlyHoldingThisHand = isInHand && isInLeftHand == isLeft;
        var currentHeldItem = 
            isLeft ? 
            EquipmentInteractor.instance.leftHandHeldEquipment : 
            EquipmentInteractor.instance.rightHandHeldEquipment;

        if (currentlyHoldingThisHand && (!gripActive || !canBePickedUp))
        {
            Detach(isLeft);
            return;
        }

        if (canBePickedUp && gripActive && isInside && !_didSwap && currentHeldItem == null)
        {
            if (isInHand && isInLeftHand != isLeft)
            {
                _didSwap = true;
                Detach(!isLeft);
            }

            Attach(isLeft);
        }
    }
    
    private void Attach(bool isLeft)
    {
        isInHand = true;
        isInLeftHand = isLeft;
            
        var handParent = isLeft 
            ? GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent 
            : GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent;

        transform.SetParent(handParent, true);
        GorillaTagger.Instance.StartVibration(isLeft, 0.07f, 0.07f);

        if (isLeft) EquipmentInteractor.instance.leftHandHeldEquipment = this;
        else EquipmentInteractor.instance.rightHandHeldEquipment = this;

        if (grabPoint)
        {
            transform.localRotation = Quaternion.Inverse(grabPoint.localRotation);
            transform.localPosition = transform.localRotation * -grabPoint.localPosition;
        }
        
        OnGrab(isLeft);
    }
    
    private void Detach(bool isLeft)
    {
        isInHand = false;
        transform.SetParent(null, true);

        if (_returnRoutine != null) 
            StopCoroutine(_returnRoutine);

        if (returnToDropPointOnDrop && dropPoint)
        {
            _returnRoutine = StartCoroutine(ReturnToHome());
        }
        
        if (isLeft) EquipmentInteractor.instance.leftHandHeldEquipment = null;
        else EquipmentInteractor.instance.rightHandHeldEquipment = null;
        
        OnDrop(isLeft);
    }
    
    private IEnumerator ReturnToHome()
    {
        while (Vector3.Distance(transform.position, dropPoint.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, dropPoint.position, Time.deltaTime * returnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, dropPoint.rotation, Time.deltaTime * returnSpeed);
            yield return null;
        }
    
        transform.position = dropPoint.position;
        transform.rotation = dropPoint.rotation;
        
        transform.SetParent(dropPoint);
        
        _returnRoutine = null;
    }
    
    public virtual void OnGrab(bool isLeft) { }
    public virtual void OnDrop(bool isLeft) { }
    
    public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand) {}
    public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand) {}
    public override void DropItemCleanup() {}
}