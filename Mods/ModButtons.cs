using Photon.Pun;
using Undefined.Mods.Categories;
using Undefined.Utilities;
using System.Collections.Generic;
using static Undefined.Menu.Main;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Utilities.Variables;
using static Undefined.Utilities.NotificationLib;

namespace Undefined.Mods;

public class ModButtons
{
    public static ButtonInfo[][] buttons = new ButtonInfo[][]
    {
        new ButtonInfo[] { // Main Mods [0]
            new ButtonInfo { buttonText = "Settings", method =() => activeCategory = 1, isTogglable = false, },

            new ButtonInfo { buttonText = "Room Mods", method =() => activeCategory = 3, isTogglable = false, },
            new ButtonInfo { buttonText = "Movement Mods", method =() => activeCategory = 4, isTogglable = false, },
            new ButtonInfo { buttonText = "Visual Mods", method =() => activeCategory = 5, isTogglable = false, },
        },

        new ButtonInfo[] { // Settings [1]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},
            new ButtonInfo { buttonText = "Menu", method =() => activeCategory = 2, isTogglable = false},
        },

        new ButtonInfo[] { // Menu Settings [2]
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = 1, isTogglable = false},
            new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Puts the menu on your right hand."},
            //new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles the FPS counter."},
            new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles the disconnect button."},
            new ButtonInfo { buttonText = "ArrayList", enableMethod =() => ArrayListEnabled = true, disableMethod =() => ArrayListEnabled = false, enabled = ArrayListEnabled = true, toolTip = "Toggles the ArrayList."},
        },

        new ButtonInfo[] { // Room Mods [3]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "Join Menu Code", method =() => Room.JoinRoom("[Undefined]"), isTogglable = false, toolTip = "Joins the menu code."},
            new ButtonInfo { buttonText = "Disconnect", method =() => Room.Disconnect(), isTogglable = false, toolTip = "Disconnects you from the room."},
            new ButtonInfo { buttonText = "Join Random Public", method =() => Room.JoinRandomPublic(), isTogglable = false, toolTip = "Makes you join a random public server."},
            new ButtonInfo { buttonText = "Primary Disconnect", method =() => Room.PrimaryDisconnect(), isTogglable = true, toolTip = "Disconnects you from the room if u press right primary button."},
            new ButtonInfo { buttonText = "Anti Afk", enableMethod =() => Room.EnableAntiAFK(), disableMethod =() => Room.DisableAntiAFK(), isTogglable = true, toolTip = "Makes you not get kicked if u go afk."},
            new ButtonInfo { buttonText = "No Network Triggers", enableMethod =() => Room.DisableNetworkTriggers(), disableMethod =() => Room.EnableNetworkTriggers(), isTogglable = true, toolTip = "Disables network triggers."},
        },

        new ButtonInfo[] { // Movement Mods [4]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},


            new ButtonInfo { buttonText = "Fly", method =() => Movement.Fly(), isTogglable = true, toolTip = "You can fly."},
            new ButtonInfo { buttonText = "NoClip", method =() => Movement.NoClip(), isTogglable = true, toolTip = "You can go through Objects by holding right trigger."},
            new ButtonInfo { buttonText = "Bouncy Monke", enableMethod =() => Movement.Bouncy(), disableMethod =() => Movement.ResetBouncy(), isTogglable = true, toolTip = "Makes you a Bouncy monke."},
            new ButtonInfo { buttonText = "WASD Fly", method =() => Movement.WASDFly(), isTogglable = true, toolTip = "You can fly around with WASD."},
            new ButtonInfo { buttonText = "Teleport to Stump", method =() => Movement.TPSTUMP(), isTogglable = true, toolTip = "You get teleported to stump."},
            new ButtonInfo { buttonText = "Teleport gun", method =() => Movement.TeleportGun(), isTogglable = true, toolTip = "You can teleport by pressing trigger on ur controller."},
        },

        new ButtonInfo[] { // Safety Mods [5]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "2D Box ESP", enableMethod =() => Visuals.BoxESP2DEnable(), method =() => Visuals.BoxESP2D(), disableMethod =() => Visuals.BoxESP2DDisable(), isTogglable = true, toolTip = "Shows 2D box ESP on players"},
        },

        new ButtonInfo[] { // Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, categoryName = "Admin"},

            new ButtonInfo { buttonText = "No Admin Indicator", enableMethod =() => Console.EnableNoAdminIndicator(), method =() => Console.UpdateNoAdminIndicator(), disableMethod =() => Console.DisableNoAdminIndicator(), isTogglable = true},
        },

        new ButtonInfo[] { // Super Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, categoryName = "SuperAdmin"},

            new ButtonInfo { buttonText = "Rainbow Sword", enableMethod =() => ConsoleAssets.spawnRainbowSword(), method =() => ConsoleAssets.UpdateRainbowSword(), disableMethod =() => ConsoleAssets.destroyRainbowSword(), isTogglable = true},
            new ButtonInfo { buttonText = "Roblox Sword", enableMethod =() => ConsoleAssets.spawnRobloxSword(), method =() => ConsoleAssets.UpdateRobloxSword(), disableMethod =() => ConsoleAssets.destroyRobloxSword(), isTogglable = true},
            new ButtonInfo { buttonText = "Battle Arena", enableMethod =() => ConsoleAssets.spawnBattleArena(), disableMethod =() => ConsoleAssets.destroyBattleArena(), isTogglable = true},
            new ButtonInfo { buttonText = "Pistol", enableMethod =() => ConsoleAssets.spawnPistol(), method =() => ConsoleAssets.UpdatePistol(), disableMethod =() => ConsoleAssets.destroyPistol(), isTogglable = true},
            new ButtonInfo { buttonText = "Super Crown", enableMethod =() => ConsoleAssets.supercrown(), disableMethod =() => ConsoleAssets.destroysupercrown(), isTogglable = true},
        },
    };

    // yes
    public static int FindCategory(string name)
    {
        for (int i = 0; i < ModButtons.buttons.Length; i++)
        {
            foreach (ButtonInfo button in ModButtons.buttons[i])
            {
                if (button.categoryName == name)
                    return i;
            }
        }

        return -1;
    }

    public static List<ButtonInfo> GetActiveMods()
    {
        List<ButtonInfo> active = new List<ButtonInfo>();

        foreach (var category in buttons)
        {
            foreach (var btn in category)
            {
                if (btn == null)
                    continue;

                if (!btn.isTogglable)
                    continue;

                if (string.IsNullOrEmpty(btn.buttonText))
                    continue;

                if (btn.buttonText.StartsWith("Return"))
                    continue;

                if (btn.enabled)
                    active.Add(btn);
            }
        }

        return active;
    }
}