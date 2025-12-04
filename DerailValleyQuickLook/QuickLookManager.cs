using DV.Interaction.Inputs;
using UnityEngine;
using UnityModManagerNet;

namespace DerailValleyQuickLook;

public class QuickLookManager
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;
    public CameraManager cameraManager;
    private bool _isPressed;
    private bool _wasPressed;

    public QuickLookManager()
    {
        cameraManager = new CameraManager();
    }

    public void OnFrame()
    {
        if (!BindingsHelper.IsReady || UnityModManager.UI.Instance.Opened || !cameraManager.IsReady)
            return;

        _isPressed = BindingsHelper.GetIsPressed(Actions.QuickLookDown);

        if (_isPressed && !_wasPressed)
        {
            if (cameraManager.GetIsLookingDown())
                cameraManager.StopQuickLookDown();
            else
                cameraManager.QuickLookDown();
        }

        _wasPressed = _isPressed;

        if (cameraManager.TargetRotation.HasValue)
        {
            var magnitude = InputManager.GetMouseAxisInput().magnitude;
            if (magnitude > 0.5f)
            {
                Logger.Log($"User has moved mouse: {magnitude}");
                cameraManager.Reset();
                return;
            }

            var current = cameraManager.GetCurrentRotation();
            var target = cameraManager.TargetRotation.Value;

            cameraManager.SetCameraRotation(
                Quaternion.Slerp(current, target, Main.settings.Speed * Time.deltaTime)
            );

            if (Quaternion.Angle(current, target) < 2f) // any lower and it takes too long to detect so feels sluggish
            {
                cameraManager.OnReachedTarget();
            }
        }
    }

    public void Unload()
    {
        Logger.Log("QuickLookManager unload");
        cameraManager.Reset();
    }
}