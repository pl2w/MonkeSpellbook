using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonkeSpellbook.Behaviours;

public static class AssetLoader
{
    private static AssetBundle _defaultBundle;
    
    public static AssetBundle LoadBundle(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null)
        {
            Plugin.Log.LogError($"Embedded resource not found: {resourcePath}");
            return null;
        }

        var bundle = AssetBundle.LoadFromStream(stream);
        if (bundle == null)
        {
            Plugin.Log.LogError($"Failed to load AssetBundle from resource: {resourcePath}");
        }

        return bundle;
    }

    public static T LoadAsset<T>(string assetName) where T : Object
    {
        if (_defaultBundle == null)
            throw new InvalidOperationException("Default AssetBundle not set. Call SetDefaultBundle first.");
        
        var asset = _defaultBundle.LoadAsset<T>(assetName);
        if (asset == null)
        {
            Plugin.Log.LogError($"Asset '{assetName}' of type {typeof(T)} not found in bundle.");
        }

        return asset;
    }

    public static void SetDefaultBundle(AssetBundle bundle)
    {
        if (bundle == null)
        {
            Plugin.Log.LogError("Attempted to set default AssetBundle to null.");
            return;
        }
        
        _defaultBundle  = bundle;
    }
}