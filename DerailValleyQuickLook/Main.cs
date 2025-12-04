using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine;

namespace DerailValleyQuickLook;

#if DEBUG
[EnableReloading]
#endif
public static class Main
{
    public static QuickLookManager quickLookManager;
    public static UnityModManager.ModEntry ModEntry;
    public static Settings settings;
    public static GameObject updateDriverGO;
    public static bool hasAddedBinding = false;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        ModEntry = modEntry;

        Harmony? harmony = null;
        try
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            updateDriverGO = new GameObject("DerailValleyQuickLook_UpdateDriver");
            UnityEngine.Object.DontDestroyOnLoad(updateDriverGO);

            quickLookManager = new QuickLookManager();

            var updateDriver = updateDriverGO.AddComponent<UpdateDriver>();
            updateDriver.OnFrame = quickLookManager.OnFrame;

            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ModEntry.Logger.Log("DerailValleyQuickLook started");
        }
        catch (Exception ex)
        {
            ModEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
            harmony?.UnpatchAll(modEntry.Info.Id);
            return false;
        }

        modEntry.OnUnload = Unload;
        return true;
    }

    static void OnGUI(UnityModManager.ModEntry modEntry)
    {
        settings.Draw();
    }

    static void OnSaveGUI(UnityModManager.ModEntry modEntry)
    {
        settings.Save(modEntry);
    }

    private static bool Unload(UnityModManager.ModEntry modEntry)
    {
        if (updateDriverGO != null)
            GameObject.Destroy(updateDriverGO);

        quickLookManager?.Unload();

        ModEntry.Logger.Log("DerailValleyQuickLook stopped");
        return true;
    }
}
