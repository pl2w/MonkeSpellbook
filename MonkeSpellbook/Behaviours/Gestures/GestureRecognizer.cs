using System.Collections.Generic;
using System.IO;
using PDollarGestureRecognizer;

namespace MonkeSpellbook.Behaviours.Gestures;

public class GestureRecognizer
{
    private readonly List<Gesture> _gestureSet = [];

    public GestureRecognizer()
    {
        LoadGestures();
    }
    
    public Result Recognize(Gesture gesture)
    {
        return PointCloudRecognizer.Classify(gesture, _gestureSet.ToArray());
    }

    private void AddGesture(Gesture gesture)
    {
        _gestureSet.Add(gesture);
    }

    private void LoadGestures()
    {
        var path = Path.Combine(BepInEx.Paths.PluginPath, "MonkeSpellbook", "SpellGestures");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var xmlFiles = Directory.GetFiles(path, "*.xml");
        foreach (var xmlFile in xmlFiles)
        {
            var gesture = GestureIO.ReadGestureFromFile(xmlFile);
            
            Plugin.Log.LogDebug(gesture.Name);
            
            AddGesture(gesture);
        }
    }

    public void ExportGesture(string gestureName, Gesture gesture)
    {
        var path = Path.Combine(BepInEx.Paths.PluginPath, "MonkeSpellbook", "SpellGestures");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        var filePath = Path.Combine(path, $"{gestureName}.xml");
        
        GestureIO.WriteGesture(gesture.Points, gestureName, filePath);
    }
}