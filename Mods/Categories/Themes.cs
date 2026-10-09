using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public static string CustomColorFolder => Path.Combine(BepInEx.Paths.GameRootPath, Constants.PluginName, "Custom Colors");

    static Themes()
    {
        try
        {
            Directory.CreateDirectory(CustomColorFolder);
        }
        catch { }
    }

    public static void SaveColor()
    {
        Directory.CreateDirectory(CustomColorFolder);

        int number = 1;
        while (File.Exists(Path.Combine(CustomColorFolder, $"Custom Color {number}.txt")))
            number++;

        Color32 color = new Color32((byte)red, (byte)green, (byte)blue, 255);
        File.WriteAllText(Path.Combine(CustomColorFolder, $"Custom Color {number}.txt"), "#" + ColorUtility.ToHtmlStringRGB(color));

        NotificationLib.SendNotification(NotificationLib.NotificationType.Saved, $"Custom Color {number}");
    }

    public static void DeleteColor(string colorName)
    {
        string path = Path.Combine(CustomColorFolder, colorName + ".txt");

        if (!File.Exists(path))
            return;

        File.Delete(path);
        NotificationLib.SendNotification(NotificationLib.NotificationType.Deleted, colorName);
    }

    public static void UpdateCustomColors()
    {
        List<ModButtonInfo> buttons = new List<ModButtonInfo>
        {
            ModButtonInfo.Back(Category.Themes)
        };

        try
        {
            Directory.CreateDirectory(CustomColorFolder);

            var files = Directory.GetFiles(CustomColorFolder, "*.txt")
                .OrderBy(file => int.TryParse(Path.GetFileNameWithoutExtension(file).Replace("Custom Color", "").Trim(), out int number) ? number : int.MaxValue)
                .ThenBy(file => file);

            foreach (string file in files)
            {
                if (!ColorUtility.TryParseHtmlString(File.ReadAllText(file).Trim(), out Color color))
                    continue;

                Color32 color32 = color;
                buttons.Add(new ModButtonInfo(Path.GetFileNameWithoutExtension(file), () => SetColor(color32.r, color32.g, color32.b), false));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"{Constants.PluginName} Custom Colors Err: {ex.Message}");
        }

        ModButtons.Buttons[Category.CustomColors] = buttons.ToArray();

        if (Variables.activePage * Variables.buttonsPerPage >= buttons.Count)
            Variables.activePage = 0;
    }

    public static bool pageButtons;
    public static bool sideButtons;
    public static bool wristMenu;
    public static bool thickMenu;
    public static bool thinMenu;

    public static void UseThickMenu()
    {
        thickMenu = true;
        thinMenu = false;
        SetEnabled("Thin Menu", false);
    }

    public static void UseThinMenu()
    {
        thinMenu = true;
        thickMenu = false;
        SetEnabled("Thick Menu", false);
    }

    public static void UseTriggers()
    {
        pageButtons = false;
        sideButtons = false;
        SetEnabled("Under Buttons", false);
        SetEnabled("Side Buttons", false);
    }

    public static void TriggersOff()
    {
        if (!IsOn("Under Buttons") && !IsOn("Side Buttons"))
            SetEnabled("Triggers", true);
    }

    public static void UseUnderButtons()
    {
        pageButtons = true;
        sideButtons = false;
        SetEnabled("Triggers", false);
        SetEnabled("Side Buttons", false);
    }

    public static void UnderButtonsOff()
    {
        pageButtons = false;

        if (!IsOn("Side Buttons"))
            SetEnabled("Triggers", true);
    }

    public static void UseSideButtons()
    {
        sideButtons = true;
        pageButtons = false;
        SetEnabled("Triggers", false);
        SetEnabled("Under Buttons", false);
    }

    public static void SideButtonsOff()
    {
        sideButtons = false;

        if (!IsOn("Under Buttons"))
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
