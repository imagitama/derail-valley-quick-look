using System;
using System.Collections.Generic;
using System.Linq;
using DV.Interaction.Inputs;
using Rewired;

namespace DerailValleyQuickLook;

public static class Actions
{
    public static int QuickLookDown = 1;
}

[Serializable]
public struct BindingInfo
{
    public ControllerType ControllerType;
    public string? ControllerName;
    public int ControllerId;
    public string ButtonName;
    public int ButtonId;
    public int ActionId;
}

public static class BindingsHelper
{
    public static bool IsReady => ReInput.isReady;

    public static int GetButtonId(ControllerType controllerType, int controllerId, string buttonName)
    {
        var player = InputManager.NewPlayer;
        Controller controller = player.controllers.GetController(controllerType, controllerId);

        // TODO: cache this
        var result = controller.ButtonElementIdentifiers.ToList().Find(x => x.name == buttonName);

        if (result == null)
            return -1;

        return result.id;
    }

    public static bool GetIsPressed(ControllerType controllerType, int controllerId, int buttonId)
    {
        var player = InputManager.NewPlayer;
        Controller controller = player.controllers.GetController(controllerType, controllerId);

        var pressed = controller.GetButtonById(buttonId);
        return pressed;
    }

    public static bool GetIsPressed(ControllerType controllerType, int controllerId, string buttonName)
    {
        var player = InputManager.NewPlayer;

        // TODO: cache this
        Controller controller = player.controllers.GetController(controllerType, controllerId);

        var buttonId = GetButtonId(controllerType, controllerId, buttonName);

        var pressed = controller.GetButtonById(buttonId);
        return pressed;
    }

    public static bool GetIsPressed(int actionId)
    {
        var bindings = Main.settings.Bindings;

        // TODO: more performant way of doing this
        var bindingsForAction = bindings.Where(binding => binding.ActionId == actionId);

        foreach (var binding in bindingsForAction)
        {
            var player = InputManager.NewPlayer;

            // TODO: cache this
            Controller controller = player.controllers.GetController(binding.ControllerType, binding.ControllerId);

            var pressed = controller.GetButtonById(binding.ButtonId);

            if (pressed)
                return true;
        }

        return false;
    }

    public static (string buttonName, int buttonId)? GetAnyButtonPressedInfo(ControllerType controllerType)
    {
        var controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(controllerType, 0);

        if (!controllerPollingInfo.success)
            return null;

        return (controllerPollingInfo.elementIdentifierName, controllerPollingInfo.elementIdentifierId);
    }

    public static string GetControllerNameFromType(ControllerType controllerType)
    {
        return InputManager.NewPlayer.controllers.Controllers.ToList().Find(x => x.type == controllerType).name;
    }

    public static List<Controller> GetAllControllers() => InputManager.NewPlayer.controllers.Controllers.ToList();
}