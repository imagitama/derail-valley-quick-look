using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityModManagerNet;

namespace DerailValleyQuickLook;

[Serializable]
public class Settings : UnityModManager.ModSettings, IDrawable
{
    private static UnityModManager.ModEntry.ModLogger Logger => Main.ModEntry.Logger;

    public float RotationAmount = 45f;
    public float MinRotationForDownOffset = 10f;
    public float Speed = 10f;
    public List<BindingInfo> Bindings = new List<BindingInfo>();

    public override void Save(UnityModManager.ModEntry modEntry)
    {
        Save(this, modEntry);
    }

    private bool _wasPressed = false;
    private float _timer = 0;
    private int? _bindingIndexRecording;

    public void Draw()
    {
        UnityModManager.UI.DrawFloatField(ref RotationAmount, "Rotation amount (degrees, default 45)");
        UnityModManager.UI.DrawFloatField(ref MinRotationForDownOffset, $"When looking up it checks you are looking {RotationAmount} degrees down minus this offset (degrees, default 10)");
        UnityModManager.UI.DrawFloatField(ref Speed, "Speed (higher faster, default 10)");

        var controllers = BindingsHelper.GetAllControllers();

        if (controllers == null)
            return;

        for (var i = 0; i < Bindings.Count; i++)
        {
            var binding = Bindings[i];
            binding.ActionId = Actions.QuickLookDown;

            GUILayout.Label($"Controller: {binding.ControllerName} ({binding.ControllerType})");

            ControllerType newControllerType = binding.ControllerType;
            string? newControllerName = binding.ControllerName;
            int newControllerId = binding.ControllerId;

            foreach (var controller in controllers)
            {
                // TODO: why is controller ID always 0
                var isNowChecked = GUILayout.Toggle(binding.ControllerName == controller.name, controller.name);

                if (isNowChecked && binding.ControllerName != controller.name)
                {
                    Logger.Log($"Binding select controller {binding.ControllerName} => {controller.name}");
                    newControllerType = controller.type;
                    newControllerName = controller.name;
                    newControllerId = controller.id;
                }
            }

            GUILayout.Label($"Button name: {binding.ButtonName} ({binding.ButtonId})");
            var newButtonName = GUILayout.TextField(binding.ButtonName);
            var newButtonId = BindingsHelper.GetButtonId(newControllerType, newControllerId, newButtonName);

            if (_bindingIndexRecording == i)
            {
                if (GUILayout.Button("Stop Recording"))
                {
                    _bindingIndexRecording = null;
                }

                // TODO: have a timer to cancel like every binding system has?
                GUILayout.Label("Waiting for a button press...");

                var result = BindingsHelper.GetAnyButtonPressedInfo(newControllerType);

                if (result != null)
                {
                    var (pressedButtonName, pressedButtonId) = result.Value;
                    Logger.Log($"User pressed controllerType={newControllerType} controllerName={newControllerName} name={pressedButtonName} id={pressedButtonId}");
                    newButtonName = pressedButtonName;
                    newButtonId = pressedButtonId;

                    _bindingIndexRecording = null;
                }
            }
            else
            {
                if (GUILayout.Button("Record"))
                {
                    _bindingIndexRecording = i;
                }
            }

            binding.ControllerType = newControllerType;
            binding.ControllerName = newControllerName;
            binding.ControllerId = newControllerId;
            binding.ButtonName = newButtonName;
            binding.ButtonId = newButtonId;

            Bindings[i] = binding;

            if (BindingsHelper.GetIsPressed(binding.ControllerType, binding.ControllerId, binding.ButtonId))
            {
                _wasPressed = true;
                _timer = Time.time + 0.5f;
            }

            if (_wasPressed && Time.time >= _timer)
            {
                _wasPressed = false;
            }

            if (_wasPressed)
            {
                GUILayout.Label("Pressed!");
            }

            if (GUILayout.Button("Remove Binding"))
                Bindings.RemoveAt(i);

            GUILayout.Label("");
        }

        if (GUILayout.Button("Add Binding"))
        {
            Bindings.Add(
                new BindingInfo()
                {
                    ActionId = Actions.QuickLookDown,
                    ControllerId = 0,
                    ControllerType = ControllerType.Keyboard,
                    ControllerName = BindingsHelper.GetControllerNameFromType(ControllerType.Keyboard) ?? "",
                    ButtonName = "Space",
                    ButtonId = BindingsHelper.GetButtonId(ControllerType.Keyboard, 0, "Space")
                }
            );
        }
    }

    public void OnChange()
    {
    }
}
