using Photon.Pun;
using System.Collections.Generic;
using System.Diagnostics;
using Undefined.Mods.Categories;
using Undefined.Utilities;
using static Undefined.Menu.Main;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Utilities.NotificationLib;
using static Undefined.Utilities.Variables;
using static Undefined.Mods.Categories.SoundMods;
using static Undefined.Mods.Categories.Overpowered;

namespace Undefined.Mods;

public class ModButtons
{
    public static ButtonInfo[][] buttons = new ButtonInfo[][]
    {
        new ButtonInfo[] { // Main Mods [0]
            new ButtonInfo { buttonText = "Join Discord", method = JoinDiscord, isTogglable = false, toolTip = "Makes you join the discord server for Undefined menu."},

            new ButtonInfo { buttonText = "Settings", method =() => activeCategory = 1, isTogglable = false, },

            new ButtonInfo { buttonText = "Enabled Mods", method =() => activeCategory = 3, isTogglable = false, },

            new ButtonInfo { buttonText = "Room Mods", method =() => activeCategory = 4, isTogglable = false, },
            new ButtonInfo { buttonText = "Movement Mods", method =() => activeCategory = 5, isTogglable = false, },
            new ButtonInfo { buttonText = "Fun Mods", method =() => activeCategory = 6, isTogglable = false, },
            new ButtonInfo { buttonText = "Visual Mods", method =() => activeCategory = 7, isTogglable = false, },
            new ButtonInfo { buttonText = "Tag Mods", method =() => activeCategory = 8, isTogglable = false, },
            new ButtonInfo { buttonText = "Map Loader", method =() => activeCategory = 9, isTogglable = false, },
            new ButtonInfo { buttonText = "Master Mods", method =() => activeCategory = 10, isTogglable = false, },
            new ButtonInfo { buttonText = "Overpowered Mods", method =() => activeCategory = 11, isTogglable = false, },
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
            new ButtonInfo { buttonText = "Room Notifications", enableMethod =() => NotificationLib.RoomNotifications = true, disableMethod =() => NotificationLib.RoomNotifications = false, enabled = NotificationLib.RoomNotifications = true, toolTip = "Toggles the Room Notifications."},
            new ButtonInfo { buttonText = "Button Sound", isTogglable = false, isIncremental = true, incrementalDisplayName = "Button Sound", incrementalValues = Settings.buttonSoundOptions, incrementalMethod = Settings.SetButtonSound, toolTip = "Changes the button click sound." },
            new ButtonInfo { buttonText = "Font", isTogglable = false, isIncremental = true, incrementalDisplayName = "Font", incrementalValues = MENUSETTINGS.Settings.fontOptions, incrementalMethod = MENUSETTINGS.Settings.SetFont, toolTip = "Changes the menu font." },
            new ButtonInfo { buttonText = "Platform Mode", isTogglable = false, isIncremental = true, incrementalDisplayName = "Mode", incrementalValues = Movement.PlatformMode, incrementalMethod = Movement.SetPlatformMode, toolTip = "Changes the platform type." },
        },

        new ButtonInfo[] { // Enabled Mods [3]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

        },

        new ButtonInfo[] { // Room Mods [4]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "Join Menu Code", method =() => Room.JoinRoom("[Undefined]"), isTogglable = false, toolTip = "Joins the menu code."},
            new ButtonInfo { buttonText = "Disconnect", method =() => Room.Disconnect(), isTogglable = false, toolTip = "Disconnects you from the room."},
            new ButtonInfo { buttonText = "Join Random Public", method =() => Room.JoinRandomPublic(), isTogglable = false, toolTip = "Makes you join a random public server."},
            new ButtonInfo { buttonText = "Primary Disconnect", method =() => Room.PrimaryDisconnect(), isTogglable = true, toolTip = "Disconnects you from the room if u press right primary button."},
            new ButtonInfo { buttonText = "Connect to US", method =() => Room.Servers("us"), isTogglable = false, toolTip = "Connects you to the United States servers."},
            new ButtonInfo { buttonText = "Connect to USW", method =() => Room.Servers("usw"), isTogglable = false, toolTip = "Connects you to the western United States servers."},
            new ButtonInfo { buttonText = "Connect to EU", method =() => Room.Servers("eu"), isTogglable = false, toolTip = "Connects you to the Europe servers."},
            new ButtonInfo { buttonText = "Anti Afk", enableMethod =() => Room.EnableAntiAFK(), disableMethod =() => Room.DisableAntiAFK(), isTogglable = true, toolTip = "Makes you not get kicked if u go afk."},
            new ButtonInfo { buttonText = "No Network Triggers", enableMethod =() => Room.DisableNetworkTriggers(), disableMethod =() => Room.EnableNetworkTriggers(), isTogglable = true, toolTip = "Disables network triggers."},
            new ButtonInfo { buttonText = "Get Id Self", method =() => Room.GetIdSelf(), isTogglable = false, toolTip = "Gets ur Id."},
            new ButtonInfo { buttonText = "Get Id Gun", method =() => Room.GetIdGun(), isTogglable = true, toolTip = "Gets the Id of the gorilla ur pointing at."},
        },

        new ButtonInfo[] { // Movement Mods [5]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},


            new ButtonInfo { buttonText = "PlatForms", method =() => Movement.Platforms(), disableMethod =() => Movement.PlatformDisable(), isTogglable = true, toolTip = "You can fly."},
            new ButtonInfo { buttonText = "Fly", method =() => Movement.Fly(), isTogglable = true, toolTip = "You can fly."},
            new ButtonInfo { buttonText = "Bark Fly", method =() => Movement.BarkFly(), isTogglable = true, toolTip = "You can fly like the og bark fly."},
            new ButtonInfo { buttonText = "CheckPoint", method =() => Movement.CheckPoint(), disableMethod =() => Movement.CheckPointDisable(), isTogglable = true, toolTip = "You can fly."},
            new ButtonInfo { buttonText = "NoClip", method =() => Movement.NoClip(), isTogglable = true, toolTip = "You can go through Objects by holding right trigger."},
            new ButtonInfo { buttonText = "Bouncy Monke", enableMethod =() => Movement.Bouncy(), disableMethod =() => Movement.ResetBouncy(), isTogglable = true, toolTip = "Makes you a Bouncy monke."},
            new ButtonInfo { buttonText = "WASD Fly", method =() => Movement.WASDFly(), isTogglable = true, toolTip = "You can fly around with WASD."},
            new ButtonInfo { buttonText = "Pull Mod", method =() => Movement.PullMod(), isTogglable = true, toolTip = "You go broom broom fast."},
            new ButtonInfo { buttonText = "Teleport to Stump", method =() => Movement.TPSTUMP(), isTogglable = false, toolTip = "You get teleported to stump."},
            new ButtonInfo { buttonText = "Teleport gun", method =() => Movement.TeleportGun(), isTogglable = true, toolTip = "You can teleport by pressing trigger on ur controller."},
            new ButtonInfo { buttonText = "Auto Funny Run", method =() => Movement.AutoFunnyRun(), isTogglable = true, toolTip = "Makes you Auto Funny Run."},
            new ButtonInfo { buttonText = "Auto Elevator Climb", method =() => Movement.AutoElevatorClimb(), isTogglable = true, toolTip = "Makes you Auto Elevator Climb."},
            new ButtonInfo { buttonText = "No Tag Freeze", method =() => Movement.NoTagFreeze(), isTogglable = true, toolTip = "Disables tag freeze on your character."},
            new ButtonInfo { buttonText = "Pbbv Walk", enableMethod =() => Movement.PbbvWalk(), disableMethod =() => Movement.PbbvWalkDisable(), isTogglable = true, toolTip = "Disables tag freeze on your character."},
        },

        new ButtonInfo[] { // Fun Mods [6]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "Console Spoof", enableMethod =() => Fun.EnableConsoleSpoof(), disableMethod =() => Fun.DisableConsoleSpoof(), isTogglable = true, toolTip = "Spoofs Console Name."},
            new ButtonInfo { buttonText = "Quest Score 67", method =() => Fun.SetQuestScore(67), isTogglable = false, toolTip = "Sets ur Quest Score to 67."},
            new ButtonInfo { buttonText = "Quest Score 420", method =() => Fun.SetQuestScore(420), isTogglable = false, toolTip = "Sets ur Quest Score to 420."},
            new ButtonInfo { buttonText = "Quest Score Max", method =() => Fun.SetQuestScore(999999999), isTogglable = false, toolTip = "Sets ur Quest Score to the max."},
            new ButtonInfo { buttonText = "Bracelet", enableMethod = () => Fun.Get_Bracelet(true, true), disableMethod = () => Fun.Get_Bracelet(false, true) , isTogglable = true},
            new ButtonInfo { buttonText = "Water Splash Gun", method = () => Watergun(), disableMethod = () => VRRig.LocalRig.enabled = true, toolTip = "Splashes water on whoever you put the gun on"},
            new ButtonInfo { buttonText = "Water Splash", method = () => Watersplash(), toolTip = "Splashes water on your left or right hand"},
            new ButtonInfo { buttonText = "Hover Board Minigun", method = () => HoverboardMinigun(), toolTip = "Spams hoverboards out of your hand"},
            new ButtonInfo { buttonText = "Random Sound Spam", method = () => RandomSoundspam(), isTogglable = true},
            new ButtonInfo { buttonText = "Wolf Sound Spam", method = () => Wolf(), isTogglable = true},
            new ButtonInfo { buttonText = "Lemming Sound Spam", method = () => Lemming(), isTogglable = true},
            new ButtonInfo { buttonText = "Jman Sound Spam", method = () => jmancurly_Soundspam(), isTogglable = true},
            new ButtonInfo { buttonText = "Crystal Sound Spam", method = () => Crystal(), isTogglable = true},
            new ButtonInfo { buttonText = "Shiny Rocks Sound Spam", method = () => Shiny_Rocks(), isTogglable = true},
            new ButtonInfo { buttonText = "Fireworks Sound Spam", method = () => Fireworks(), isTogglable = true},
            new ButtonInfo { buttonText = "Bouncy Sound Spam", method = () => Bouncythings(), isTogglable = true},
            new ButtonInfo { buttonText = "Voting Rock Sound Spam", method = () => Voting_Rock(), isTogglable = true},
            new ButtonInfo { buttonText = "AK47", method = () => AK_47(), isTogglable = true},
            new ButtonInfo { buttonText = "Sound ID", isTogglable = false, isIncremental = true, incrementalDisplayName = "ID", incrementalValues = soundOptions, incrementalMethod = SetSound, toolTip = "Selects the sound ID (1-324)." },
            new ButtonInfo { buttonText = "Sound Spam", method = () => PlaySelectedSound(), isTogglable = true, toolTip = "Spams the selected sound ID." },
            new ButtonInfo { buttonText = "Override Hand Tap Sounds", method = () => Override_HandTap_Sounds(false), disableMethod = () => Override_HandTap_Sounds(true), isTogglable = true, toolTip = "Makes your hand tap sounds the custom sound id" },
            new ButtonInfo { buttonText = "No Hand Taps", method = () => No_hand_taps(false), disableMethod = () => No_hand_taps(true), toolTip = "Disables your hand taps"}
        },

        new ButtonInfo[] { // Visual Mods [7]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "2D Box ESP", enableMethod =() => Visuals.BoxESP2DEnable(), method =() => Visuals.BoxESP2D(), disableMethod =() => Visuals.BoxESP2DDisable(), isTogglable = true, toolTip = "Shows 2D box ESP on players"},
        },

        new ButtonInfo[] { // Tag Mods [8]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "Tag Gun", method =() => Tag.TagGun(), isTogglable = true, toolTip = "Tag people from afar"},
            new ButtonInfo { buttonText = "Tag All", method =() => Tag.TagAll(), isTogglable = true, toolTip = "Tags everyone in the lobbie"},
            new ButtonInfo { buttonText = "Tag Self", method =() => Tag.TagSelf(), isTogglable = true, toolTip = "tp to tagged player"},
            new ButtonInfo { buttonText = "Tag Fix", enableMethod =() => Tag.TagFix(), disableMethod =() => Tag.DisableTagFix(), isTogglable = true, toolTip = "Makes it so you can tag people from far away like og times"},
            new ButtonInfo { buttonText = "Tag Reach", method = Tag.TagReach, disableMethod =() => GorillaTagger.Instance.maxTagDistance = 1.2f, toolTip = "Makes your hand tag hitbox larger."},
        },

        new ButtonInfo[] { // Map Loader [9]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "City", method =() => MapLoader.City(), isTogglable = false, toolTip = "Teleports you to city"},
            new ButtonInfo { buttonText = "Forest", method =() => MapLoader.Forest(), isTogglable = false, toolTip = "Teleports you to Forest"},
            new ButtonInfo { buttonText = "Lava Forest", method =() => MapLoader.LavaForest(), isTogglable = false, toolTip = "Teleports you to Lava-Forest"},
        },

        new ButtonInfo[] { // Master Mods [10]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "Grey Screen", enableMethod =() => Master.GreyScreen(), disableMethod =() => Master.DisableGreyScreen(), isTogglable = true},
            new ButtonInfo { buttonText = "Guardian All", enableMethod = () => Master.GuardianAll(), disableMethod =() => Master.DisableGuardianAll(), isTogglable = true},
            new ButtonInfo { buttonText = "Spaz Targets", method = () => SpazTargets(), toolTip = "Spazes all the targets"},
            new ButtonInfo { buttonText = "Break Targets", method = () => BreakTargets(), toolTip = "Breaks all the targets"},
            new ButtonInfo { buttonText = "Break Elevator", method = () => BreakElevator(), toolTip = "Breaks The Elevator so people can phase through it"},
            new ButtonInfo { buttonText = "Break Targets", method = () => BreakTargets(), toolTip = "Breaks all the targets"},
            new ButtonInfo { buttonText = "Untag Self", method = () => UntagSelf(), isTogglable = false, toolTip = "Untags you"},
            new ButtonInfo { buttonText = "Untag All", method = () => UntagAll(), isTogglable = false, toolTip = "Untags everyone"},
            new ButtonInfo { buttonText = "Force Tag Lag", method = () => ForceTagLag(), toolTip = "Forces tag lag in the lobby"},
            new ButtonInfo { buttonText = "No Tag Cooldown", method = () => NoTagCooldown(), toolTip = "Makes it so there is no tag cooldown"},
            new ButtonInfo { buttonText = "No Tag Cooldown", method = () => NoTagCooldown(), toolTip = "Makes it so there is no tag cooldown"},
        },

        new ButtonInfo[] { // Overpowered Mods [11]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},
            new ButtonInfo { buttonText = "Stutter Master Client", method = () => StutterMaster(), toolTip = "Stutters Master Client"},
            new ButtonInfo { buttonText = "Destroy All", method = () => DestroyAll(), isTogglable = false, toolTip = "Destroys everyone"},
        },

        new ButtonInfo[] { // Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, categoryName = "Admin"},

            new ButtonInfo { buttonText = "No Admin Indicator", enableMethod =() => Console.EnableNoAdminIndicator(), method =() => Console.UpdateNoAdminIndicator(), disableMethod =() => Console.DisableNoAdminIndicator(), isTogglable = true},
            new ButtonInfo { buttonText = "Admin Notificator", enableMethod =() => Console.AdminNotificatorEnable(), disableMethod =() => Console.AdminNotificatorDisable(), isTogglable = true},
        },

        new ButtonInfo[] { // Super Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, categoryName = "SuperAdmin"},

            new ButtonInfo { buttonText = "Rainbow Sword", enableMethod =() => ConsoleAssets.spawnRainbowSword(), method =() => ConsoleAssets.UpdateRainbowSword(), disableMethod =() => ConsoleAssets.destroyRainbowSword(), isTogglable = true},
            new ButtonInfo { buttonText = "Ban Hammer", enableMethod =() => ConsoleAssets.spawnBanHammer(), method =() => ConsoleAssets.UpdateBanHammer(), disableMethod =() => ConsoleAssets.destroyBanHammer(), isTogglable = true},
            new ButtonInfo { buttonText = "Roblox Sword", enableMethod =() => ConsoleAssets.spawnRobloxSword(), method =() => ConsoleAssets.UpdateRobloxSword(), disableMethod =() => ConsoleAssets.destroyRobloxSword(), isTogglable = true},
            new ButtonInfo { buttonText = "Battle Arena", enableMethod =() => ConsoleAssets.spawnBattleArena(), disableMethod =() => ConsoleAssets.destroyBattleArena(), isTogglable = true},
            new ButtonInfo { buttonText = "Video Player", enableMethod =() => ConsoleAssets.VideoPlayer(), disableMethod =() => ConsoleAssets.destroyVideoPlayer(), isTogglable = true},
            new ButtonInfo { buttonText = "Pistol", enableMethod =() => ConsoleAssets.spawnPistol(), method =() => ConsoleAssets.UpdatePistol(), disableMethod =() => ConsoleAssets.destroyPistol(), isTogglable = true},
            new ButtonInfo { buttonText = "Super Crown", enableMethod =() => ConsoleAssets.supercrown(), disableMethod =() => ConsoleAssets.destroysupercrown(), isTogglable = true},
            new ButtonInfo { buttonText = "Travis Scott", enableMethod =() => ConsoleAssets.TravisScottConcert(), disableMethod =() => ConsoleAssets.destroyTravisScottConcert(), isTogglable = true},
            new ButtonInfo { buttonText = "Mini Travis Scott", enableMethod =() => ConsoleAssets.spawnMiniTravis(), disableMethod =() => ConsoleAssets.destroyminiTravis(), isTogglable = true},
            new ButtonInfo { buttonText = "Fake mod menu", enableMethod =() => ConsoleAssets.spawnBaitMenu(), disableMethod =() => ConsoleAssets.destroyBaitMenu(), isTogglable = true},
            new ButtonInfo { buttonText = "cheezburger", enableMethod =() => ConsoleAssets.spawnCheezburger(), disableMethod =() => ConsoleAssets.destroyCheezburger(), isTogglable = true},
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

                if (btn.enabled && !active.Contains(btn))
                    active.Add(btn);
            }
        }

        return active;
    }
}