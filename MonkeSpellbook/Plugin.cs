using System;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;
using MonkeSpellbook.Behaviours;
using MonkeSpellbook.Behaviours.Wand;
using UnityEngine;

namespace MonkeSpellbook;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    
    private GameObject _book, _wand;
    private bool _initialized;
    
    public Plugin()
    {
        Log = Logger;

        var harmony = new Harmony(PluginInfo.Guid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        
        GorillaTagger.OnPlayerSpawned(Initialise);
    }

    public void Initialise()
    {
        if (_initialized)
        {
            Logger.LogWarning("Monke Spellbook has already been initialized!");
            return;
        }
        
        try
        {
            var bundle = AssetLoader.LoadBundle("MonkeSpellbook.Resources.monkespellbook");
            if (bundle == null)
            {
                Logger.LogError("Failed to load AssetBundle!");
                return;
            }
            
            _wand = Instantiate(bundle.LoadAsset<GameObject>("Wand"));
            _wand.transform.position = new Vector3(-65.9547f, 11.8385f, -82.9238f);
            
            _wand.AddComponent<MagicWand>();
            
            var mr = _wand.transform.Find("WandModel")?.gameObject.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                Logger.LogError("Failed to get MeshRenderer from WandModel!");
                return;
            }
            
            var texture2D = mr.material.mainTexture;
        
            mr.material = new Material(UberShader.GetShader());
            mr.material.EnableKeyword("_USE_TEXTURE");
            mr.material.DisableKeyword("USE_TEXTURE__AS_MASK");
            mr.material.SetInt("_ColorSource", 1);
            mr.material.mainTexture = texture2D;
            
            _initialized = true;
            Logger.LogInfo("MonkeSpellbook initialized successfully.");
        }
        catch (Exception e)
        {
            Logger.LogError($"Initialization failed: {e.Message}");
        }
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.monkespellbook";
    public const string Name = "MonkeSpellbook";
    public const string Version = "0.1.0";
}