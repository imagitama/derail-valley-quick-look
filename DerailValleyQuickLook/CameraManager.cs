using UnityEngine;
using UnityModManagerNet;

namespace DerailValleyQuickLook;

public class CameraManager
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;

    public bool IsReady => PlayerManager.PlayerTransform != null;
    public bool isQuickLookingDown = false;
    public Quaternion? TargetRotation = null;
    private Quaternion _originalRotation;
    private CustomFirstPersonController? _firstPersonController;
    private static Transform Character => PlayerManager.PlayerTransform;
    private static Transform CameraTransform => PlayerManager.PlayerCamera.transform;

    public void QuickLookDown()
    {
        var playerCamera = PlayerManager.PlayerCamera;

        if (playerCamera == null)
        {
            Logger.Log("No player camera");
            return;
        }

        Logger.Log("Quick look down");

        float pitch = CameraTransform.localEulerAngles.x;
        float yaw = Character.rotation.eulerAngles.y;

        _originalRotation = Quaternion.Euler(pitch, yaw, 0f);

        float currentPitch = CameraTransform.localEulerAngles.x;
        float newPitch = currentPitch + Main.settings.RotationAmount;

        TargetRotation = Quaternion.Euler(newPitch, yaw, 0f);

        isQuickLookingDown = true;
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
        if (!isQuickLookingDown)
            return;

        Logger.Log("Stop quick look down");

        TargetRotation = _originalRotation;

        isQuickLookingDown = false;
    }

    /// <summary>
    /// Called on mod unload and if user moves their mouse during quick look.
    /// </summary>
    public void Reset()
    {
        Logger.Log("CameraManager.Reset");

        TargetRotation = null;
        isQuickLookingDown = false;
    }
}