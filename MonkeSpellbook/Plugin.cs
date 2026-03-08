using System;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using MonkeSpellbook.Behaviours;
using MonkeSpellbook.Behaviours.Wand;
using UnityEngine;

namespace MonkeSpellbook;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    private GameObject _book, _wand;
    
    public Plugin()
    {
        var harmony = new Harmony(PluginInfo.Guid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        
        GorillaTagger.OnPlayerSpawned(Initialise);
    }

    public void Initialise()
    {
        try
        {
            var bundle = AssetLoader.LoadBundle("MonkeSpellbook.Resources.monkespellbook");
            
            _wand = Instantiate(bundle.LoadAsset<GameObject>("Wand"), GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            _wand.transform.localPosition = Vector3.zero;
            _wand.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            
            _wand.AddComponent<MagicWand>();
            
            var mr = _wand.transform.Find("WandModel").gameObject.GetComponent<MeshRenderer>();
            var texture2D = mr.material.mainTexture;
        
            mr.material = new Material(UberShader.GetShader());
            mr.material.EnableKeyword("_USE_TEXTURE");
            mr.material.DisableKeyword("USE_TEXTURE__AS_MASK");
            mr.material.SetInt("_ColorSource", 1);
            mr.material.mainTexture = texture2D;
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.monkespellbook";
    public const string Name = "MonkeSpellbook";
    public const string Version = "0.1.0";
}