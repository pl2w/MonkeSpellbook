using System.Collections.Generic;
using MonkeSpellbook.Systems;
using PDollarGestureRecognizer;
using UnityEngine;

namespace MonkeSpellbook.Behaviours.Gestures;

public class GestureTracker : MonoBehaviour
{
    public List<Vector3> positions = [];
    
    public bool isActive;
    public float newPositionThreshold = 0.01f;
    
    public void StartGesture()
    {
        isActive = true;
        
        positions.Clear();
        positions.Add(SpellRuntime.Context.WandTip.position);
    }

    public void StopGesture()
    {
        isActive = false;
    }

    public void UpdateGesture()
    {
        if (!isActive || positions.Count == 0) 
            return;

        var wandPos = SpellRuntime.Context.WandTip.position;
        var lastPosition = positions[^1];
    
        if (Vector3.Distance(wandPos, lastPosition) > newPositionThreshold)
            positions.Add(wandPos);
    }
    
    public Point[] PositionsToPointArray(Vector3[] positionsList)
    {
        if (positionsList == null || positionsList.Length == 0)
            return [];
        
        var points = new Point[positionsList.Length];
        Camera cam = Camera.main;
        
        for (int i = 0; i < positionsList.Length; i++)
        {
            Vector2 screenPoint = cam.WorldToScreenPoint(positionsList[i]);
            points[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }
        return points;
    }
    
    public Point[] GetTrackedPoints()
    {
        return PositionsToPointArray(positions.ToArray());
    }
}