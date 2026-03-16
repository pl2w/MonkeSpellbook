using System;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;
using GorillaLocomotion;
using MonkeSpellbook.Behaviours;
using MonkeSpellbook.Behaviours.Spellbook;
using MonkeSpellbook.Behaviours.Wand;
using MonkeSpellbook.Systems;
using UnityEngine;

namespace MonkeSpellbook;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    
    private GameObject _wand, _book;
    
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
            Logger.LogWarning("Monke Spellbook has already been initialized...");
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
            
            AssetLoader.SetDefaultBundle(bundle);
            
            _wand = Instantiate(AssetLoader.LoadAsset<GameObject>("Wand"));
            var wandComp = _wand.AddComponent<MagicWand>();
            SpellRuntime.Context = new SpellContext(
                wandComp.WandCollider,
                wandComp.WandTip,
                GTPlayer.Instance,
                wandComp
            );
            
            _book = new GameObject("SpellBook");
            var spellbookComp = _book.AddComponent<Spellbook>();

            wandComp.OnGestureRecognized += spellbookComp.HandleGesture;
            
            _initialized = true;
            Logger.LogInfo("MonkeSpellbook initialized successfully.");
        }
        catch (Exception e)
        {
            Logger.LogError($"Initialization failed: {e}");
        }
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.monkespellbook";
    public const string Name = "MonkeSpellbook";
    public const string Version = "0.1.0";
}