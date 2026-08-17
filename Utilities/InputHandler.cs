using System;
using UnityEngine;

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
    LeftJoystick,
    RightJoystick,
}

public class InputHandler : Singleton<InputHandler>
{
    private const float JoystickDeadzoneThreshold = 0.5f;
    private const float JoystickDeadzoneThresholdSqr = JoystickDeadzoneThreshold * JoystickDeadzoneThreshold;

    private static readonly InputType[] AllInputTypes = (InputType[])Enum.GetValues(typeof(InputType));

    public ControllerJoystick LeftJoystick = ControllerJoystick.Released;
    public ControllerJoystick RightJoystick = ControllerJoystick.Released;

    public ControllerButton LeftPrimary = ControllerButton.Released;
    public ControllerButton LeftSecondary = ControllerButton.Released;
    public ControllerButton LeftTrigger = ControllerButton.Released;
    public ControllerButton LeftGrip = ControllerButton.Released;

    public ControllerButton RightPrimary = ControllerButton.Released;
    public ControllerButton RightSecondary = ControllerButton.Released;
    public ControllerButton RightTrigger = ControllerButton.Released;
    public ControllerButton RightGrip = ControllerButton.Released;

    private void Update()
    {
        if (ControllerInputPoller.instance == null)
        {
            ResetInputs();
            return;
        }

        HandleInput(ref RightPrimary, ControllerInputPoller.instance.rightControllerPrimaryButton);
        HandleInput(ref RightSecondary, ControllerInputPoller.instance.rightControllerSecondaryButton);
        HandleInput(ref RightTrigger, ControllerInputPoller.instance.rightControllerTriggerButton);
        HandleInput(ref RightGrip, ControllerInputPoller.instance.rightGrab);

        HandleInput(ref LeftPrimary, ControllerInputPoller.instance.leftControllerPrimaryButton);
        HandleInput(ref LeftSecondary, ControllerInputPoller.instance.leftControllerSecondaryButton);
        HandleInput(ref LeftTrigger, ControllerInputPoller.instance.leftControllerTriggerButton);
        HandleInput(ref LeftGrip, ControllerInputPoller.instance.leftGrab);

        Vector2 leftAxis = ControllerInputPoller.instance.leftControllerPrimary2DAxis;
        Vector2 rightAxis = ControllerInputPoller.instance.rightControllerPrimary2DAxis;

        HandleJoystickInput(ref LeftJoystick, leftAxis.sqrMagnitude >= JoystickDeadzoneThresholdSqr, leftAxis);
        HandleJoystickInput(ref RightJoystick, rightAxis.sqrMagnitude >= JoystickDeadzoneThresholdSqr, rightAxis);
    }

    private void ResetInputs()
    {
        HandleInput(ref RightPrimary, false);
        HandleInput(ref RightSecondary, false);
        HandleInput(ref RightTrigger, false);
        HandleInput(ref RightGrip, false);

        HandleInput(ref LeftPrimary, false);
        HandleInput(ref LeftSecondary, false);
        HandleInput(ref LeftTrigger, false);
        HandleInput(ref LeftGrip, false);

        HandleJoystickInput(ref LeftJoystick, false, Vector2.zero);
        HandleJoystickInput(ref RightJoystick, false, Vector2.zero);
    }

    public InputType[] GetCurrentlyPressedInputs()
    {
        int pressedCount = 0;
        Span<InputType> pressedInputs = stackalloc InputType[AllInputTypes.Length];

        for (int i = 0; i < AllInputTypes.Length; i++)
        {
            InputType inputType = AllInputTypes[i];
            if (GetInput(inputType).IsPressed)
            {
                pressedInputs[pressedCount++] = inputType;
            }
        }

        if (pressedCount == 0)
        {
            return Array.Empty<InputType>();
        }

        InputType[] result = new InputType[pressedCount];
        pressedInputs.Slice(0, pressedCount).CopyTo(result);
        return result;
    }

    public ControllerButton[] GetCurrentlyPressedControllerButtons()
    {
        int pressedCount = 0;
        Span<ControllerButton> pressedButtons = stackalloc ControllerButton[AllInputTypes.Length];

        for (int i = 0; i < AllInputTypes.Length; i++)
        {
            ControllerButton button = GetInput(AllInputTypes[i]);
            if (button.IsPressed)
            {
                pressedButtons[pressedCount++] = button;
            }
        }

        if (pressedCount == 0)
        {
            return Array.Empty<ControllerButton>();
        }

        ControllerButton[] result = new ControllerButton[pressedCount];
        pressedButtons.Slice(0, pressedCount).CopyTo(result);
        return result;
    }

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
        InputType.LeftJoystick => LeftJoystick,
        InputType.RightJoystick => RightJoystick,
        _ => default,
    };

    private void HandleInput(ref ControllerButton button, bool isPressed)
    {
        bool wasPressed = button.IsPressed;
        button.IsPressed = isPressed;
        button.IsReleased = !isPressed;

        button.WasReleased = wasPressed && !isPressed;
        button.WasPressed = !wasPressed && isPressed;
    }

    private void HandleJoystickInput(ref ControllerJoystick joystick, bool isPressed, Vector2 axis)
    {
        bool wasPressed = joystick.IsPressed;
        joystick.Axis = axis;
        joystick.IsPressed = isPressed;
        joystick.IsReleased = !isPressed;

        joystick.WasReleased = wasPressed && !isPressed;
        joystick.WasPressed = !wasPressed && isPressed;
    }

    public struct ControllerButton
    {
        public bool IsPressed;
        public bool WasPressed;

        public bool IsReleased;
        public bool WasReleased;

        public static ControllerButton Released => new ControllerButton { IsReleased = true };
    }

    public struct ControllerJoystick
    {
        public bool IsPressed;
        public bool WasPressed;

        public bool IsReleased;
        public bool WasReleased;

        public Vector2 Axis;

        public static ControllerJoystick Released => new ControllerJoystick { IsReleased = true, Axis = Vector2.zero };

        public static implicit operator ControllerButton(ControllerJoystick joystick) => new ControllerButton
        {
            IsPressed = joystick.IsPressed,
            WasPressed = joystick.WasPressed,
            IsReleased = joystick.IsReleased,
            WasReleased = joystick.WasReleased,
        };
    }
}