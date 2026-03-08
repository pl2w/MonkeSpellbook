using System.Reflection;
using UnityEngine;

namespace MonkeSpellbook.Behaviours;

public static class AssetLoader
{
    public static AssetBundle LoadBundle(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null)
        {
            Debug.LogError($"Embedded resource not found: {resourcePath}");
            return null;
        }

        return AssetBundle.LoadFromStream(stream);
    }
}