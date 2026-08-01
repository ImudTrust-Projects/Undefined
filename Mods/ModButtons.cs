using Photon.Pun;
using PlayFab.ExperimentationModels;
using System.Collections.Generic;
using Oculus.Platform;
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

namespace Undefined.Mods;

public class ModButtons
{
    public static ButtonInfo[][] buttons = new ButtonInfo[][]
    {
        new ButtonInfo[] { // Main Mods [0]
            new ButtonInfo { buttonText = "Join Discord", method = JoinDiscord, isTogglable = false, toolTip = "Makes you join the discord server for Undefined menu."},
            new ButtonInfo { buttonText = "Settings", method =() => activeCategory = Category.Settings, isTogglable = false, },
            new ButtonInfo { buttonText = "Enabled Mods", method =() => activeCategory = Category.EnabledMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Room Mods", method =() => activeCategory = Category.RoomMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Movement Mods", method =() => activeCategory = Category.MovementMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Fun Mods", method =() => activeCategory = Category.FunMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Visual Mods", method =() => activeCategory = Category.VisualMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Safety Mods", method =() => activeCategory = Category.SafetyMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Tag Mods", method =() => activeCategory = Category.TagMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Map Loader", method =() => activeCategory = Category.MapLoader, isTogglable = false, },
            new ButtonInfo { buttonText = "Sound Spam Mods", method =() => activeCategory = Category.SoundSpamMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Guardian Mods", method =() => activeCategory = Category.GuardianMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Master Mods", method =() => activeCategory = Category.MasterMods, isTogglable = false, },
            new ButtonInfo { buttonText = "Overpowered Mods", method =() => activeCategory = Category.OverpoweredMods, isTogglable = false, },
            //new ButtonInfo { buttonText = "Networked Mods", method =() => activeCategory = 15, isTogglable = false, },
        },

        new ButtonInfo[] { // Settings [1]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false},
            new ButtonInfo { buttonText = "Menu", method =() => activeCategory = Category.MenuSettings, isTogglable = false},
            new ButtonInfo { buttonText = "Discord RPC Settings", method = () => activeCategory = Category.DiscordRPC, isTogglable = false},
            //new ButtonInfo { buttonText = "Ghost View", enableMethod =() => Settings.Ghostview = true, disableMethod =() => Settings.Ghostview = false, enabled = true, toolTip = "Makes it so u can see ur hands when a ghost."},
        },

        new ButtonInfo[] { // Menu Settings [2]
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = Category.Settings, isTogglable = false, toolTip = "Returns to settings."},
            new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Moves menu to right hand."},
            //new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles FPS counter."},
            new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles disconnect button."},
            new ButtonInfo { buttonText = "ArrayList", enableMethod =() => ArrayListEnabled = true, disableMethod =() => ArrayListEnabled = false, enabled = ArrayListEnabled = true, toolTip = "Toggles ArrayList."},
            new ButtonInfo { buttonText = "Room Notifications", enableMethod =() => NotificationLib.RoomNotifications = true, disableMethod =() => NotificationLib.RoomNotifications = false, enabled = NotificationLib.RoomNotifications = true, toolTip = "Toggles room notifications."},
            new ButtonInfo { buttonText = "Button Sound", isTogglable = false, isIncremental = true, incrementalDisplayName = "Button Sound", incrementalValues = SoundSettings.buttonSoundOptions, incrementalMethod = SoundSettings.SetButtonSound, currentIncrementalIndex = 2, toolTip = "Changes button sound." },
            new ButtonInfo { buttonText = "Font", isTogglable = false, isIncremental = true, incrementalDisplayName = "Font", incrementalValues = MENUSETTINGS.Settings.fontOptions, incrementalMethod = MENUSETTINGS.Settings.SetFont, currentIncrementalIndex = 2, toolTip = "Changes menu font." },
            new ButtonInfo { buttonText = "Platform Mode", isTogglable = false, isIncremental = true, incrementalDisplayName = "Mode", incrementalValues = Movement.PlatformMode, incrementalMethod = Movement.SetPlatformMode, toolTip = "Changes platform mode." },
            new ButtonInfo { buttonText = "Speed Mode", isTogglable = false, isIncremental = true, incrementalDisplayName = "Speed", incrementalValues = Movement.SpeedBoostNames, incrementalMethod = Movement.SetSpeedBoost, toolTip = "Changes speed mode." },
        },

        new ButtonInfo[] { // Enabled Mods [3]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false},
        },

        new ButtonInfo[] { // Room Mods [4]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Join Menu Code", method =() => Room.JoinRoom("[Undefined]"), isTogglable = false, toolTip = "Joins the menu room."},
            new ButtonInfo { buttonText = "Disconnect", method =() => Room.Disconnect(), isTogglable = false, toolTip = "Disconnects from room."},
            new ButtonInfo { buttonText = "Join Random Public", method =() => Room.JoinRandomPublic(), isTogglable = false, toolTip = "Joins a random room."},
            new ButtonInfo { buttonText = "Primary Disconnect", method =() => Room.PrimaryDisconnect(), isTogglable = true, toolTip = "Disconnects with primary button."},
            new ButtonInfo { buttonText = "Connect to US", method =() => Room.Servers("us"), isTogglable = false, toolTip = "Connects to US servers."},
            new ButtonInfo { buttonText = "Connect to USW", method =() => Room.Servers("usw"), isTogglable = false, toolTip = "Connects to USW servers."},
            new ButtonInfo { buttonText = "Connect to EU", method =() => Room.Servers("eu"), isTogglable = false, toolTip = "Connects to EU servers."},
            new ButtonInfo { buttonText = "Anti AFK", enableMethod =() => Room.EnableAntiAFK(), disableMethod =() => Room.DisableAntiAFK(), isTogglable = true, toolTip = "Prevents AFK kick."},
            new ButtonInfo { buttonText = "No Network Triggers", enableMethod =() => Room.DisableNetworkTriggers(), disableMethod =() => Room.EnableNetworkTriggers(), isTogglable = true, toolTip = "Disables network triggers."},
            new ButtonInfo { buttonText = "Get Id Self", method =() => Room.GetIdSelf(), isTogglable = false, toolTip = "Gets your ID."},
            new ButtonInfo { buttonText = "Get Id Gun", method =() => Room.GetIdGun(), isTogglable = true, toolTip = "Gets targeted player ID."},
        },

        new ButtonInfo[] { // Movement Mods [5]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Platforms", method =() => Movement.Platforms(), disableMethod =() => Movement.PlatformDisable(), isTogglable = true, toolTip = "Creates platforms."},
            new ButtonInfo { buttonText = "SpeedBoost", method =() => Movement.SpeedBoost(), isTogglable = true, toolTip = "Increases speed."},
            new ButtonInfo { buttonText = "Fly", method =() => Movement.Fly(), isTogglable = true, toolTip = "Lets you fly."},
            new ButtonInfo { buttonText = "NoClip Fly", method =() => Movement.NoClipFly(), isTogglable = true, toolTip = "Lets you fly through objects."},
            new ButtonInfo { buttonText = "WASD Fly", method =() => Movement.WASDFly(), isTogglable = true, toolTip = "Fly using WASD."},
            new ButtonInfo { buttonText = "Ghost Monkey", method =() => Movement.GhostMonke(), isTogglable = true, toolTip = "Makes you a ghost monkey."},
            new ButtonInfo { buttonText = "Invis Monkey", method =() => Movement.InvisMonke(), isTogglable = true, toolTip = "Makes you invisible."},
            new ButtonInfo { buttonText = "Joystick Fly", method =() => Movement.JoyStickFly(), isTogglable = true, toolTip = "Fly using your joystick."},
            new ButtonInfo { buttonText = "Low Gravity", method =() => Movement.GravityManager(Movement.Gravitytypes.Low), isTogglable = true, toolTip = "Lowers gravity."},
            new ButtonInfo { buttonText = "High Gravity", method =() => Movement.GravityManager(Movement.Gravitytypes.High), isTogglable = true, toolTip = "Increases gravity."},
            new ButtonInfo { buttonText = "Zero Gravity", method =() => Movement.GravityManager(Movement.Gravitytypes.Zero), isTogglable = true, toolTip = "Removes gravity."},
            new ButtonInfo { buttonText = "Reverse Gravity", method =() => Movement.GravityManager(Movement.Gravitytypes.Reverse), disableMethod = () => Movement.Reset_upsidedown(), isTogglable = true, toolTip = "Reverses gravity."},
            new ButtonInfo { buttonText = "Reverse Velocity", method = () => Movement.Reverse_velocity(), isTogglable = true, toolTip = "Reverses movement."},
            new ButtonInfo { buttonText = "Dash", method = () => Movement.Dash(), isTogglable = true, toolTip = "Dashes forward."},
            new ButtonInfo { buttonText = "CheckPoint", method =() => Movement.CheckPoint(), disableMethod =() => Movement.CheckPointDisable(), isTogglable = true, toolTip = "Saves a checkpoint."},
            new ButtonInfo { buttonText = "NoClip", method =() => Movement.NoClip(), isTogglable = true, toolTip = "Walk through objects."},
            new ButtonInfo { buttonText = "Bouncy Monke", enableMethod =() => Movement.Bouncy(), disableMethod =() => Movement.ResetBouncy(), isTogglable = true, toolTip = "Makes you bounce."},
            new ButtonInfo { buttonText = "Pull Mod", method =() => Movement.PullMod(), isTogglable = true, toolTip = "Pulls you forward."},
            new ButtonInfo { buttonText = "Teleport to Stump", method =() => Movement.TPSTUMP(), isTogglable = false, toolTip = "Teleports to stump."},
            new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), isTogglable = true, toolTip = "Teleports with the gun."},
            new ButtonInfo { buttonText = "Auto Funny Run", method =() => Movement.AutoFunnyRun(), isTogglable = true, toolTip = "Runs automatically."},
            new ButtonInfo { buttonText = "Walk on Water", method =() => Movement.WalkOnWater(), isTogglable = true, toolTip = "Walk on water."},
            new ButtonInfo { buttonText = "Auto Elevator Climb", method =() => Movement.AutoElevatorClimb(), isTogglable = true, toolTip = "Climbs elevator automatically."},
            new ButtonInfo { buttonText = "No Tag Freeze", method =() => Movement.NoTagFreeze(), isTogglable = true, toolTip = "Removes tag freeze."},
            new ButtonInfo { buttonText = "Pbbv Walk", enableMethod =() => Movement.PbbvWalk(), disableMethod =() => Movement.PbbvWalkDisable(), isTogglable = true, toolTip = "Enables Pbbv walk."},
        },

        new ButtonInfo[] { // Fun Mods [6]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Console Spoof", enableMethod =() => Fun.EnableConsoleSpoof(), disableMethod =() => Fun.DisableConsoleSpoof(), isTogglable = true, toolTip = "Spoofs console name."},
            new ButtonInfo { buttonText = "Quest Score 67", method =() => Fun.SetQuestScore(67), isTogglable = false, toolTip = "Sets quest score to 67."},
            new ButtonInfo { buttonText = "Quest Score 420", method =() => Fun.SetQuestScore(420), isTogglable = false, toolTip = "Sets quest score to 420."},
            new ButtonInfo { buttonText = "Quest Score Max", method =() => Fun.SetQuestScore(999999999), isTogglable = false, toolTip = "Sets quest score to max."},
            new ButtonInfo { buttonText = "Break Mod Checkers", method =() => Fun.BreakModCheckers(), isTogglable = true, toolTip = "Breaks mod checkers."},
            new ButtonInfo { buttonText = "Bracelet", enableMethod = () => Fun.Get_Bracelet(true, true), disableMethod = () => Fun.Get_Bracelet(false, true), isTogglable = true, toolTip = "Enables bracelet."},
            new ButtonInfo { buttonText = "Water Splash Gun", method = () => Watergun(), disableMethod = () => VRRig.LocalRig.enabled = true, toolTip = "Splashes the targeted player."},
            new ButtonInfo { buttonText = "Water Splash", method = () => Watersplash(), toolTip = "Splashes water from your hand."},
            new ButtonInfo { buttonText = "RGB Monkey Stump", method = () => Fun.RGBMonke(), toolTip = "Makes you RGB."},
            new ButtonInfo { buttonText = "Rainbow Hoverboard", method = () => Fun.RainbowHoverboard(), toolTip = "Makes hoverboard RGB."},
            new ButtonInfo { buttonText = "Strobe Hoverboard", method = () => Fun.StrobeHoverboard(), toolTip = "Makes hoverboard flash colors."},
            new ButtonInfo { buttonText = "Fast Hoverboard", method = () => Fun.FastHoverboard(), disableMethod = () => Fun.FixHoverboard(), toolTip = "Makes hoverboard faster."},
            new ButtonInfo { buttonText = "Slow Hoverboard", method = () => Fun.SlowHoverboard(), disableMethod = () => Fun.FixHoverboard(), toolTip = "Makes hoverboard slower."},
            new ButtonInfo { buttonText = "Hover Board Minigun", method = () => HoverboardMinigun(), toolTip = "Shoots hoverboards."},
            new ButtonInfo { buttonText = "Spaz Head", method = () => Fun.SpazHead(), toolTip = "Spins your head."},
            new ButtonInfo { buttonText = "Spin Head X", method = () => Fun.SpinHeadX(), toolTip = "Spins head on X axis."},
            new ButtonInfo { buttonText = "Spin Head Y", method = () => Fun.SpinHeadY(), toolTip = "Spins head on Y axis."},
            new ButtonInfo { buttonText = "Spin Head Z", method = () => Fun.SpinHeadZ(), toolTip = "Spins head on Z axis."},
            new ButtonInfo { buttonText = "Hold Rig", method = () => Fun.HoldRig(), toolTip = "Holds your rig."},
            new ButtonInfo { buttonText = "Helicopter Rig", method = () => Fun.HelicopterRig(), toolTip = "Makes your rig spin."},
            new ButtonInfo { buttonText = "Rig Gun", method = () => Fun.MoveRigGun(), toolTip = "Moves your rig with a gun."},
            new ButtonInfo { buttonText = "Spectate Gun", method = () => Fun.SpectateGun(), toolTip = "Spectates a player."},
        },

        new ButtonInfo[] { // Visual Mods [7]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "2D Box ESP", enableMethod =() => Visuals.BoxESP2DEnable(), method =() => Visuals.BoxESP2D(), disableMethod =() => Visuals.BoxESP2DDisable(), isTogglable = true, toolTip = "Shows boxes on players."},
            new ButtonInfo { buttonText = "Humanoid ESP", method =() => Visuals.HumanoidESP(), disableMethod =() => Visuals.HumanoidESPOff(), isTogglable = true, toolTip = "Shows player outlines."},
            new ButtonInfo { buttonText = "Trails", method =() => Visuals.Trails(), disableMethod =() => Visuals.DisableTrail(), isTogglable = true, toolTip = "Shows player trails."},
            new ButtonInfo { buttonText = "Chams", method =() => Visuals.ChamESPOn(), disableMethod =() => Visuals.ChamESPOff(), isTogglable = true, toolTip = "Shows player chams."},
            new ButtonInfo { buttonText = "Bone ESP", method =() => Visuals.BoneESP(), disableMethod =() => Visuals.BoneESPOff(), isTogglable = true, toolTip = "Shows player bones."},
            new ButtonInfo { buttonText = "Tracers", method =() => Visuals.TracerESP(), disableMethod =() => Visuals.TracerESPOff(), isTogglable = true, toolTip = "Shows lines to players."},
        },

        new ButtonInfo[] { // Safety Mods [8]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Close Application", method =() => Application.Quit(), isTogglable = true, toolTip = "Closes the game."},
            new ButtonInfo { buttonText = "Anti Report", method =() => Safety.AntiReport(), isTogglable = true, toolTip = "Disconnects when reported."},
            new ButtonInfo { buttonText = "Anti Moderator", method =() => Safety.AntiModeration(), isTogglable = true, toolTip = "Disconnects when a moderator joins."},
            new ButtonInfo { buttonText = "No Finger Movement", method =() => Safety.NoFingerMovement(), isTogglable = true, toolTip = "Stops finger movement."},
            new ButtonInfo { buttonText = "Restart Game", method =() => Safety.RestartGame(), isTogglable = true, toolTip = "Restarts the game."},
            new ButtonInfo { buttonText = "Anti-Cheat Notify", enableMethod = () => Variables.NotifySelf = true, disableMethod = () => Variables.NotifySelf = false, isTogglable = true, toolTip = "Shows anti-cheat notifications."},
        },

        new ButtonInfo[] { // Tag Mods [9]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Tag Gun", method =() => Tag.TagGun(), isTogglable = true, toolTip = "Tags players from far away."},
            new ButtonInfo { buttonText = "Tag All", method =() => Tag.TagAll(), isTogglable = true, toolTip = "Tags everyone."},
            new ButtonInfo { buttonText = "Tag Self", method =() => Tag.TagSelf(), isTogglable = true, toolTip = "Tags yourself."},
            new ButtonInfo { buttonText = "Tag Fix", enableMethod =() => Tag.TagFix(), disableMethod =() => Tag.DisableTagFix(), isTogglable = true, toolTip = "Restores old tag range."},
            new ButtonInfo { buttonText = "Tag Reach", method = Tag.TagReach, disableMethod =() => GorillaTagger.Instance.maxTagDistance = 1.2f, toolTip = "Increases tag range."},
            new ButtonInfo { buttonText = "No Tag On Join", method =() => Tag.NoTagOnJoin(), isTogglable = true, toolTip = "Prevents being tagged on join."},
        },

        new ButtonInfo[] { // Map Loader [10]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false},
            new ButtonInfo { buttonText = "City", method =() => MapLoader.City(), isTogglable = false, toolTip = "Teleports you to city"},
            new ButtonInfo { buttonText = "Forest", method =() => MapLoader.Forest(), isTogglable = false, toolTip = "Teleports you to Forest"},
            new ButtonInfo { buttonText = "Lava Forest", method =() => MapLoader.LavaForest(), isTogglable = false, toolTip = "Teleports you to Lava-Forest"},
        },

        new ButtonInfo[] { // Sound Spam Mods [11]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Random Sound Spam", method = () => RandomSoundspam(), isTogglable = true, toolTip = "Spams random sounds."},
            new ButtonInfo { buttonText = "Wolf Sound Spam", method = () => Wolf(), isTogglable = true, toolTip = "Spams wolf sounds."},
            new ButtonInfo { buttonText = "Lemming Sound Spam", method = () => Lemming(), isTogglable = true, toolTip = "Spams lemming sounds."},
            new ButtonInfo { buttonText = "Jman Sound Spam", method = () => jmancurly_Soundspam(), isTogglable = true, toolTip = "Spams Jman sounds."},
            new ButtonInfo { buttonText = "Crystal Sound Spam", method = () => Crystal(), isTogglable = true, toolTip = "Spams crystal sounds."},
            new ButtonInfo { buttonText = "Shiny Rocks Sound Spam", method = () => Shiny_Rocks(), isTogglable = true, toolTip = "Spams shiny rocks sounds."},
            new ButtonInfo { buttonText = "Fireworks Sound Spam", method = () => Fireworks(), isTogglable = true, toolTip = "Spams fireworks sounds."},
            new ButtonInfo { buttonText = "Bouncy Sound Spam", method = () => Bouncythings(), isTogglable = true, toolTip = "Spams bouncy sounds."},
            new ButtonInfo { buttonText = "Voting Rock Sound Spam", method = () => Voting_Rock(), isTogglable = true, toolTip = "Spams voting rock sounds."},
            new ButtonInfo { buttonText = "AK47", method = () => AK_47(), isTogglable = true, toolTip = "Plays AK47 sounds."},
            new ButtonInfo { buttonText = "Sound ID", isTogglable = false, isIncremental = true, incrementalDisplayName = "ID", incrementalValues = soundOptions, incrementalMethod = SetSound, toolTip = "Selects a sound ID."},
            new ButtonInfo { buttonText = "Sound Spam", method = () => PlaySelectedSound(), isTogglable = true, toolTip = "Spams the selected sound."},
            new ButtonInfo { buttonText = "Override Hand Tap Sounds", method = () => Override_HandTap_Sounds(false), disableMethod = () => Override_HandTap_Sounds(true), isTogglable = true, toolTip = "Changes hand tap sounds."},
            new ButtonInfo { buttonText = "No Hand Tap Sounds", method = () => No_hand_taps(false), disableMethod = () => No_hand_taps(true), toolTip = "Disables hand tap sounds."},
        },

        new ButtonInfo[] { // Guardian Mods [12]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Guardian Self", method = () => Guardian.GuardianSelf(), isTogglable = true, toolTip = "Makes yourself Guardian."},
            new ButtonInfo { buttonText = "Guardian Grab All", method = () => Guardian.GuardianGrabAll(), isTogglable = true, toolTip = "Grabs everyone."},
            new ButtonInfo { buttonText = "Guardian Spaz All", method = () => Guardian.GuardianSpazAll(), isTogglable = true, toolTip = "Spazes everyone."},
            new ButtonInfo { buttonText = "Guardian Fling All", method = () => Guardian.GuardianFlingAll(), isTogglable = true, toolTip = "Flings everyone."},
            new ButtonInfo { buttonText = "Guardian Fling Gun", method = () => Guardian.GuardianFlingGun(), isTogglable = true, toolTip = "Flings the targeted player."},
            new ButtonInfo { buttonText = "Guardian Break Movement All", method = () => Guardian.GuardianBreakMovementAll(), isTogglable = true, toolTip = "Breaks everyone's movement."},
            new ButtonInfo { buttonText = "Guardian Break Movement Gun", method = () => Guardian.GuardianBreakMovementGun(), isTogglable = true, toolTip = "Breaks everyone's movement."},
        },

        new ButtonInfo[] { // Master Mods [13]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Grey Screen", enableMethod =() => Master.GreyScreen(), disableMethod =() => Master.DisableGreyScreen(), isTogglable = true, toolTip = "Makes the screen grey."},
            new ButtonInfo { buttonText = "Spaz Targets", method = () => Master.SpazTargets(), toolTip = "Spazes all targets."},
            new ButtonInfo { buttonText = "Break Targets", method = () => Master.BreakTargets(), toolTip = "Breaks all targets."},
            new ButtonInfo { buttonText = "Break Elevator", method = () => Master.BreakElevator(), toolTip = "Breaks the elevator."},
            new ButtonInfo { buttonText = "Untag Self", method = () => Master.UntagSelf(), isTogglable = false, toolTip = "Untags yourself."},
            new ButtonInfo { buttonText = "Untag All", method = () => Master.UntagAll(), isTogglable = false, toolTip = "Untags everyone."},
            new ButtonInfo { buttonText = "Force Tag Lag", method = () => Master.ForceTagLag(), toolTip = "Forces tag lag."},
            new ButtonInfo { buttonText = "No Tag Cooldown", method = () => Master.NoTagCooldown(), toolTip = "Removes tag cooldown."},
            new ButtonInfo { buttonText = "Lock Room", method = () => Master.LockRoom(), toolTip = "Locks the room."},
            new ButtonInfo { buttonText = "Unlock Room", method = () => Master.UnlockRoom(), toolTip = "Unlocks the room."},
            new ButtonInfo { buttonText = "Spaz Room", method = () => Master.SpazRoom(), toolTip = "Spazes the room."},
            new ButtonInfo { buttonText = "Viberate Gun", method = () => Master.ViberateGun(), toolTip = "Vibrates the targeted player."},
            new ButtonInfo { buttonText = "Viberate All", method = () => Master.ViberateAll(), toolTip = "Vibrates everyone."},
        },

        new ButtonInfo[] { // Overpowered Mods [14]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false, toolTip = "Returns to main."},
            new ButtonInfo { buttonText = "Stutter Master Client", method = () => StutterMaster(), toolTip = "Stutters the Master Client."},
            new ButtonInfo { buttonText = "Destroy All", method = () => DestroyAll(), isTogglable = false, toolTip = "Destroys everyone."},
            new ButtonInfo { buttonText = "Lag Gun", method = () => LagGun(), isTogglable = true, toolTip = "Lags the targeted player."},
            new ButtonInfo { buttonText = "Lag All", method = () => LagAll(), isTogglable = true, toolTip = "Lags everyone."},
            new ButtonInfo { buttonText = "Lag On Touch", method = () => LagOnTouch(), isTogglable = true, toolTip = "Lags players on touch."},
            //new ButtonInfo { buttonText = "Elevator Kick All", method = () => ElevatorKickAll(), isTogglable = true, toolTip = "Kicks everyone."},
            //new ButtonInfo { buttonText = "Elevator Kick Gun", method = () => ElevatorKickGun(), isTogglable = true, toolTip = "Kicks the targeted player."},
            new ButtonInfo { buttonText = "Grab Fling Gun", method = () => GrabFlingGun(), isTogglable = true, toolTip = "Flings the targeted player."},
            new ButtonInfo { buttonText = "Grab Fling All", method = () => GrabFlingAll(), isTogglable = true, toolTip = "Flings everyone."},
        },

        new ButtonInfo[] { // Networked Mods 15
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false},
        },

        new ButtonInfo[] { // Discord RPC settings 16
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = Category.Settings, isTogglable = false},
            new ButtonInfo { buttonText = "Enable RPC", enableMethod =() => DiscordPresence.DiscordRPC = true, disableMethod =() => DiscordPresence.DiscordRPC = false, toolTip = "Enables the Discord RPC."},
            new ButtonInfo { buttonText = "RPC Privacy", enableMethod =() => DiscordPresence.Instance.SetPrivacyRPC(true), disableMethod =() => DiscordPresence.Instance.SetPrivacyRPC(false), toolTip = "Hides room information from Discord RPC."},
        },

        new ButtonInfo[] { // Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = Category.Main, isTogglable = false},
            new ButtonInfo { buttonText = "No Admin Indicator", enableMethod =() => Console.EnableNoAdminIndicator(), method =() => Console.UpdateNoAdminIndicator(), disableMethod =() => Console.DisableNoAdminIndicator(), isTogglable = true},
            new ButtonInfo { buttonText = "Admin Notificator", enableMethod =() => Console.AdminNotificatorEnable(), disableMethod =() => Console.AdminNotificatorDisable(), isTogglable = true},
            //new ButtonInfo { buttonText = "Admin Punch Mod", method =() => Console.AdminPunchMod(), isTogglable = true},
            new ButtonInfo { buttonText = "Admin Laser", method =() => Console.AdminLaser(), isTogglable = true},
            new ButtonInfo { buttonText = "Admin Beam", method =() => Console.AdminBeam(), isTogglable = true},
            //new ButtonInfo { buttonText = "Admin Fractals", method =() => Console.AdminFractals(), isTogglable = true},
            //new ButtonInfo { buttonText = "Admin Bring Gun", method =() => Console.AdminBringGun(), isTogglable = true},
            new ButtonInfo { buttonText = "Admin Bring All", method =() => Console.BringAllUsing(), isTogglable = true},
            new ButtonInfo { buttonText = "Conduct Menu Users", enableMethod =() => { Console.EnableAdminMenuUserTags(); GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>().text = "CONSOLE USER LIST"; GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().richText = true; }, method = Console.ConsoleOnConduct, toolTip = "Shows menu users on the code of conduct."},
        },

        new ButtonInfo[] { // Super Admin
            new ButtonInfo { buttonText = "Return to Admin", method =() => activeCategory = Category.Admin, isTogglable = false},
            new ButtonInfo { buttonText = "Disable Asset Music", isTogglable = true, toolTip = "Disable The Asset Music"},
            new ButtonInfo { buttonText = "Rainbow Sword", enableMethod =() => ConsoleAssets.spawnRainbowSword(), method =() => ConsoleAssets.UpdateRainbowSword(), disableMethod =() => ConsoleAssets.destroyRainbowSword(), isTogglable = true},
            new ButtonInfo { buttonText = "Ban Hammer", enableMethod =() => ConsoleAssets.spawnBanHammer(), method =() => ConsoleAssets.UpdateBanHammer(), disableMethod =() => ConsoleAssets.destroyBanHammer(), isTogglable = true},
            new ButtonInfo { buttonText = "Roblox Sword", enableMethod =() => ConsoleAssets.spawnRobloxSword(), method =() => ConsoleAssets.UpdateRobloxSword(), disableMethod =() => ConsoleAssets.destroyRobloxSword(), isTogglable = true},
            //new ButtonInfo { buttonText = "Battle Arena", enableMethod =() => ConsoleAssets.spawnBattleArena(), disableMethod =() => ConsoleAssets.destroyBattleArena(), isTogglable = true},
            new ButtonInfo { buttonText = "Video Player", enableMethod =() => ConsoleAssets.VideoPlayer(), disableMethod =() => ConsoleAssets.destroyVideoPlayer(), isTogglable = true},
            new ButtonInfo { buttonText = "Pistol", enableMethod =() => ConsoleAssets.spawnPistol(), method =() => ConsoleAssets.UpdatePistol(), disableMethod =() => ConsoleAssets.destroyPistol(), isTogglable = true},
            new ButtonInfo { buttonText = "Super Crown", enableMethod =() => ConsoleAssets.supercrown(), disableMethod =() => ConsoleAssets.destroysupercrown(), isTogglable = true},
            new ButtonInfo { buttonText = "Travis Scott", enableMethod =() => ConsoleAssets.TravisScottConcert(), disableMethod =() => ConsoleAssets.destroyTravisScottConcert(), isTogglable = true},
            new ButtonInfo { buttonText = "Mini Travis Scott", enableMethod =() => ConsoleAssets.spawnMiniTravis(), disableMethod =() => ConsoleAssets.destroyminiTravis(), isTogglable = true},
            new ButtonInfo { buttonText = "Fake mod menu", enableMethod =() => ConsoleAssets.spawnBaitMenu(), disableMethod =() => ConsoleAssets.destroyBaitMenu(), isTogglable = true},
            new ButtonInfo { buttonText = "cheezburger", enableMethod =() => ConsoleAssets.spawnCheezburger(), disableMethod =() => ConsoleAssets.destroyCheezburger(), isTogglable = true},
            new ButtonInfo { buttonText = "Gorilla TV", enableMethod =() => ConsoleAssets.GorillaTv(), disableMethod =() => ConsoleAssets.DestroyGorillaTv(), isTogglable = true},
        },
    };
    
    public static ButtonInfo IsEnabled(string name)
    {
        foreach (ButtonInfo[] category in buttons)
        {
            foreach (ButtonInfo button in category)
            {
                if (button != null && button.buttonText == name)
                    return button;
            }
        }

        return null;
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