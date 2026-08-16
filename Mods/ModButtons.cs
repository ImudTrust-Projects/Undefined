using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using Undefined.Menu;
using Undefined.Mods.Categories;
using Undefined.Patches;
using Undefined.Utilities;
using static Undefined.Menu.Main;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Mods.Categories.Overpowered;
using static Undefined.Mods.Categories.SoundMods;
using static Undefined.Utilities.NotificationLib;
using static Undefined.Utilities.Variables;
using static Undefined.Utilities.ModButtonInfo;
using UnityEngine;
using Application = UnityEngine.Application;
using Console = Undefined.Mods.Categories.Console;

namespace Undefined.Mods;

public static class ModButtons
{
    public static readonly Dictionary<Category, ModButtonInfo[]> Buttons = new()
    {
        [Category.Main] =
        [
            new ModButtonInfo("Join Discord", JoinDiscord, false),

            Category("Settings", Category.Settings),
            Category("Enabled Mods", Category.EnabledMods),
            Category("Room Mods", Category.RoomMods),
            Category("Movement Mods", Category.MovementMods),
            Category("Fun Mods", Category.FunMods),
            Category("Visual Mods", Category.VisualMods),
            Category("Safety Mods", Category.SafetyMods),
            Category("Tag Mods", Category.TagMods),
            Category("Map Loader", Category.MapLoader),
            Category("Sound Spam Mods", Category.SoundSpamMods),
            Category("Sound Board", Category.SoundBoard),
            Category("Master Mods", Category.MasterMods),
            Category("Overpowered Mods", Category.OverpoweredMods)
        ],

        [Category.Settings] = new ModButtonInfo[]
        {
            Back(Category.Main),
            Category("Menu Settings", Category.MenuSettings),
            Category("Discord RPC", Category.DiscordRPC),
        },

        [Category.MenuSettings] = new ModButtonInfo[]
        {
            Back(Category.Settings),
            new ModButtonInfo("Right Hand", () => rightHanded = true, () => rightHanded = false),
            new ModButtonInfo("Disconnect Button", () => disconnectButton = true, () => disconnectButton = false) { enabled = disconnectButton },
            new ModButtonInfo("ArrayList", () => ArrayListEnabled = true, () => ArrayListEnabled = false) { enabled = true },
            new ModButtonInfo("Room Notifications", () => NotificationLib.RoomNotifications = true, () => NotificationLib.RoomNotifications = false) { enabled = true },
            new ModButtonInfo("Button Sound", SoundSettings.buttonSoundOptions, SoundSettings.SetButtonSound, 2),
            new ModButtonInfo("Font", MENUSETTINGS.Settings.fontOptions, MENUSETTINGS.Settings.SetFont, 2),
            new ModButtonInfo("Platform Mode", Movement.PlatformMode, Movement.SetPlatformMode),
            new ModButtonInfo("Speed Mode", Movement.SpeedBoostNames, Movement.SetSpeedBoost),
        },

        [Category.EnabledMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.RoomMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Join Menu", () => Room.JoinRoom("[Undefined]"), false),
            new ModButtonInfo("Disconnect", () => Room.Disconnect(), false),
            new ModButtonInfo("Join Random", () => Room.JoinRandomPublic(), false),
            new ModButtonInfo("Primary Disconnect", () => Room.PrimaryDisconnect(), true),
            new ModButtonInfo("US Region", () => Room.Servers("us"), false),
            new ModButtonInfo("USW Region", () => Room.Servers("usw"), false),
            new ModButtonInfo("EU Region", () => Room.Servers("eu"), false),
            new ModButtonInfo("Anti AFK", () => Room.EnableAntiAFK(), () => Room.DisableAntiAFK()),
            new ModButtonInfo("No Network Triggers", () => Room.DisableNetworkTriggers(), () => Room.EnableNetworkTriggers()),
            new ModButtonInfo("Get ID Self", () => Room.GetIdSelf(), false),
            new ModButtonInfo("Get ID Gun", () => Room.GetIdGun(), true),
            new ModButtonInfo("Mute Gun", () => Room.MuteGun(), true),
            new ModButtonInfo("Mute All", () => Room.MuteAll(true), () => Room.MuteAll(false)),
        },

        [Category.MovementMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Platforms", () => Movement.Platforms(), () => Movement.PlatformDisable()),
            new ModButtonInfo("SpeedBoost", () => Movement.SpeedBoost(), true),
            new ModButtonInfo("Fly", () => Movement.Fly(), true),
            new ModButtonInfo("Slingshot Fly", () => Movement.SlingshotFly(), true),
            new ModButtonInfo("Trigger Fly", () => Movement.TriggerFly(), true),
            new ModButtonInfo("Hand Fly", () => Movement.HandFly(), true),
            new ModButtonInfo("Joystick Fly", () => Movement.JoyStickFly(), true),
            new ModButtonInfo("WASD Fly", () => Movement.WASDFly(), true),
            new ModButtonInfo("Ghost Monkey", () => Movement.GhostMonke(), true),
            new ModButtonInfo("Invis Monkey", () => Movement.InvisMonke(), true),
            new ModButtonInfo("Low Gravity", () => Movement.GravityManager(Movement.Gravitytypes.Low), true),
            new ModButtonInfo("High Gravity", () => Movement.GravityManager(Movement.Gravitytypes.High), true),
            new ModButtonInfo("Zero Gravity", () => Movement.GravityManager(Movement.Gravitytypes.Zero), true),
            new ModButtonInfo("Reverse Gravity", () => Movement.GravityManager(Movement.Gravitytypes.Reverse), () => Movement.Reset_upsidedown()),
            new ModButtonInfo("Reverse Velocity", () => Movement.Reverse_velocity(), true),
            new ModButtonInfo("Dash", () => Movement.Dash(), true),
            new ModButtonInfo("CheckPoint", () => Movement.CheckPoint(), () => Movement.CheckPointDisable()),
            new ModButtonInfo("NoClip", () => Movement.NoClip(), true),
            new ModButtonInfo("Bouncy Monke", () => Movement.Bouncy(), () => Movement.ResetBouncy()),
            new ModButtonInfo("Pull Mod", () => Movement.PullMod(), true),
            new ModButtonInfo("Teleport Stump", () => Movement.TPSTUMP(), false),
            new ModButtonInfo("Teleport Gun", () => Movement.TeleportGun(), true),
            new ModButtonInfo("Auto Funny Run", () => Movement.AutoFunnyRun(), true),
            new ModButtonInfo("Walk on Water", () => Movement.WalkOnWater(), true),
            new ModButtonInfo("Auto Elevator Climb", () => Movement.AutoElevatorClimb(), true),
            new ModButtonInfo("No Tag Freeze", () => Movement.NoTagFreeze(), true),
            new ModButtonInfo("Pbbv Walk", () => Movement.PbbvWalk(), () => Movement.PbbvWalkDisable()),
        },

        [Category.FunMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Console Spoof", Fun.EnableConsoleSpoof, Fun.DisableConsoleSpoof),
            new ModButtonInfo("Quest Score 67", Fun.SetQuestScore67, false),
            new ModButtonInfo("Quest Score 420", Fun.SetQuestScore420, false),
            new ModButtonInfo("Quest Score Max", Fun.SetQuestScoreMax, false),
            new ModButtonInfo("Break Mod Checkers", Fun.BreakModCheckers, true),
            new ModButtonInfo("Bracelet", Fun.EnableBracelet, Fun.DisableBracelet),
            new ModButtonInfo("Water Splash Gun", () => Watergun(), () => VRRig.LocalRig.enabled = true),
            new ModButtonInfo("Water Splash", () => Watersplash(), true),
            new ModButtonInfo("RGB Monkey", Fun.RGBMonke, true),
            new ModButtonInfo("Rainbow Hoverboard", Fun.RainbowHoverboard, true),
            new ModButtonInfo("Strobe Hoverboard", Fun.StrobeHoverboard, true),
            new ModButtonInfo("Fast Hoverboard", Fun.FastHoverboard, Fun.FixHoverboard),
            new ModButtonInfo("Slow Hoverboard", Fun.SlowHoverboard, Fun.FixHoverboard),
            new ModButtonInfo("Hoverboard Minigun", () => HoverboardMinigun(), true),
            new ModButtonInfo("Spaz Head", Fun.SpazHead, true),
            new ModButtonInfo("Spin Head X", Fun.SpinHeadX, true),
            new ModButtonInfo("Spin Head Y", Fun.SpinHeadY, true),
            new ModButtonInfo("Spin Head Z", Fun.SpinHeadZ, true),
            new ModButtonInfo("Grab Rig", Fun.GrabRig, true),
            new ModButtonInfo("Helicopter Rig", Fun.HelicopterRig, true),
            new ModButtonInfo("Rig Gun", Fun.MoveRigGun, true),
            new ModButtonInfo("Spectate Gun", Fun.SpectateGun, true),
            new ModButtonInfo("Set Name HIDE", Fun.SetNameHIDE, false),
            new ModButtonInfo("Set Name SEEK", Fun.SetNameSEEK, false),
            new ModButtonInfo("Set Name RUN", Fun.SetNameRUN, false),
            new ModButtonInfo("Set Name HIDDEN", Fun.SetNameHIDDEN, false),
            new ModButtonInfo("Set Name FOUND", Fun.SetNameFOUND, false),
            new ModButtonInfo("Set Name BEHINDYOU", Fun.SetNameBEHINDYOU, false),
            new ModButtonInfo("Set Name STATUE", Fun.SetNameSTATUE, false),
            new ModButtonInfo("Set Name GHOST", Fun.SetNameGHOST, false),
            new ModButtonInfo("Set Name HAUNT", Fun.SetNameHAUNT, false),
            new ModButtonInfo("Set Name CREEP", Fun.SetNameCREEP, false),
            new ModButtonInfo("Set Name STALKER", Fun.SetNameSTALKER, false),
            new ModButtonInfo("Set Name 404", Fun.SetName404, false),
        },

        [Category.VisualMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("2D Box ESP", () => Visuals.BoxESP2DEnable(), () => Visuals.BoxESP2DDisable()) { method = Visuals.BoxESP2D },
            new ModButtonInfo("Humanoid ESP", () => Visuals.HumanoidESP(), () => Visuals.HumanoidESPOff()),
            new ModButtonInfo("Trails", () => Visuals.Trails(), () => Visuals.DisableTrail()),
            new ModButtonInfo("Chams", () => Visuals.ChamESPOn(), () => Visuals.ChamESPOff()),
            new ModButtonInfo("Bone ESP", () => Visuals.BoneESP(), () => Visuals.BoneESPOff()),
            new ModButtonInfo("Tracers", () => Visuals.TracerESP(), () => Visuals.TracerESPOff()),
        },

        [Category.SafetyMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Close Game", () => Application.Quit(), true),
            new ModButtonInfo("Anti Report", () => Safety.AntiReport(), true),
            new ModButtonInfo("Anti Report (Fling)", () => Safety.AntiReportSnowballfling(), true),
            new ModButtonInfo("Anti Moderator", () => Safety.AntiModeration(), true),
            new ModButtonInfo("Restart Game", () => Safety.RestartGame(), true),
            new ModButtonInfo("Anti-Cheat Notify", () => Variables.NotifySelf = true, () => Variables.NotifySelf = false),
        },

        [Category.TagMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Tag Gun", () => Tag.TagGun(), true),
            new ModButtonInfo("Tag All", () => Tag.TagAll(), true),
            new ModButtonInfo("Tag Self", () => Tag.TagSelf(), true),
            new ModButtonInfo("Tag Fix", () => Tag.TagFix(), () => Tag.DisableTagFix()),
            new ModButtonInfo("Tag Reach", Tag.TagReach, () => GorillaTagger.Instance.maxTagDistance = 1.2f),
            new ModButtonInfo("No Tag On Join", () => Tag.NoTagOnJoin(), true),
        },

        [Category.MapLoader] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("City", () => MapLoader.City(), false),
            new ModButtonInfo("Forest", () => MapLoader.Forest(), false),
            new ModButtonInfo("Lava Forest", () => MapLoader.LavaForest(), false),
        },

        [Category.SoundSpamMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Random Spam", () => RandomSoundspam(), true),
            new ModButtonInfo("Wolf Spam", () => Wolf(), true),
            new ModButtonInfo("Lemming Spam", () => Lemming(), true),
            new ModButtonInfo("Jman Spam", () => jmancurly_Soundspam(), true),
            new ModButtonInfo("Crystal Spam", () => Crystal(), true),
            new ModButtonInfo("Shiny Rocks Spam", () => Shiny_Rocks(), true),
            new ModButtonInfo("Fireworks Spam", () => Fireworks(), true),
            new ModButtonInfo("Bouncy Spam", () => Bouncythings(), true),
            new ModButtonInfo("Voting Rock Spam", () => Voting_Rock(), true),
            new ModButtonInfo("AK47", () => AK_47(), true),
            new ModButtonInfo("Sound ID", soundOptions, SetSound),
            new ModButtonInfo("Sound Spam", () => PlaySelectedSound(), true),
            new ModButtonInfo("Override Hand Taps", () => Override_HandTap_Sounds(false), () => Override_HandTap_Sounds(true)),
            new ModButtonInfo("No Hand Taps", () => No_hand_taps(false), () => No_hand_taps(true)),
        },

        [Category.GuardianMods] = new ModButtonInfo[]
        {
            Back(Category.MasterMods),
            new ModButtonInfo("Guardian Self", () => Guardian.GuardianSelf(), true),
            new ModButtonInfo("Guardian Grab All", () => Guardian.GuardianGrabAll(), true),
            new ModButtonInfo("Guardian Spaz All", () => Guardian.GuardianSpazAll(), true),
            new ModButtonInfo("Guardian Fling All", () => Guardian.GuardianFlingAll(), true),
            new ModButtonInfo("Guardian Fling Gun", () => Guardian.GuardianFlingGun(), true),
            new ModButtonInfo("Guardian Break Move All", () => Guardian.GuardianBreakMovementAll(), true),
            new ModButtonInfo("Guardian Break Move Gun", () => Guardian.GuardianBreakMovementGun(), true),
        },

        [Category.MasterMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Guardian Mods", () => Main.activeCategory = Category.GuardianMods, false),
            new ModButtonInfo("Grey Screen", () => Master.GreyScreen(), () => Master.DisableGreyScreen()),
            new ModButtonInfo("Spaz Targets", () => Master.SpazTargets(), true),
            new ModButtonInfo("Break Targets", () => Master.BreakTargets(), true),
            new ModButtonInfo("Break Elevator", () => Master.BreakElevator(), true),
            new ModButtonInfo("Untag Self", () => Master.UntagSelf(), false),
            new ModButtonInfo("Untag All", () => Master.UntagAll(), false),
            new ModButtonInfo("Force Tag Lag", () => Master.ForceTagLag(), true),
            new ModButtonInfo("No Tag Cooldown", () => Master.NoTagCooldown(), true),
            new ModButtonInfo("Lock Room", () => Master.LockRoom(), true),
            new ModButtonInfo("Unlock Room", () => Master.UnlockRoom(), true),
            new ModButtonInfo("Spaz Room", () => Master.SpazRoom(), true),
            new ModButtonInfo("Vibrate Gun", () => Master.ViberateGun(), true),
            new ModButtonInfo("Vibrate All", () => Master.ViberateAll(), true),
            new ModButtonInfo("Material Gun", () => Master.MatGun(), true),
            new ModButtonInfo("Material All", () => Master.MatAll(), true),
        },

        [Category.SoundBoard] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.OverpoweredMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Stutter Master", () => StutterMaster(), true),
            new ModButtonInfo("Destroy Gun", () => DestroyGun(), false),
            new ModButtonInfo("Destroy All", () => DestroyAll(), false),
            new ModButtonInfo("Lag Gun", () => LagGun(), true),
            new ModButtonInfo("Lag All", () => LagAll(), true),
            new ModButtonInfo("Lag On Touch", () => LagOnTouch(), true),
            new ModButtonInfo("Stump Kick All", () => STumpkickall(), true),
            new ModButtonInfo("Grab Fling Gun", () => GrabFlingGun(), true),
            new ModButtonInfo("Grab Fling All", () => GrabFlingAll(), true),
        },

        [Category.NetworkedMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.DiscordRPC] = new ModButtonInfo[]
        {
            Back(Category.Settings),
            new ModButtonInfo("Enable RPC", () => DiscordPresence.DiscordRPC = true, () => DiscordPresence.DiscordRPC = false),
            new ModButtonInfo("RPC Privacy", () => DiscordPresence.Instance.SetPrivacyRPC(true), () => DiscordPresence.Instance.SetPrivacyRPC(false)),
        },

        [Category.Admin] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("No Admin Indicator", () => Console.EnableNoAdminIndicator(), () => Console.DisableNoAdminIndicator()) { method = Console.UpdateNoAdminIndicator },
            new ModButtonInfo("Admin Notificator", () => Console.AdminNotificatorEnable(), () => Console.AdminNotificatorDisable()),
            new ModButtonInfo("Admin Laser", () => Console.AdminLaser(), true),
            new ModButtonInfo("Admin Beam", () => Console.AdminBeam(), true),
            new ModButtonInfo("Admin Bring All", () => Console.BringAllUsing(), true),
            new ModButtonInfo
            {
                buttonText = "Conduct Users",
                enableMethod = () =>
                {
                    Console.EnableAdminMenuUserTags();
                    Variables.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>().text = "CONSOLE USER LIST";
                    Variables.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().richText = true;
                },
                method = Console.ConsoleOnConduct
            },
        },

        [Category.SuperAdmin] = new ModButtonInfo[]
        {
            Back(Category.Admin),
            new ModButtonInfo("Disable Asset Music", null, true),
            new ModButtonInfo("Rainbow Sword", () => ConsoleAssets.spawnRainbowSword(), () => ConsoleAssets.destroyRainbowSword()) { method = ConsoleAssets.UpdateRainbowSword },
            new ModButtonInfo("Ban Hammer", () => ConsoleAssets.spawnBanHammer(), () => ConsoleAssets.destroyBanHammer()) { method = ConsoleAssets.UpdateBanHammer },
            new ModButtonInfo("Roblox Sword", () => ConsoleAssets.spawnRobloxSword(), () => ConsoleAssets.destroyRobloxSword()) { method = ConsoleAssets.UpdateRobloxSword },
            new ModButtonInfo("Video Player", () => ConsoleAssets.VideoPlayer(), () => ConsoleAssets.destroyVideoPlayer()),
            new ModButtonInfo("Pistol", () => ConsoleAssets.spawnPistol(), () => ConsoleAssets.destroyPistol()) { method = ConsoleAssets.UpdatePistol },
            new ModButtonInfo("Super Crown", () => ConsoleAssets.supercrown(), () => ConsoleAssets.destroysupercrown()),
            new ModButtonInfo("Travis Scott", () => ConsoleAssets.TravisScottConcert(), () => ConsoleAssets.destroyTravisScottConcert()),
            new ModButtonInfo("Mini Travis", () => ConsoleAssets.spawnMiniTravis(), () => ConsoleAssets.destroyminiTravis()),
            new ModButtonInfo("Fake Menu", () => ConsoleAssets.spawnBaitMenu(), () => ConsoleAssets.destroyBaitMenu()),
            new ModButtonInfo("Cheezburger", () => ConsoleAssets.spawnCheezburger(), () => ConsoleAssets.destroyCheezburger()),
            new ModButtonInfo("Gorilla TV", () => ConsoleAssets.GorillaTv(), () => ConsoleAssets.DestroyGorillaTv()),
        },
    };

    public static ModButtonInfo IsEnabled(string name)
    {
        foreach (var category in Buttons.Values)
        {
            foreach (var button in category)
            {
                if (button != null && button.buttonText == name)
                    return button;
            }
        }
        return null;
    }

    public static List<ModButtonInfo> GetActiveMods()
    {
        var active = new List<ModButtonInfo>();

        foreach (var category in Buttons.Values)
        {
            foreach (var btn in category)
            {
                if (btn == null) continue;
                if (!btn.isTogglable) continue;
                if (string.IsNullOrEmpty(btn.buttonText)) continue;
                if (btn.buttonText.StartsWith("Return") || btn.buttonText.StartsWith("Back")) continue;
                if (btn.enabled && !active.Contains(btn))
                    active.Add(btn);
            }
        }

        return active;
    }
}