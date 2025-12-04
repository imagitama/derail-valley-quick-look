using UnityEngine;
using System;
using UnityModManagerNet;

namespace DerailValleyQuickLook;

public class UpdateDriver : MonoBehaviour
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;

    public Action? OnFrame;

    public void Start()
    {
        Main.ModEntry.Logger.Log($"UpdateDriver started");
    }

    public void Update()
    {
        OnFrame?.Invoke();
    }

    public void OnDisable()
    {
        Logger.Log($"UpdateDriver disabled");
    }

    public void OnDestroy()
    {
        Logger.Log($"UpdateDriver destroyed");
    }
}
