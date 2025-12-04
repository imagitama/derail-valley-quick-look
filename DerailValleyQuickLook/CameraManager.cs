using UnityEngine;
using UnityModManagerNet;

namespace DerailValleyQuickLook;

public class CameraManager
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;

    public bool IsReady => PlayerManager.PlayerTransform != null;
    public Quaternion? TargetRotation = null;
    private CustomFirstPersonController? _firstPersonController;
    private static Transform Character => PlayerManager.PlayerTransform;
    private static Transform CameraTransform => PlayerManager.PlayerCamera.transform;

    public bool GetIsLookingDown()
    {
        var pitch = CameraTransform.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f; // convert 0–360 to -180–180

        var minDown = Main.settings.RotationAmount - Main.settings.MinRotationForDownOffset;
        var maxDown = 90f;

        var isLookingDown = pitch >= minDown && pitch <= maxDown;

        Logger.Log($"Looking down={isLookingDown} pitch={pitch}");

        return isLookingDown;
    }

    public void QuickLookDown()
    {
        var playerCamera = PlayerManager.PlayerCamera;

        if (playerCamera == null)
        {
            Logger.Log("No player camera");
            return;
        }

        Logger.Log("Quick look down");

        float yaw = Character.rotation.eulerAngles.y;
        float currentPitch = CameraTransform.localEulerAngles.x;
        float newPitch = currentPitch + Main.settings.RotationAmount;

        TargetRotation = Quaternion.Euler(newPitch, yaw, 0f);
    }

    public Quaternion GetCurrentRotation()
    {
        float pitch = CameraTransform.localEulerAngles.x;
        float yaw = Character.rotation.eulerAngles.y;
        return Quaternion.Euler(pitch, yaw, 0f);
    }

    public void OnReachedTarget()
    {
        Logger.Log("Reached target");
        TargetRotation = null;
    }

    public void SetCameraRotation(Quaternion targetRotation)
    {
        if (_firstPersonController == null)
            _firstPersonController = PlayerManager.PlayerTransform.GetComponent<CustomFirstPersonController>();

        _firstPersonController.m_MouseLook.ForceRotationNoTilt(
            Character,
            CameraTransform,
            targetRotation
        );
    }

    public void StopQuickLookDown()
    {
        Logger.Log("Stop quick look down");

        float yaw = Character.rotation.eulerAngles.y;
        float currentPitch = CameraTransform.localEulerAngles.x;
        float newPitch = currentPitch - Main.settings.RotationAmount;

        TargetRotation = Quaternion.Euler(newPitch, yaw, 0f);
    }

    /// <summary>
    /// Called on mod unload and if user moves their mouse during quick look.
    /// </summary>
    public void Reset()
    {
        Logger.Log("CameraManager.Reset");

        TargetRotation = null;
    }
}