using System;
using System.Collections.Generic;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Settings
{
    public static string currentButtonSound = "click3";
    public static bool DiscordRPC = false;
    public static string currentMenuOpenSound = "Open";
    public static string currentMenuCloseSound = "Close";

    public static string currentNotificationSound = "NotificationSound";

    public static List<string> buttonSoundOptions = new List<string>
    {
        "click1",
        "click2",
        "click3"
    };

    public static List<string> menuSoundOptions = new List<string>
    {
        "Open",
        "click1",
        "click2",
        "click3"
    };

    public static List<string> notificationOptions = new List<string>
    {
        "NotificationSound",
    };

    public static Dictionary<string, string> soundDisplayNames = new Dictionary<string, string>
    {
        { "Open", "Undefined" },
        { "Close", "Undefined" },
        { "click1", "Click 1" },
        { "click2", "Click 2" },
        { "click3", "Undefined" },
        { "NotificationSound", "Notification" }
    };

    public static void SetButtonSound(string sound)
    {
        currentButtonSound = sound;
        AudioHandler.Play(sound, 0.5f);
        SaveSound();
    }

    public static void SetMenuOpenSound(string sound)
    {
        currentMenuOpenSound = sound;
        SaveSound();
    }

    public static void SetMenuSound(string sound, bool syncOpenClose = true)
    {
        if (syncOpenClose)
        {
            currentMenuOpenSound = sound;
            currentMenuCloseSound = sound;
        }
        else
        {
            currentMenuOpenSound = sound;
        }

        SaveSound();
    }

    public static void PlayMenuOpen()
    {
        AudioHandler.Play(currentMenuOpenSound, 0.5f);
    }

    public static void PlayMenuClose()
    {
        AudioHandler.Play(currentMenuCloseSound, 0.5f);
    }

    public static void SetNotificationSound(string sound)
    {
        currentNotificationSound = sound;
        AudioHandler.Play(sound, 0.5f);
        SaveSound();
    }

    public static string GetButtonSoundDisplay()
    {
        return GetDisplayName(currentButtonSound);
    }

    public static string GetMenuOpenDisplay()
    {
        return GetDisplayName(currentMenuOpenSound);
    }

    public static string GetMenuCloseDisplay()
    {
        return GetDisplayName(currentMenuCloseSound);
    }

    public static string GetNotificationSoundDisplay()
    {
        return GetDisplayName(currentNotificationSound);
    }

    public static string GetDisplayName(string sound)
    {
        return soundDisplayNames.TryGetValue(sound, out var name) ? name : sound;
    }

    public static void LoadSound()
    {
        currentButtonSound = PlayerPrefs.GetString("ButtonSound", currentButtonSound);
        currentMenuOpenSound = PlayerPrefs.GetString("MenuOpenSound", currentMenuOpenSound);
        currentMenuCloseSound = PlayerPrefs.GetString("MenuCloseSound", currentMenuCloseSound);
        currentNotificationSound = PlayerPrefs.GetString("NotificationSound", currentNotificationSound);
    }

    private static void SaveSound()
    {
        PlayerPrefs.SetString("ButtonSound", currentButtonSound);
        PlayerPrefs.SetString("MenuOpenSound", currentMenuOpenSound);
        PlayerPrefs.SetString("MenuCloseSound", currentMenuCloseSound);
        PlayerPrefs.SetString("NotificationSound", currentNotificationSound);
        PlayerPrefs.Save();
    }

    public static bool Ghostview;
}