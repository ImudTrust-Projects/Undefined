using System;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace Undefined.Utilities;

public enum InputType
{
    RightPrimary,
    RightSecondary,
    RightTrigger,
    RightGrip,
    LeftPrimary,
    LeftSecondary,
    LeftTrigger,
    LeftGrip,
}

public class InputHandler : Singleton<InputHandler>
{
    public ControllerJoystick LeftJoystick, RightJoystick;
    public ControllerButton LeftPrimary, LeftSecondary, LeftTrigger, LeftGrip;

    public ControllerButton RightPrimary, RightSecondary, RightTrigger, RightGrip;

    private void Update()
    {
        if (ControllerInputPoller.instance == null)
            return;

        HandleInput(ref RightPrimary, ControllerInputPoller.instance.rightControllerPrimaryButton);
        HandleInput(ref RightSecondary, ControllerInputPoller.instance.rightControllerSecondaryButton);
        HandleInput(ref RightTrigger, ControllerInputPoller.instance.rightControllerTriggerButton);
        HandleInput(ref RightGrip, ControllerInputPoller.instance.rightGrab);

        HandleInput(ref LeftPrimary, ControllerInputPoller.instance.leftControllerPrimaryButton);
        HandleInput(ref LeftSecondary, ControllerInputPoller.instance.leftControllerSecondaryButton);
        HandleInput(ref LeftTrigger, ControllerInputPoller.instance.leftControllerTriggerButton);
        HandleInput(ref LeftGrip, ControllerInputPoller.instance.leftGrab);

        LeftJoystick.Axis = ControllerInputPoller.instance.leftControllerPrimary2DAxis;
        RightJoystick.Axis = ControllerInputPoller.instance.rightControllerPrimary2DAxis;
    }

    public InputType[] GetCurrentlyPressedInputs() => Enum.GetValues(typeof(InputType)).Cast<InputType>()
                                                          .Where(inputType => GetInput(inputType).IsPressed).ToArray();

    public ControllerButton[] GetCurrentlyPressedControllerButtons() =>
            (from InputType inputType in Enum.GetValues(typeof(InputType))
             where GetInput(inputType).IsPressed
             select GetInput(inputType)).ToArray();

    public ControllerButton GetInput(InputType inputType) => inputType switch
    {
        InputType.RightPrimary => RightPrimary,
        InputType.RightSecondary => RightSecondary,
        InputType.RightTrigger => RightTrigger,
        InputType.RightGrip => RightGrip,
        InputType.LeftPrimary => LeftPrimary,
        InputType.LeftSecondary => LeftSecondary,
        InputType.LeftTrigger => LeftTrigger,
        InputType.LeftGrip => LeftGrip,
        var _ => default(ControllerButton),
    };

    private void HandleInput(ref ControllerButton button, bool isPressed)
    {
        bool wasPressed = button.IsPressed;
        button.IsPressed = isPressed;
        button.IsReleased = !isPressed;

        button.WasReleased = wasPressed && !isPressed;
        button.WasPressed = !wasPressed && isPressed;
    }

    private void HandleJoystickInput(ref ControllerJoystick button, bool isPressed)
    {
        bool wasPressed = button.IsPressed;
        button.IsPressed = isPressed;
        button.IsReleased = !isPressed;

        button.WasReleased = wasPressed && !isPressed;
        button.WasPressed = !wasPressed && isPressed;
    }

    public struct ControllerButton
    {
        public bool IsPressed;
        public bool WasPressed;

        public bool IsReleased;
        public bool WasReleased;
    }

    public struct ControllerJoystick
    {
        public bool IsPressed;
        public bool WasPressed;

        public bool IsReleased;
        public bool WasReleased;

        public Vector2 Axis;
    }
}