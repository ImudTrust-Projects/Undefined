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
            Category("Enabled", Category.EnabledMods),
            Category("Favourites", Category.FavouriteMods),
            Category("Room", Category.RoomMods),
            Category("Movement", Category.MovementMods),
            Category("Fun", Category.FunMods),
            Category("Rig", Category.RigMods),
            Category("Visual", Category.VisualMods),
            Category("Safety", Category.SafetyMods),
            Category("Advantages", Category.AdvantagesMods),
            Category("Map Loader", Category.MapLoader),
            Category("Sound Spam", Category.SoundSpamMods),
            Category("SoundBoard", Category.SoundBoard),
            Category("Master", Category.MasterMods),
            Category("Overpowered", Category.OverpoweredMods)
        ],

        [Category.Settings] = new ModButtonInfo[]
        {
            Back(Category.Main),
            Category("Menu Settings", Category.MenuSettings),
            Category("Discord RPC", Category.DiscordRPC),
            Category("Themes", Category.Themes),
            Category("Special Menu Settings", Category.SpecialMenu),
            new ModButtonInfo("Custom Board Color", () => customBoardColor = true, () => customBoardColor = false),
            new ModButtonInfo("PC Button Click", PcButtonClick) { disableMethod = StopPcButtonClick },
            new ModButtonInfo("Searching", () => searchingEnabled = true, () => { searchingEnabled = false; if (KeyboardManager.isSearching) KeyboardManager.CloseKeyboard(); }),
        },

        [Category.SpecialMenu] = new ModButtonInfo[]
        {
            Back(Category.Settings),
            new ModButtonInfo("Throwable Menu", () => throwableMenu = true, () => throwableMenu = false),
            new ModButtonInfo("Colliders", () => menuColliders = true, () => menuColliders = false),
            new ModButtonInfo("Zero Gravity Menu", () => zeroGravityMenu = true, () => zeroGravityMenu = false),
            new ModButtonInfo("Explode Menu", () => explodeMenu = true, () => explodeMenu = false),
            new ModButtonInfo("Disappear Animation", () => disappearAnimation = true, () => disappearAnimation = false),
            new ModButtonInfo("Keep Menu", () => keepMenu = true, () => keepMenu = false),
        },

        [Category.MenuSettings] = new ModButtonInfo[]
        {
            Back(Category.Settings),
            new ModButtonInfo("Right Hand", () => rightHanded = true, () => rightHanded = false, null),
            new ModButtonInfo("Disconnect Button", () => disconnectButton = true, () => disconnectButton = false, null) { enabled = disconnectButton },
            new ModButtonInfo("ArrayList", () => ArrayListEnabled = true, () => ArrayListEnabled = false, null) { enabled = true },
            new ModButtonInfo("Room Notifications", () => NotificationLib.RoomNotifications = true, () => NotificationLib.RoomNotifications = false, null) { enabled = true },
            new ModButtonInfo("Button Sound", SoundSettings.buttonSoundOptions, SoundSettings.SetButtonSound, 2),
            new ModButtonInfo("Font", MENUSETTINGS.Settings.fontOptions, MENUSETTINGS.Settings.SetFont, 2),
            new ModButtonInfo("Platform Mode", Movement.PlatformMode, Movement.SetPlatformMode),
            new ModButtonInfo("Speed Mode", Movement.SpeedBoostNames, Movement.SetSpeedBoost),
        },

        [Category.Themes] = new ModButtonInfo[]
        {
            Back(Category.Settings),
            Category("Colors", Category.ThemeColors),
            Category("Custom Colors", Category.CustomColors),
            Category("Button Types", Category.ButtonTypes),
            new ModButtonInfo("Wrist Menu", () => Themes.wristMenu = true, () => Themes.wristMenu = false, null),
            new ModButtonInfo("Thick Menu", Themes.UseThickMenu, () => Themes.thickMenu = false, null),
            new ModButtonInfo("Thin Menu", Themes.UseThinMenu, () => Themes.thinMenu = false, null),
        },

        [Category.ButtonTypes] = new ModButtonInfo[]
        {
            Back(Category.Themes),
            new ModButtonInfo("Triggers", Themes.UseTriggers, Themes.TriggersOff) { enabled = true },
            new ModButtonInfo("Under Buttons", Themes.UseUnderButtons, Themes.UnderButtonsOff, null),
            new ModButtonInfo("Side Buttons", Themes.UseSideButtons, Themes.SideButtonsOff, null),
        },

        [Category.ThemeColors] = new ModButtonInfo[]
        {
            Back(Category.Themes),
            new ModButtonInfo("R", Themes.ColorValues, Themes.SetRed, 1),
            new ModButtonInfo("G", Themes.ColorValues, Themes.SetGreen, 1),
            new ModButtonInfo("B", Themes.ColorValues, Themes.SetBlue, 1),
            new ModButtonInfo("Default", () => Themes.SetColor(25, 25, 25), false),
            new ModButtonInfo("Red", () => Themes.SetColor(200, 25, 25), false),
            new ModButtonInfo("Orange", () => Themes.SetColor(225, 100, 0), false),
            new ModButtonInfo("Yellow", () => Themes.SetColor(200, 175, 0), false),
            new ModButtonInfo("Green", () => Themes.SetColor(25, 150, 50), false),
            new ModButtonInfo("Cyan", () => Themes.SetColor(0, 150, 175), false),
            new ModButtonInfo("Blue", () => Themes.SetColor(25, 75, 200), false),
            new ModButtonInfo("Purple", () => Themes.SetColor(100, 25, 175), false),
            new ModButtonInfo("Pink", () => Themes.SetColor(200, 50, 125), false),
            new ModButtonInfo("Black", () => Themes.SetColor(0, 0, 0), false),
            new ModButtonInfo("Save Color", Themes.SaveColor, false),
        },

        [Category.CustomColors] = new ModButtonInfo[]
        {
            Back(Category.Themes),
        },

        [Category.EnabledMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.FavouriteMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.RoomMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Join Menu", () => Room.JoinRoom("[Undefined]"), false),
            new ModButtonInfo("Join Custom Room", () => Room.JoinCustomRoom(), false),
            new ModButtonInfo("Disconnect", () => Room.Disconnect(), false),
            new ModButtonInfo("Join Random", () => Room.JoinRandomPublic(), false),
            new ModButtonInfo("Primary Disconnect", () => Room.PrimaryDisconnect()),
            new ModButtonInfo("US Region", () => Room.Servers("us")),
            new ModButtonInfo("USW Region", () => Room.Servers("usw")),
            new ModButtonInfo("EU Region", () => Room.Servers("eu")),
            new ModButtonInfo("Anti AFK", () => Room.EnableAntiAFK(), () => Room.DisableAntiAFK(), null),
            new ModButtonInfo("No Network Triggers", () => Room.DisableNetworkTriggers(), () => Room.EnableNetworkTriggers(), null),
            new ModButtonInfo("Get ID Self", () => Room.GetIdSelf(), false),
            new ModButtonInfo("Get ID Gun", () => Room.GetIdGun()),
            new ModButtonInfo("Unlock VIM", () => Room.UnlockVIM(), false),
            new ModButtonInfo("Mute Gun", () => Room.MuteGun()),
            new ModButtonInfo("Mute All", () => Room.MuteAll(true), () => Room.MuteAll(false), null),
        },

        [Category.MovementMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Platforms", () => Movement.Platforms(), () => Movement.PlatformDisable()),
            new ModButtonInfo("SpeedBoost", () => Movement.SpeedBoost()),
            new ModButtonInfo("Amplified Monke", () => Movement.AmplifiedMonke()),
            new ModButtonInfo("Fly", () => Movement.Fly()),
            new ModButtonInfo("Slingshot Fly", () => Movement.SlingshotFly()),
            new ModButtonInfo("Trigger Fly", () => Movement.TriggerFly()),
            new ModButtonInfo("Hand Fly", () => Movement.HandFly()),
            new ModButtonInfo("Joystick Fly", () => Movement.JoyStickFly()),
            new ModButtonInfo("WASD Fly", () => Movement.WASDFly()),
            new ModButtonInfo("Low Gravity", () => Movement.GravityManager(Movement.Gravitytypes.Low), () => Movement.GravityManager(Movement.Gravitytypes.Reset)),
            new ModButtonInfo("High Gravity", () => Movement.GravityManager(Movement.Gravitytypes.High), () => Movement.GravityManager(Movement.Gravitytypes.Reset)),
            new ModButtonInfo("Zero Gravity", () => Movement.GravityManager(Movement.Gravitytypes.Zero), () => Movement.GravityManager(Movement.Gravitytypes.Reset)),
            new ModButtonInfo("Reverse Gravity", () => Movement.GravityManager(Movement.Gravitytypes.Reverse), () => Movement.GravityManager(Movement.Gravitytypes.Reset)),
            new ModButtonInfo("Reverse Velocity", () => Movement.Reverse_velocity(), true),
            new ModButtonInfo("Dash", () => Movement.Dash()),
            new ModButtonInfo("Up And Down", () => Movement.UpAndDown()),
            new ModButtonInfo("CheckPoint", () => Movement.CheckPoint(), () => Movement.CheckPointDisable()),
            new ModButtonInfo("NoClip", () => Movement.NoClip()),
            new ModButtonInfo("Bouncy Monke", () => Movement.Bouncy(), () => Movement.ResetBouncy(), null),
            new ModButtonInfo("Pull Mod", () => Movement.PullMod()),
            new ModButtonInfo("Teleport Stump", () => Movement.TPSTUMP(), false),
            new ModButtonInfo("Teleport Gun", () => Movement.TeleportGun()),
            new ModButtonInfo("Auto Funny Run", () => Movement.AutoFunnyRun()),
            new ModButtonInfo("Walk on Water", () => Movement.WalkOnWater()),
            new ModButtonInfo("Pbbv Walk", () => Movement.PbbvWalk(), () => Movement.PbbvWalkDisable(), null),
        },

        [Category.FunMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Open Elevator", () => Fun.OpenElevator()),
            new ModButtonInfo("Close Elevator", () => Fun.CloseElevator()),
            new ModButtonInfo("Console Spoof", Fun.EnableConsoleSpoof, Fun.DisableConsoleSpoof, null),
            new ModButtonInfo("Quest Score 67", Fun.SetQuestScore67, false),
            new ModButtonInfo("Quest Score 420", Fun.SetQuestScore420, false),
            new ModButtonInfo("Quest Score Max", Fun.SetQuestScoreMax, false),
            new ModButtonInfo("Custom Quest Score Max", Fun.SetCustomQuestScore, false),
            new ModButtonInfo("Break Mod Checkers", Fun.BreakModCheckers),
            new ModButtonInfo("Bracelet", Fun.EnableBracelet, Fun.DisableBracelet, null),
            new ModButtonInfo("Water Splash Gun", () => Watergun(), () => VRRig.LocalRig.enabled = true),
            new ModButtonInfo("Water Splash", () => Watersplash()),
            new ModButtonInfo("RGB Monkey", () => Fun.RGBMonke()),
            new ModButtonInfo("Rainbow Hoverboard", () => Fun.RainbowHoverboard()),
            new ModButtonInfo("Strobe Hoverboard", () => Fun.StrobeHoverboard()),
            new ModButtonInfo("Fast Hoverboard", Fun.FastHoverboard, Fun.FixHoverboard, null),
            new ModButtonInfo("Slow Hoverboard", Fun.SlowHoverboard, Fun.FixHoverboard, null),
            new ModButtonInfo("Hoverboard Minigun", () => HoverboardMinigun()),
            new ModButtonInfo("Spectate Gun", () => Fun.SpectateGun()),
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
        
        [Category.RigMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Ghost Monkey", () => Rig.GhostMonke()),
            new ModButtonInfo("Invis Monkey", () => Rig.InvisMonke()),
            new ModButtonInfo("Spaz Head", () => Rig.SpazHead()),
            new ModButtonInfo("Spin Head X", () => Rig.SpinHeadX()),
            new ModButtonInfo("Spin Head Y", () => Rig.SpinHeadY()),
            new ModButtonInfo("Spin Head Z", () => Rig.SpinHeadZ()),
            new ModButtonInfo("Upside Down Head", Rig.UpsideDownHead, Rig.ResetHead, null),
            new ModButtonInfo("Backwards Head", Rig.BackwardsHead, Rig.ResetHead, null),
            new ModButtonInfo("Grab Rig", () => Rig.GrabRig()),
            new ModButtonInfo("Ascend Monke", () => Rig.AscendMonke()),
            new ModButtonInfo("Helicopter Rig", () => Rig.HelicopterRig()),
            new ModButtonInfo("Clap", () => Rig.Clap(), () => Rig.ResetRig(), null),
            new ModButtonInfo("Flap Arms", () => Rig.FlapArms(), () => Rig.ResetRig(), null),
            new ModButtonInfo("Wave", () => Rig.Wave(), () => Rig.ResetRig(), null),
            new ModButtonInfo("Rig Gun", () => Rig.MoveRigGun()),
            new ModButtonInfo("Spider Rig", () => Rig.Spider(), () => Rig.DisableRecRoomBody(), null),
            new ModButtonInfo("Full Body Tracking", () => Rig.FullBodyTracking(), () => Rig.DisableRecRoomBody(), null),
            new ModButtonInfo("Inverse Spider", () => Rig.InverseSpider(), () => Rig.DisableRecRoomBody(), null),
            new ModButtonInfo("RecRoom Rig", () => Rig.RecRoomRig(), () => Rig.DisableRecRoomBody(), null),
            new ModButtonInfo("JoystickRot", () => Rig.JoystickRot(), () => Rig.DisableRecRoomBody(), null),
            new ModButtonInfo("RecRoom Torso", () => Rig.RecRoomTorso(), () => Rig.DisableRecRoomBody(), null),
        },

        [Category.VisualMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("2D Box ESP", () => Visuals.BoxESP2DEnable(), () => Visuals.BoxESP2DDisable(), () =>  Visuals.BoxESP2D()),
            new ModButtonInfo("Humanoid ESP", () => Visuals.HumanoidESP(), () => Visuals.HumanoidESPOff()),
            new ModButtonInfo("Trails", () => Visuals.Trails(), () => Visuals.DisableTrail()),
            new ModButtonInfo("Chams", () => Visuals.ChamESPOn(), () => Visuals.ChamESPOff()),
            new ModButtonInfo("Bone ESP", () => Visuals.BoneESP(), () => Visuals.BoneESPOff()),
            new ModButtonInfo("Tracers", () => Visuals.TracerESP(), () => Visuals.TracerESPOff()),
            new ModButtonInfo("EnableTerraformer", () => Master.EnableTerraformer(), () => Master.DisableTerraformer(), null),
            
        },

        [Category.SafetyMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Close Game", () => Application.Quit(), false),
            new ModButtonInfo("Anti Report", () => Safety.AntiReport()),
            //new ModButtonInfo("Anti Report (Fling)", () => Safety.AntiReportSnowballfling()),
            new ModButtonInfo("Anti Moderator", () => Safety.AntiModeration()),
            new ModButtonInfo("Restart Game", () => Safety.RestartGame(), false),
            new ModButtonInfo("Anti-Cheat Notify", () => Variables.NotifySelf = true, () => Variables.NotifySelf = false),
        },

        [Category.AdvantagesMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Tag Gun", () => Advantages.TagGun()),
            new ModButtonInfo("Tag All", () => Advantages.TagAll()),
            new ModButtonInfo("Tag Self", () => Advantages.TagSelf()),
            new ModButtonInfo("Tag Fix", () => Advantages.TagFix(), () => Advantages.DisableTagFix()),
            new ModButtonInfo("Tag Reach", Advantages.TagReach, () => GorillaTagger.Instance.maxTagDistance = 1.2f),
            new ModButtonInfo("No Tag On Join", () => Advantages.NoTagOnJoin()),
            new ModButtonInfo("45 fps", () => Advantages.FPS(true, 45),() => Advantages.FPS(false) ),
            new ModButtonInfo("60 fps", () => Advantages.FPS(true, 60),() => Advantages.FPS(false) ),
            new ModButtonInfo("90 fps", () => Advantages.FPS(true, 90),() => Advantages.FPS(false) ),
            new ModButtonInfo("120 fps", () => Advantages.FPS(true, 120),() => Advantages.FPS(false) ),
            new ModButtonInfo("Unlock fps", () => Advantages.UnlockFps(true),() => Advantages.UnlockFps(false) ),
            new ModButtonInfo("No Tag Freeze", () => Advantages.NoTagFreeze()),
        },

        [Category.MapLoader] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Tutorial", () => MapLoader.Tutorial(), false),
            new ModButtonInfo("City", () => MapLoader.City(), false),
            new ModButtonInfo("Forest", () => MapLoader.Forest(), false),
            new ModButtonInfo("Canyon", () => MapLoader.Canyon(), false),
            new ModButtonInfo("Cave", () => MapLoader.Cave(), false),
            new ModButtonInfo("Beach", () => MapLoader.Beach(), false),
            new ModButtonInfo("Mall", () => MapLoader.Mall(), false),
            new ModButtonInfo("Lava Forest", () => MapLoader.LavaForest(), false),
            new ModButtonInfo("Metropolis", () => MapLoader.Metropolis(), false),
            new ModButtonInfo("Basement", () => MapLoader.Basement(), false),
            new ModButtonInfo("Critters", () => MapLoader.Critters(), false),
            new ModButtonInfo("Monke Blocks", () => MapLoader.MonkeBlocks(), false),
            new ModButtonInfo("Mountain", () => MapLoader.Mountain(), false),
            new ModButtonInfo("Ranked", () => MapLoader.Ranked(), false),
            new ModButtonInfo("SkatePark", () => MapLoader.SkatePark(), false),
            new ModButtonInfo("Sky Jungle", () => MapLoader.SkyJungle(), false),
            new ModButtonInfo("Studio", () => MapLoader.Studio(), false),
        },

        [Category.SoundSpamMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("Random Spam", () => RandomSoundspam()),
            new ModButtonInfo("Wolf Spam", () => Wolf()),
            new ModButtonInfo("Lemming Spam", () => Lemming()),
            new ModButtonInfo("Jman Spam", () => jmancurly_Soundspam()),
            new ModButtonInfo("Crystal Spam", () => Crystal()),
            new ModButtonInfo("Shiny Rocks Spam", () => Shiny_Rocks()),
            new ModButtonInfo("Fireworks Spam", () => Fireworks()),
            new ModButtonInfo("Bouncy Spam", () => Bouncythings()),
            new ModButtonInfo("Voting Rock Spam", () => Voting_Rock()),
            new ModButtonInfo("AK47", () => AK_47()),
            new ModButtonInfo("Sound ID", soundOptions, SetSound),
            new ModButtonInfo("Sound Spam", () => PlaySelectedSound()),
            new ModButtonInfo("Override Hand Taps", () => Override_HandTap_Sounds(false), () => Override_HandTap_Sounds(true)),
            new ModButtonInfo("No Hand Taps", () => No_hand_taps(false), () => No_hand_taps(true)),
        },

        [Category.GuardianMods] = new ModButtonInfo[]
        {
            Back(Category.MasterMods),
            new ModButtonInfo("Guardian Self", () => Guardian.GuardianSelf()),
            new ModButtonInfo("Guardian Gun", () => Guardian.GuardianGun()),
            new ModButtonInfo("UnGuardian Gun", () => Guardian.UnguardianGun()),
            new ModButtonInfo("Guardian Grab All", () => Guardian.GuardianGrabAll()),
            new ModButtonInfo("Guardian Spaz All", () => Guardian.GuardianSpazAll()),
            new ModButtonInfo("Guardian Fling All", () => Guardian.GuardianFlingAll()),
            new ModButtonInfo("Guardian Fling Gun", () => Guardian.GuardianFlingGun()),
            new ModButtonInfo("Guardian Break Move All", () => Guardian.GuardianBreakMovementAll()),
            new ModButtonInfo("Guardian Break Move Gun", () => Guardian.GuardianBreakMovementGun()),
        },

        [Category.MasterMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            Category("Guardian Mods", Category.GuardianMods),
            //new ModButtonInfo("Grey Screen", () => Master.GreyScreen(), () => Master.DisableGreyScreen()), patched
            new ModButtonInfo("Spaz Targets", () => Master.SpazTargets()),
            new ModButtonInfo("Break Targets", () => Master.BreakTargets()),
            new ModButtonInfo("Break Elevator", () => Master.BreakElevator()),
            new ModButtonInfo("Untag Self", () => Master.UntagSelf(), false),
            new ModButtonInfo("Untag All", () => Master.UntagAll(), false),
            new ModButtonInfo("Force Tag Lag", () => Master.ForceTagLag()),
            new ModButtonInfo("No Tag Cooldown", () => Master.NoTagCooldown()),
            new ModButtonInfo("Lock Room", () => Master.LockRoom()),
            new ModButtonInfo("Unlock Room", () => Master.UnlockRoom()),
            new ModButtonInfo("Spaz Room", () => Master.SpazRoom()),
            //new ModButtonInfo("Vibrate Gun", () => Master.ViberateGun()),
            //new ModButtonInfo("Vibrate All", () => Master.ViberateAll()),
            new ModButtonInfo("Material Gun", () => Master.MatGun()),
            new ModButtonInfo("Material All", () => Master.MatAll()),
            new ModButtonInfo("Spawn Lucy", () => Master.SpawnLucy(), false),
            new ModButtonInfo("Spawn Red Lucy", () => Master.SpawnRedLucy(), false),
            new ModButtonInfo("Lucy Chase Gun", () => Master.LucyChaseGun()),
            new ModButtonInfo("Lucy Grab Gun", () => Master.LucyGrabGun()),
            new ModButtonInfo("Lucy Grab All", () => Master.LucyGrabAll()),
            new ModButtonInfo("Move Lucy Gun", () => Master.MoveLucyGun()),
            new ModButtonInfo("Fast Lucy", () => Master.FastLucy(), () => Master.ResetLucySpeed()),
            new ModButtonInfo("Slow Lucy", () => Master.SlowLucy(), () => Master.ResetLucySpeed()),
        },

        [Category.SoundBoard] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.OverpoweredMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
            //new ModButtonInfo("Stutter Master", () => StutterMaster()),
            //new ModButtonInfo("Destroy Gun", () => DestroyGun()),
            //new ModButtonInfo("Destroy All", () => DestroyAll()),
            //new ModButtonInfo("Lag Gun", () => LagGun()),
            //new ModButtonInfo("Lag All", () => LagAll()),
            //new ModButtonInfo("Lag On Touch", () => LagOnTouch()),
            //new ModButtonInfo("Stump Kick All", () => STumpkickall()),
            //new ModButtonInfo("Grab Fling Gun", () => GrabFlingGun()),
            //new ModButtonInfo("Grab Fling All", () => GrabFlingAll()),
            //new ModButtonInfo("SnowBall Fling Gun", () => SnowBallLauncherGun()),
            //new ModButtonInfo("SnowBall Up Up and Away Gun", () => SnowballUpAwayGun()),
        },

        [Category.NetworkedMods] = new ModButtonInfo[]
        {
            Back(Category.Main),
        },

        [Category.DiscordRPC] = new ModButtonInfo[]
        {
            Back(Category.Settings),
            new ModButtonInfo("Enable RPC", () => DiscordPresence.DiscordRPC = true, () => DiscordPresence.DiscordRPC = false, null),
            new ModButtonInfo("RPC Privacy", () => DiscordPresence.Instance.SetPrivacyRPC(true), () => DiscordPresence.Instance.SetPrivacyRPC(false), null),
        },

        [Category.Admin] = new ModButtonInfo[]
        {
            Back(Category.Main),
            new ModButtonInfo("No Admin Indicator", () => Console.EnableNoAdminIndicator(), () => Console.DisableNoAdminIndicator(), () => Console.UpdateNoAdminIndicator()),
            new ModButtonInfo("Admin Notificator", () => Console.AdminNotificatorEnable(), () => Console.AdminNotificatorDisable()),
            new ModButtonInfo("Telekinesis", () => Console.TelekinesisEnable(), () => Console.TelekinesisDisable(), () => Console.Telekinesis()),
            new ModButtonInfo("Silent Kick Gun", () => Console.silkickgun()),
            new ModButtonInfo("Admin Laser", () => Console.AdminLaser()),
            new ModButtonInfo("Admin Beam", () => Console.AdminBeam()),
            new ModButtonInfo("Admin Bring All", () => Console.BringAllUsing()),
            new ModButtonInfo("Admin Strangle", () => Console.AdminStrangle()),
            new ModButtonInfo("Admin Size Changer", () => Console.SizeChanger(), () => Console.DisableSizeChanger()),
            new ModButtonInfo("Conduct Users", () => { Console.EnableAdminMenuUserTags(); Variables.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>().text = "CONSOLE USER LIST"; Variables.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().richText = true; }, null, () => Console.ConsoleOnConduct()),
        },

        [Category.SuperAdmin] = new ModButtonInfo[]
        {
            Back(Category.Admin),
            new ModButtonInfo("Disable Asset Music", null),
            new ModButtonInfo("Rainbow Sword", () => ConsoleAssets.spawnRainbowSword(), () => ConsoleAssets.destroyRainbowSword(), () => ConsoleAssets.UpdateRainbowSword()),
            new ModButtonInfo("Ban Hammer", () => ConsoleAssets.spawnBanHammer(), () => ConsoleAssets.destroyBanHammer(), () => ConsoleAssets.UpdateBanHammer()),
            new ModButtonInfo("Roblox Sword", () => ConsoleAssets.spawnRobloxSword(), () => ConsoleAssets.destroyRobloxSword(), () => ConsoleAssets.UpdateRobloxSword()),
            new ModButtonInfo("Video Player", () => ConsoleAssets.VideoPlayer(), () => ConsoleAssets.destroyVideoPlayer()),
            new ModButtonInfo("BoomBox", () => ConsoleAssets.Boombox(GorillaTagger.Instance.offlineVRRig), () => ConsoleAssets.destroyBoombox(GorillaTagger.Instance.offlineVRRig)),
            new ModButtonInfo("Iphone", () => ConsoleAssets.iPhoneTikTok(GorillaTagger.Instance.offlineVRRig), () => ConsoleAssets.destroyiPhoneTikTok(GorillaTagger.Instance.offlineVRRig)),
            new ModButtonInfo("Pistol", () => ConsoleAssets.spawnPistol(), () => ConsoleAssets.destroyPistol(), () => ConsoleAssets.UpdatePistol()),
            new ModButtonInfo("Super Crown", () => ConsoleAssets.supercrown(), () => ConsoleAssets.destroysupercrown()),
            new ModButtonInfo("Travis Scott", () => ConsoleAssets.TravisScottConcert(), () => ConsoleAssets.destroyTravisScottConcert()),
            new ModButtonInfo("Mini Travis", () => ConsoleAssets.spawnMiniTravis(), () => ConsoleAssets.destroyminiTravis()),
            new ModButtonInfo("Fake Menu", () => ConsoleAssets.spawnBaitMenu(), () => ConsoleAssets.destroyBaitMenu()),
            new ModButtonInfo("Cheezburger", () => ConsoleAssets.spawnCheezburger(), () => ConsoleAssets.destroyCheezburger()),
            new ModButtonInfo("Gorilla TV", () => ConsoleAssets.GorillaTv(), () => ConsoleAssets.DestroyGorillaTv()),
            new ModButtonInfo("Cherry Bomb", () => ConsoleAssets.CherryBomb(), () => ConsoleAssets.destroyCherryBomb(), () => ConsoleAssets.UpdateCherryBomb() ),
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