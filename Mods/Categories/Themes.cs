using System.Collections.Generic;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.MENUSETTINGS.Settings;

namespace Undefined.Mods.Categories;

public static class Themes
{
    public static readonly List<string> ColorValues = new List<string>
    {
        "0", "25", "50", "75", "100", "125", "150", "175", "200", "225", "250", "255"
    };

    public static bool pageButtons;

    public static void UseTriggers()
    {
        pageButtons = false;
        SetEnabled("Buttons", false);
    }

    public static void TriggersOff()
    {
        if (!IsOn("Buttons"))
            SetEnabled("Triggers", true);
    }

    public static void UseButtons()
    {
        pageButtons = true;
        SetEnabled("Triggers", false);
    }

    public static void ButtonsOff()
    {
        pageButtons = false;
        SetEnabled("Triggers", true);
    }

    private static bool IsOn(string buttonText)
    {
        ModButtonInfo button = ModButtons.IsEnabled(buttonText);
        return button != null && button.enabled;
    }

    private static void SetEnabled(string buttonText, bool enabled)
    {
        ModButtonInfo button = ModButtons.IsEnabled(buttonText);

        if (button != null)
            button.enabled = enabled;
    }

    private static int red = 25;
    private static int green = 25;
    private static int blue = 25;

    public static void SetRed(string value)
    {
        if (int.TryParse(value, out int number))
            red = number;

        ApplyColor();
    }

    public static void SetGreen(string value)
    {
        if (int.TryParse(value, out int number))
            green = number;

        ApplyColor();
    }

    public static void SetBlue(string value)
    {
        if (int.TryParse(value, out int number))
            blue = number;

        ApplyColor();
    }

    public static void SetColor(int r, int g, int b)
    {
        red = r;
        green = g;
        blue = b;

        SetIndex("R", r);
        SetIndex("G", g);
        SetIndex("B", b);

        ApplyColor();
    }

    private static void SetIndex(string buttonText, int value)
    {
        ModButtonInfo button = ModButtons.IsEnabled(buttonText);
        int index = ColorValues.IndexOf(value.ToString());

        if (button != null && index >= 0)
            button.currentIncrementalIndex = index;
    }

    private static void ApplyColor()
    {
        Color color = new Color32((byte)red, (byte)green, (byte)blue, 255);
        Color light = new Color32((byte)Mathf.Min(red + 20, 255), (byte)Mathf.Min(green + 20, 255), (byte)Mathf.Min(blue + 20, 255), 255);

        backgroundColor.colors = ExtGradient.GetSimpleGradient(color, light);
    }
}
