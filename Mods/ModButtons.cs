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
using UnityEngine;
using Application = UnityEngine.Application;
using Console = Undefined.Mods.Categories.Console;

namespace Undefined.Mods;

public static class ModButtons
{
    public static readonly Dictionary<Category, ButtonInfo[]> Buttons = new()
    {
        [Category.Main] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Join Discord", method = JoinDiscord, isTogglable = false, toolTip = "Join the Undefined discord server." },
            new ButtonInfo { buttonText = "Settings", method = () => Main.activeCategory = Category.Settings, isTogglable = false },
            new ButtonInfo { buttonText = "Enabled Mods", method = () => Main.activeCategory = Category.EnabledMods, isTogglable = false },
            new ButtonInfo { buttonText = "Room Mods", method = () => Main.activeCategory = Category.RoomMods, isTogglable = false },
            new ButtonInfo { buttonText = "Movement Mods", method = () => Main.activeCategory = Category.MovementMods, isTogglable = false },
            new ButtonInfo { buttonText = "Fun Mods", method = () => Main.activeCategory = Category.FunMods, isTogglable = false },
            new ButtonInfo { buttonText = "Visual Mods", method = () => Main.activeCategory = Category.VisualMods, isTogglable = false },
            new ButtonInfo { buttonText = "Safety Mods", method = () => Main.activeCategory = Category.SafetyMods, isTogglable = false },
            new ButtonInfo { buttonText = "Tag Mods", method = () => Main.activeCategory = Category.TagMods, isTogglable = false },
            new ButtonInfo { buttonText = "Map Loader", method = () => Main.activeCategory = Category.MapLoader, isTogglable = false },
            new ButtonInfo { buttonText = "Sound Spam", method = () => Main.activeCategory = Category.SoundSpamMods, isTogglable = false },
            new ButtonInfo { buttonText = "SoundBoard", method = () => Main.activeCategory = Category.SoundBoard, isTogglable = false },
            new ButtonInfo { buttonText = "Master Mods", method = () => Main.activeCategory = Category.MasterMods, isTogglable = false },
            new ButtonInfo { buttonText = "Overpowered", method = () => Main.activeCategory = Category.OverpoweredMods, isTogglable = false },
        },

        [Category.Settings] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Menu Settings", method = () => Main.activeCategory = Category.MenuSettings, isTogglable = false },
            new ButtonInfo { buttonText = "Discord RPC", method = () => Main.activeCategory = Category.DiscordRPC, isTogglable = false },
        },

        [Category.MenuSettings] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Back", method = () => Main.activeCategory = Category.Settings, isTogglable = false },
            new ButtonInfo { buttonText = "Right Hand", enableMethod = () => rightHanded = true, disableMethod = () => rightHanded = false },
            new ButtonInfo { buttonText = "Disconnect Button", enableMethod = () => disconnectButton = true, disableMethod = () => disconnectButton = false, enabled = disconnectButton },
            new ButtonInfo { buttonText = "ArrayList", enableMethod = () => ArrayListEnabled = true, disableMethod = () => ArrayListEnabled = false, enabled = true },
            new ButtonInfo { buttonText = "Room Notifications", enableMethod = () => NotificationLib.RoomNotifications = true, disableMethod = () => NotificationLib.RoomNotifications = false, enabled = true },
            new ButtonInfo { buttonText = "Button Sound", isTogglable = false, isIncremental = true, incrementalValues = SoundSettings.buttonSoundOptions, incrementalMethod = SoundSettings.SetButtonSound, currentIncrementalIndex = 2 },
            new ButtonInfo { buttonText = "Font", isTogglable = false, isIncremental = true, incrementalValues = MENUSETTINGS.Settings.fontOptions, incrementalMethod = MENUSETTINGS.Settings.SetFont, currentIncrementalIndex = 2 },
            new ButtonInfo { buttonText = "Platform Mode", isTogglable = false, isIncremental = true, incrementalValues = Movement.PlatformMode, incrementalMethod = Movement.SetPlatformMode },
            new ButtonInfo { buttonText = "Speed Mode", isTogglable = false, isIncremental = true, incrementalValues = Movement.SpeedBoostNames, incrementalMethod = Movement.SetSpeedBoost },
        },

        [Category.EnabledMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
        },

        [Category.RoomMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Join Menu", method = () => Room.JoinRoom("[Undefined]"), isTogglable = false },
            new ButtonInfo { buttonText = "Disconnect", method = () => Room.Disconnect(), isTogglable = false },
            new ButtonInfo { buttonText = "Join Random", method = () => Room.JoinRandomPublic(), isTogglable = false },
            new ButtonInfo { buttonText = "Primary Disconnect", method = () => Room.PrimaryDisconnect(), isTogglable = true },
            new ButtonInfo { buttonText = "US Region", method = () => Room.Servers("us"), isTogglable = false },
            new ButtonInfo { buttonText = "USW Region", method = () => Room.Servers("usw"), isTogglable = false },
            new ButtonInfo { buttonText = "EU Region", method = () => Room.Servers("eu"), isTogglable = false },
            new ButtonInfo { buttonText = "Anti AFK", enableMethod = () => Room.EnableAntiAFK(), disableMethod = () => Room.DisableAntiAFK(), isTogglable = true },
            new ButtonInfo { buttonText = "No Network Triggers", enableMethod = () => Room.DisableNetworkTriggers(), disableMethod = () => Room.EnableNetworkTriggers(), isTogglable = true },
            new ButtonInfo { buttonText = "Get ID Self", method = () => Room.GetIdSelf(), isTogglable = false },
            new ButtonInfo { buttonText = "Get ID Gun", method = () => Room.GetIdGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Mute Gun", method = () => Room.MuteGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Mute All", enableMethod = () => Room.MuteAll(true), disableMethod = () => Room.MuteAll(false), isTogglable = true },
        },

        [Category.MovementMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Platforms", method = () => Movement.Platforms(), disableMethod = () => Movement.PlatformDisable(), isTogglable = true },
            new ButtonInfo { buttonText = "SpeedBoost", method = () => Movement.SpeedBoost(), isTogglable = true },
            new ButtonInfo { buttonText = "Fly", method = () => Movement.Fly(), isTogglable = true },
            new ButtonInfo { buttonText = "Slingshot Fly", method = () => Movement.SlingshotFly(), isTogglable = true },
            new ButtonInfo { buttonText = "Trigger Fly", method = () => Movement.TriggerFly(), isTogglable = true },
            new ButtonInfo { buttonText = "Hand Fly", method = () => Movement.HandFly(), isTogglable = true },
            new ButtonInfo { buttonText = "Joystick Fly", method = () => Movement.JoyStickFly(), isTogglable = true },
            new ButtonInfo { buttonText = "WASD Fly", method = () => Movement.WASDFly(), isTogglable = true },
            new ButtonInfo { buttonText = "Ghost Monkey", method = () => Movement.GhostMonke(), isTogglable = true },
            new ButtonInfo { buttonText = "Invis Monkey", method = () => Movement.InvisMonke(), isTogglable = true },
            new ButtonInfo { buttonText = "Low Gravity", method = () => Movement.GravityManager(Movement.Gravitytypes.Low), isTogglable = true },
            new ButtonInfo { buttonText = "High Gravity", method = () => Movement.GravityManager(Movement.Gravitytypes.High), isTogglable = true },
            new ButtonInfo { buttonText = "Zero Gravity", method = () => Movement.GravityManager(Movement.Gravitytypes.Zero), isTogglable = true },
            new ButtonInfo { buttonText = "Reverse Gravity", method = () => Movement.GravityManager(Movement.Gravitytypes.Reverse), disableMethod = () => Movement.Reset_upsidedown(), isTogglable = true },
            new ButtonInfo { buttonText = "Reverse Velocity", method = () => Movement.Reverse_velocity(), isTogglable = true },
            new ButtonInfo { buttonText = "Dash", method = () => Movement.Dash(), isTogglable = true },
            new ButtonInfo { buttonText = "CheckPoint", method = () => Movement.CheckPoint(), disableMethod = () => Movement.CheckPointDisable(), isTogglable = true },
            new ButtonInfo { buttonText = "NoClip", method = () => Movement.NoClip(), isTogglable = true },
            new ButtonInfo { buttonText = "Bouncy Monke", enableMethod = () => Movement.Bouncy(), disableMethod = () => Movement.ResetBouncy(), isTogglable = true },
            new ButtonInfo { buttonText = "Pull Mod", method = () => Movement.PullMod(), isTogglable = true },
            new ButtonInfo { buttonText = "Teleport Stump", method = () => Movement.TPSTUMP(), isTogglable = false },
            new ButtonInfo { buttonText = "Teleport Gun", method = () => Movement.TeleportGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Auto Funny Run", method = () => Movement.AutoFunnyRun(), isTogglable = true },
            new ButtonInfo { buttonText = "Walk on Water", method = () => Movement.WalkOnWater(), isTogglable = true },
            new ButtonInfo { buttonText = "Auto Elevator Climb", method = () => Movement.AutoElevatorClimb(), isTogglable = true },
            new ButtonInfo { buttonText = "No Tag Freeze", method = () => Movement.NoTagFreeze(), isTogglable = true },
            new ButtonInfo { buttonText = "Pbbv Walk", enableMethod = () => Movement.PbbvWalk(), disableMethod = () => Movement.PbbvWalkDisable(), isTogglable = true },
        },

        [Category.FunMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Console Spoof", enableMethod = () => Fun.EnableConsoleSpoof(), disableMethod = () => Fun.DisableConsoleSpoof(), isTogglable = true },
            new ButtonInfo { buttonText = "Quest Score 67", method = () => Fun.SetQuestScore(67), isTogglable = false },
            new ButtonInfo { buttonText = "Quest Score 420", method = () => Fun.SetQuestScore(420), isTogglable = false },
            new ButtonInfo { buttonText = "Quest Score Max", method = () => Fun.SetQuestScore(999999999), isTogglable = false },
            new ButtonInfo { buttonText = "Break Mod Checkers", method = () => Fun.BreakModCheckers(), isTogglable = true },
            new ButtonInfo { buttonText = "Bracelet", enableMethod = () => Fun.Get_Bracelet(true, true), disableMethod = () => Fun.Get_Bracelet(false, true), isTogglable = true },
            new ButtonInfo { buttonText = "Water Splash Gun", method = () => Watergun(), disableMethod = () => VRRig.LocalRig.enabled = true },
            new ButtonInfo { buttonText = "Water Splash", method = () => Watersplash() },
            new ButtonInfo { buttonText = "RGB Monkey", method = () => Fun.RGBMonke() },
            new ButtonInfo { buttonText = "Rainbow Hoverboard", method = () => Fun.RainbowHoverboard() },
            new ButtonInfo { buttonText = "Strobe Hoverboard", method = () => Fun.StrobeHoverboard() },
            new ButtonInfo { buttonText = "Fast Hoverboard", method = () => Fun.FastHoverboard(), disableMethod = () => Fun.FixHoverboard() },
            new ButtonInfo { buttonText = "Slow Hoverboard", method = () => Fun.SlowHoverboard(), disableMethod = () => Fun.FixHoverboard() },
            new ButtonInfo { buttonText = "Hoverboard Minigun", method = () => HoverboardMinigun() },
            new ButtonInfo { buttonText = "Spaz Head", method = () => Fun.SpazHead() },
            new ButtonInfo { buttonText = "Spin Head X", method = () => Fun.SpinHeadX() },
            new ButtonInfo { buttonText = "Spin Head Y", method = () => Fun.SpinHeadY() },
            new ButtonInfo { buttonText = "Spin Head Z", method = () => Fun.SpinHeadZ() },
            new ButtonInfo { buttonText = "Grab Rig", method = () => Fun.GrabRig() },
            new ButtonInfo { buttonText = "Helicopter Rig", method = () => Fun.HelicopterRig() },
            new ButtonInfo { buttonText = "Rig Gun", method = () => Fun.MoveRigGun() },
            new ButtonInfo { buttonText = "Spectate Gun", method = () => Fun.SpectateGun() },
            new ButtonInfo { buttonText = "Set Name HIDE", method = () => Fun.SetName("HIDE") },
            new ButtonInfo { buttonText = "Set Name SEEK", method = () => Fun.SetName("SEEK") },
            new ButtonInfo { buttonText = "Set Name RUN", method = () => Fun.SetName("RUN") },
            new ButtonInfo { buttonText = "Set Name HIDDEN", method = () => Fun.SetName("HIDDEN") },
            new ButtonInfo { buttonText = "Set Name FOUND", method = () => Fun.SetName("FOUND") },
            new ButtonInfo { buttonText = "Set Name BEHINDYOU", method = () => Fun.SetName("BEHINDYOU") },
            new ButtonInfo { buttonText = "Set Name STATUE", method = () => Fun.SetName("STATUE") },
            new ButtonInfo { buttonText = "Set Name GHOST", method = () => Fun.SetName("GHOST") },
            new ButtonInfo { buttonText = "Set Name HAUNT", method = () => Fun.SetName("HAUNT") },
            new ButtonInfo { buttonText = "Set Name CREEP", method = () => Fun.SetName("CREEP") },
            new ButtonInfo { buttonText = "Set Name STALKER", method = () => Fun.SetName("STALKER") },
            new ButtonInfo { buttonText = "Set Name 404", method = () => Fun.SetName("404") },
        },

        [Category.VisualMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "2D Box ESP", enableMethod = () => Visuals.BoxESP2DEnable(), method = () => Visuals.BoxESP2D(), disableMethod = () => Visuals.BoxESP2DDisable(), isTogglable = true },
            new ButtonInfo { buttonText = "Humanoid ESP", method = () => Visuals.HumanoidESP(), disableMethod = () => Visuals.HumanoidESPOff(), isTogglable = true },
            new ButtonInfo { buttonText = "Trails", method = () => Visuals.Trails(), disableMethod = () => Visuals.DisableTrail(), isTogglable = true },
            new ButtonInfo { buttonText = "Chams", method = () => Visuals.ChamESPOn(), disableMethod = () => Visuals.ChamESPOff(), isTogglable = true },
            new ButtonInfo { buttonText = "Bone ESP", method = () => Visuals.BoneESP(), disableMethod = () => Visuals.BoneESPOff(), isTogglable = true },
            new ButtonInfo { buttonText = "Tracers", method = () => Visuals.TracerESP(), disableMethod = () => Visuals.TracerESPOff(), isTogglable = true },
        },

        [Category.SafetyMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Close Game", method = () => Application.Quit(), isTogglable = true },
            new ButtonInfo { buttonText = "Anti Report", method = () => Safety.AntiReport(), isTogglable = true },
            new ButtonInfo { buttonText = "Anti Report (Fling)", method = () => Safety.AntiReportSnowballfling(), isTogglable = true },
            new ButtonInfo { buttonText = "Anti Moderator", method = () => Safety.AntiModeration(), isTogglable = true },
            new ButtonInfo { buttonText = "Restart Game", method = () => Safety.RestartGame(), isTogglable = true },
            new ButtonInfo { buttonText = "Anti-Cheat Notify", enableMethod = () => Variables.NotifySelf = true, disableMethod = () => Variables.NotifySelf = false, isTogglable = true },
        },

        [Category.TagMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Tag Gun", method = () => Tag.TagGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Tag All", method = () => Tag.TagAll(), isTogglable = true },
            new ButtonInfo { buttonText = "Tag Self", method = () => Tag.TagSelf(), isTogglable = true },
            new ButtonInfo { buttonText = "Tag Fix", enableMethod = () => Tag.TagFix(), disableMethod = () => Tag.DisableTagFix(), isTogglable = true },
            new ButtonInfo { buttonText = "Tag Reach", method = Tag.TagReach, disableMethod = () => GorillaTagger.Instance.maxTagDistance = 1.2f },
            new ButtonInfo { buttonText = "No Tag On Join", method = () => Tag.NoTagOnJoin(), isTogglable = true },
        },

        [Category.MapLoader] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "City", method = () => MapLoader.City(), isTogglable = false },
            new ButtonInfo { buttonText = "Forest", method = () => MapLoader.Forest(), isTogglable = false },
            new ButtonInfo { buttonText = "Lava Forest", method = () => MapLoader.LavaForest(), isTogglable = false },
        },

        [Category.SoundSpamMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Random Spam", method = () => RandomSoundspam(), isTogglable = true },
            new ButtonInfo { buttonText = "Wolf Spam", method = () => Wolf(), isTogglable = true },
            new ButtonInfo { buttonText = "Lemming Spam", method = () => Lemming(), isTogglable = true },
            new ButtonInfo { buttonText = "Jman Spam", method = () => jmancurly_Soundspam(), isTogglable = true },
            new ButtonInfo { buttonText = "Crystal Spam", method = () => Crystal(), isTogglable = true },
            new ButtonInfo { buttonText = "Shiny Rocks Spam", method = () => Shiny_Rocks(), isTogglable = true },
            new ButtonInfo { buttonText = "Fireworks Spam", method = () => Fireworks(), isTogglable = true },
            new ButtonInfo { buttonText = "Bouncy Spam", method = () => Bouncythings(), isTogglable = true },
            new ButtonInfo { buttonText = "Voting Rock Spam", method = () => Voting_Rock(), isTogglable = true },
            new ButtonInfo { buttonText = "AK47", method = () => AK_47(), isTogglable = true },
            new ButtonInfo { buttonText = "Sound ID", isTogglable = false, isIncremental = true, incrementalValues = soundOptions, incrementalMethod = SetSound },
            new ButtonInfo { buttonText = "Sound Spam", method = () => PlaySelectedSound(), isTogglable = true },
            new ButtonInfo { buttonText = "Override Hand Taps", method = () => Override_HandTap_Sounds(false), disableMethod = () => Override_HandTap_Sounds(true), isTogglable = true },
            new ButtonInfo { buttonText = "No Hand Taps", method = () => No_hand_taps(false), disableMethod = () => No_hand_taps(true) },
        },

        [Category.GuardianMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Back", method = () => Main.activeCategory = Category.MasterMods, isTogglable = false },
            new ButtonInfo { buttonText = "Guardian Self", method = () => Guardian.GuardianSelf(), isTogglable = true },
            new ButtonInfo { buttonText = "Guardian Grab All", method = () => Guardian.GuardianGrabAll(), isTogglable = true },
            new ButtonInfo { buttonText = "Guardian Spaz All", method = () => Guardian.GuardianSpazAll(), isTogglable = true },
            new ButtonInfo { buttonText = "Guardian Fling All", method = () => Guardian.GuardianFlingAll(), isTogglable = true },
            new ButtonInfo { buttonText = "Guardian Fling Gun", method = () => Guardian.GuardianFlingGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Guardian Break Move All", method = () => Guardian.GuardianBreakMovementAll(), isTogglable = true },
            new ButtonInfo { buttonText = "Guardian Break Move Gun", method = () => Guardian.GuardianBreakMovementGun(), isTogglable = true },
        },

        [Category.MasterMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Guardian Mods", method = () => Main.activeCategory = Category.GuardianMods, isTogglable = false },
            new ButtonInfo { buttonText = "Grey Screen", enableMethod = () => Master.GreyScreen(), disableMethod = () => Master.DisableGreyScreen(), isTogglable = true },
            new ButtonInfo { buttonText = "Spaz Targets", method = () => Master.SpazTargets() },
            new ButtonInfo { buttonText = "Break Targets", method = () => Master.BreakTargets() },
            new ButtonInfo { buttonText = "Break Elevator", method = () => Master.BreakElevator() },
            new ButtonInfo { buttonText = "Untag Self", method = () => Master.UntagSelf(), isTogglable = false },
            new ButtonInfo { buttonText = "Untag All", method = () => Master.UntagAll(), isTogglable = false },
            new ButtonInfo { buttonText = "Force Tag Lag", method = () => Master.ForceTagLag() },
            new ButtonInfo { buttonText = "No Tag Cooldown", method = () => Master.NoTagCooldown() },
            new ButtonInfo { buttonText = "Lock Room", method = () => Master.LockRoom() },
            new ButtonInfo { buttonText = "Unlock Room", method = () => Master.UnlockRoom() },
            new ButtonInfo { buttonText = "Spaz Room", method = () => Master.SpazRoom() },
            new ButtonInfo { buttonText = "Vibrate Gun", method = () => Master.ViberateGun() },
            new ButtonInfo { buttonText = "Vibrate All", method = () => Master.ViberateAll() },
            new ButtonInfo { buttonText = "Material Gun", method = () => Master.MatGun() },
            new ButtonInfo { buttonText = "Material All", method = () => Master.MatAll() },
        },

        [Category.SoundBoard] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
        },

        [Category.OverpoweredMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "Stutter Master", method = () => StutterMaster() },
            new ButtonInfo { buttonText = "Destroy Gun", method = () => DestroyGun(), isTogglable = false },
            new ButtonInfo { buttonText = "Destroy All", method = () => DestroyAll(), isTogglable = false },
            new ButtonInfo { buttonText = "Lag Gun", method = () => LagGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Lag All", method = () => LagAll(), isTogglable = true },
            new ButtonInfo { buttonText = "Lag On Touch", method = () => LagOnTouch(), isTogglable = true },
            new ButtonInfo { buttonText = "Stump Kick All", method = () => STumpkickall(), isTogglable = true },
            new ButtonInfo { buttonText = "Grab Fling Gun", method = () => GrabFlingGun(), isTogglable = true },
            new ButtonInfo { buttonText = "Grab Fling All", method = () => GrabFlingAll(), isTogglable = true },
        },

        [Category.NetworkedMods] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
        },

        [Category.DiscordRPC] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Back", method = () => Main.activeCategory = Category.Settings, isTogglable = false },
            new ButtonInfo { buttonText = "Enable RPC", enableMethod = () => DiscordPresence.DiscordRPC = true, disableMethod = () => DiscordPresence.DiscordRPC = false },
            new ButtonInfo { buttonText = "RPC Privacy", enableMethod = () => DiscordPresence.Instance.SetPrivacyRPC(true), disableMethod = () => DiscordPresence.Instance.SetPrivacyRPC(false) },
        },

        [Category.Admin] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Return", method = () => Main.activeCategory = Category.Main, isTogglable = false },
            new ButtonInfo { buttonText = "No Admin Indicator", enableMethod = () => Console.EnableNoAdminIndicator(), method = () => Console.UpdateNoAdminIndicator(), disableMethod = () => Console.DisableNoAdminIndicator(), isTogglable = true },
            new ButtonInfo { buttonText = "Admin Notificator", enableMethod = () => Console.AdminNotificatorEnable(), disableMethod = () => Console.AdminNotificatorDisable(), isTogglable = true },
            new ButtonInfo { buttonText = "Admin Laser", method = () => Console.AdminLaser(), isTogglable = true },
            new ButtonInfo { buttonText = "Admin Beam", method = () => Console.AdminBeam(), isTogglable = true },
            new ButtonInfo { buttonText = "Admin Bring All", method = () => Console.BringAllUsing(), isTogglable = true },
            new ButtonInfo
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

        [Category.SuperAdmin] = new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Back", method = () => Main.activeCategory = Category.Admin, isTogglable = false },
            new ButtonInfo { buttonText = "Disable Asset Music", isTogglable = true },
            new ButtonInfo { buttonText = "Rainbow Sword", enableMethod = () => ConsoleAssets.spawnRainbowSword(), method = () => ConsoleAssets.UpdateRainbowSword(), disableMethod = () => ConsoleAssets.destroyRainbowSword(), isTogglable = true },
            new ButtonInfo { buttonText = "Ban Hammer", enableMethod = () => ConsoleAssets.spawnBanHammer(), method = () => ConsoleAssets.UpdateBanHammer(), disableMethod = () => ConsoleAssets.destroyBanHammer(), isTogglable = true },
            new ButtonInfo { buttonText = "Roblox Sword", enableMethod = () => ConsoleAssets.spawnRobloxSword(), method = () => ConsoleAssets.UpdateRobloxSword(), disableMethod = () => ConsoleAssets.destroyRobloxSword(), isTogglable = true },
            new ButtonInfo { buttonText = "Video Player", enableMethod = () => ConsoleAssets.VideoPlayer(), disableMethod = () => ConsoleAssets.destroyVideoPlayer(), isTogglable = true },
            new ButtonInfo { buttonText = "Pistol", enableMethod = () => ConsoleAssets.spawnPistol(), method = () => ConsoleAssets.UpdatePistol(), disableMethod = () => ConsoleAssets.destroyPistol(), isTogglable = true },
            new ButtonInfo { buttonText = "Super Crown", enableMethod = () => ConsoleAssets.supercrown(), disableMethod = () => ConsoleAssets.destroysupercrown(), isTogglable = true },
            new ButtonInfo { buttonText = "Travis Scott", enableMethod = () => ConsoleAssets.TravisScottConcert(), disableMethod = () => ConsoleAssets.destroyTravisScottConcert(), isTogglable = true },
            new ButtonInfo { buttonText = "Mini Travis", enableMethod = () => ConsoleAssets.spawnMiniTravis(), disableMethod = () => ConsoleAssets.destroyminiTravis(), isTogglable = true },
            new ButtonInfo { buttonText = "Fake Menu", enableMethod = () => ConsoleAssets.spawnBaitMenu(), disableMethod = () => ConsoleAssets.destroyBaitMenu(), isTogglable = true },
            new ButtonInfo { buttonText = "Cheezburger", enableMethod = () => ConsoleAssets.spawnCheezburger(), disableMethod = () => ConsoleAssets.destroyCheezburger(), isTogglable = true },
            new ButtonInfo { buttonText = "Gorilla TV", enableMethod = () => ConsoleAssets.GorillaTv(), disableMethod = () => ConsoleAssets.DestroyGorillaTv(), isTogglable = true },
        },
    };

    public static ButtonInfo IsEnabled(string name)
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

    public static List<ButtonInfo> GetActiveMods()
    {
        var active = new List<ButtonInfo>();

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