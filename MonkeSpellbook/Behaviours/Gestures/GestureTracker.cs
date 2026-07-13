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

    public float fadeOutDuration = 0.25f;
    
    public ParticleSystem trailParticles;
    private ParticleSystem.Particle[] _particleBuffer;

    public void Awake()
    {
        trailParticles = transform.Find("WandTip").GetComponent<ParticleSystem>();

        var main = trailParticles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = 0f;
        main.startLifetime = 999999f;
        main.maxParticles = Mathf.Max(main.maxParticles, 10000);

        var emission = trailParticles.emission;
        emission.rateOverTime = 0f;
        emission.rateOverDistance = 0f;
        
        var colorOverLifetime = trailParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var alphaGradient = new Gradient();
        alphaGradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = alphaGradient;

        _particleBuffer = new ParticleSystem.Particle[main.maxParticles];
    }

    public void StartGesture()
    {
        isActive = true;

        positions.Clear();

        var startPos = SpellRuntime.Context.WandTip.position;
        positions.Add(startPos);

        EmitAt(startPos);
    }

    public void StopGesture()
    {
        if (!isActive)
            return;
        
        isActive = false;
        BeginFadeOut();
    }
    
    public void UpdateGesture()
    {
        if (!isActive || positions.Count == 0) 
            return;

        var wandPos = SpellRuntime.Context.WandTip.position;
        var lastPosition = positions[^1];
    
        if (Vector3.Distance(wandPos, lastPosition) > newPositionThreshold)
        {
            positions.Add(wandPos);
            EmitAt(wandPos);
        }
    }

    private void EmitAt(Vector3 worldPos)
    {
        if (!trailParticles) 
            return;

        var emitParams = new ParticleSystem.EmitParams
        {
            position = worldPos,
            applyShapeToPosition = false
        };
        trailParticles.Emit(emitParams, 1);
    }
    
    private void BeginFadeOut()
    {
        if (!trailParticles) 
            return;

        var count = trailParticles.GetParticles(_particleBuffer);
        if (count == 0) return;

        for (var i = 0; i < count; i++)
        {
            _particleBuffer[i].startLifetime = fadeOutDuration;
            _particleBuffer[i].remainingLifetime = fadeOutDuration;
        }

        trailParticles.SetParticles(_particleBuffer, count);
    }

    private Point[] PositionsToPointArray(Vector3[] positionsList)
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