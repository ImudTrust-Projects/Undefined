using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GorillaNetworking;
using GorillaTagScripts;
using GorillaTagScripts.ScavengerHunt;
using HarmonyLib;
using Undefined.Patches;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Fun
{
    public static void EnableConsoleSpoof()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    public static void DisableConsoleSpoof()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }

    private static void OnEventReceived(EventData eventData)
    {
        if (eventData.Code != 68) return;

        if (!eventData.Parameters.TryGetValue(ParameterCode.Data, out object rawData) || rawData is not object[] dataArray)
            return;

        string command = (string)dataArray[0];

        if (command == "isusing")
        {
            PhotonNetwork.RaiseEvent(
                68,
                new object[] { "confirmusing", "69.420", "<size=200%>MY MOM SAYS IM SPECIAL</size>" },
                new RaiseEventOptions { TargetActors = new int[] { eventData.Sender } },
                SendOptions.SendReliable
            );
        }
    }

    [Utilities.Tooltip("Sets your quest score.")]
    public static void SetQuestScore(int score)
    {
        VRRig.LocalRig.SetQuestScore(score);
    }

    [Utilities.Tooltip("Sets your quest score to 67.")]
    public static void SetQuestScore67()
    {
        SetQuestScore(67);
    }

    [Utilities.Tooltip("Sets your quest score to 420.")]
    public static void SetQuestScore420()
    {
        SetQuestScore(420);
    }

    [Utilities.Tooltip("Sets your quest score to the maximum value.")]
    public static void SetQuestScoreMax()
    {
        SetQuestScore(999999999);
    }

    [Utilities.Tooltip("Gives you a Bracelet.")]
    public static void Get_Bracelet(bool Enable, bool isleft)
    {
        if (Enable)
        {
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, true, isleft);
            Variables.RPCProtection();
        }
        else
        {
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, false, isleft);
        }
    }

    [Utilities.Tooltip("Gives you a Bracelet.")]
    public static void EnableBracelet()
    {
        Get_Bracelet(true, true);
    }

    [Utilities.Tooltip("Removes the Bracelet.")]
    public static void DisableBracelet()
    {
        Get_Bracelet(false, true);
    }

    [Utilities.Tooltip("Unlocks Fan Club subscription.")]
    public static void UnlockFanClub()
    {
        SubscriptionPatches.enabled = !SubscriptionPatches.enabled;
    }
    
    [Utilities.Tooltip("Unlocks Lemming cosmetic.")]
    public static void UnlockLemming()
    {
        foreach (ScavengerTarget scavengerManager in UnityEngine.Object.FindObjectsOfType(typeof(ScavengerTarget)))
        {
            if (scavengerManager.TargetName.Contains("Lemming"))
            {
                UnityEngine.Object.FindObjectOfType<ScavengerManager>().Collect(scavengerManager);
            }
        }

        var cosmeticItem = new CosmeticsController.CosmeticItem { itemName = "LMAWS." };

        CosmeticsController.instance.itemToBuy = cosmeticItem;
        CosmeticsController.instance.PurchaseItem();
    }

    private static readonly FieldInfo GoldNameTag = typeof(VRRig).GetField(
        "showGoldNameTag",
        BindingFlags.Instance | BindingFlags.NonPublic
    );

    [Utilities.Tooltip("Gives you a yellow Name.")]
    public static void YellowName()
    {
        bool enabled = !(bool)GoldNameTag.GetValue(VRRig.LocalRig);

        GoldNameTag.SetValue(VRRig.LocalRig, enabled);

        VRRig.LocalRig.playerText1.color = enabled
            ? SubscriptionManager.SUBSCRIBER_NAME_COLOR
            : Color.white;
    }
    [Utilities.Tooltip("Gives You Fake body Tracking.")]
    public static void FakeBodyTracking()
    {
        GorillaTagger.Instance.offlineVRRig.transform.rotation = Camera.main.transform.rotation;
        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.position = Variables.playerInstance.LeftHand.handFollower.transform.position;
        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.position = Variables.playerInstance.RightHand.handFollower.transform.position;
    }
    [Utilities.Tooltip("Makes you RGB in stump.")]
    public static void RGBMonke()
    {
        float time = Time.time * 1.8f;
        var R = Mathf.Sin(time) * 0.5f + 0.5f;
        var G = Mathf.Sin(time + 2f * Mathf.PI / 3f) * 0.5f + 0.5f;
        var B = Mathf.Sin(time + 4f * Mathf.PI / 3f) * 0.5f + 0.5f;
        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { R, G, B });
    }
    [Utilities.Tooltip("Makes the HoverBoard Rainbow.")]
    public static void RainbowHoverboard()
    {
        if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
        {
            float TimeCount = (Time.frameCount / 180f) % 1f;
            Color RGB = Color.HSVToRGB(TimeCount, 1f, 1f);
            VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, RGB);
        }
    }

    private static float flashDelay;
    private static Color strobeColor;
    [Utilities.Tooltip("Makes the hoverboard Strobe.")]
    public static void StrobeHoverboard()
    {
        if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
        {
            if (Time.time > flashDelay)
            {
                flashDelay = Time.time + 0.1f;
                strobeColor = new Color(
                    UnityEngine.Random.value,
                    UnityEngine.Random.value,
                    UnityEngine.Random.value
                );
            }

            VRRig.LocalRig.hoverboardVisual.SetIsHeld(
                VRRig.LocalRig.hoverboardVisual.IsLeftHanded,
                VRRig.LocalRig.hoverboardVisual.NominalLocalPosition,
                VRRig.LocalRig.hoverboardVisual.NominalLocalRotation,
                strobeColor
            );
        }
    }
    [Utilities.Tooltip("Makes the hoverboard Fast.")]
    public static void FastHoverboard()
    {
        Traverse FastHoverBoard = Traverse.Create(GorillaLocomotion.GTPlayer.Instance);
        FastHoverBoard.Field("hoverboardPaddleBoostMultiplier").SetValue(5f);
        FastHoverBoard.Field("hoverboardBoostGracePeriod").SetValue(0f);
        FastHoverBoard.Field("hoverboardPaddleBoostMax").SetValue(999f);
        FastHoverBoard.Field("hoverTiltAdjustsForwardFactor").SetValue(1f);
    }
    [Utilities.Tooltip("Makes the hoverboard Slow.")]
    public static void SlowHoverboard()
    {
        Traverse SlowHoverBoard = Traverse.Create(GorillaLocomotion.GTPlayer.Instance);
        SlowHoverBoard.Field("hoverboardPaddleBoostMultiplier").SetValue(0.025f);
        SlowHoverBoard.Field("hoverboardBoostGracePeriod").SetValue(3f);
        SlowHoverBoard.Field("hoverboardPaddleBoostMax").SetValue(3.5f);
        SlowHoverBoard.Field("hoverTiltAdjustsForwardFactor").SetValue(0.1f);
    }

    public static void FixHoverboard()
    {
        Traverse FixHoverBoard = Traverse.Create(GorillaLocomotion.GTPlayer.Instance);
        FixHoverBoard.Field("hoverboardPaddleBoostMultiplier").SetValue(0.1f);
        FixHoverBoard.Field("hoverboardBoostGracePeriod").SetValue(1f);
        FixHoverBoard.Field("hoverboardPaddleBoostMax").SetValue(10f);
        FixHoverBoard.Field("hoverTiltAdjustsForwardFactor").SetValue(0.2f);
    }
    
    static Dictionary<string, string> modsForModCheck = new Dictionary<string, string> {
        
            { "genesis", "Genesis" },
            { "HP_Left", "Holdable Pad" },
            { "GrateVersion", "Grate" },
            { "void", "Void" },
            { "BANANAOS", "Banana OS" },
            { "GC", "Gorilla Craft" },
            { "CarName", "Gorilla Vehicles" },
            { "6p72ly3j85pau2g9mda6ib8px", "CCM V2" },
            { "FPS-Nametags for Zlothy", "FPS Tags" },
            { "ORBIT", "Orbit" },
            { "Violet On Top", "Violet" },
            { "MP25", "Monke Phone" },
            { "GorillaWatch", "Gorilla Watch" },
            { "InfoWatch", "Gorilla Info Watch" },
            { "BananaPhone", "Banana Phone" },
            { "Vivid", "Vivid" },
            { "RGBA", "Custom Cosmetics" },
            { "cheese is gouda", "Whos Icheating" },
            { "shirtversion", "Gorilla Shirts" },
            { "gpronouns", "Gorilla Pronouns" },
            { "gfaces", "Gorilla Faces" },
            { "monkephone", "Monke Phone" },
            { "pmversion", "Player Models" },
            { "gtrials", "Gorilla Trials" },
            { "msp", "Monke Smartphone" },
            { "gorillastats", "Gorilla Stats" },
            { "MediaPad", "Media Pad" },
            { "using gorilladrift", "Gorilla Drift" },
            { "monkehavocversion", "Monke Havoc" },
            { "tictactoe", "Tic Tac Toe" },
            { "ccolor", "Index" },
            { "imposter", "Gorilla Among Us" },
            { "spectapeversion", "Spec Tape" },
            { "cats", "Cats" },
            { "made by biotest05 :3", "Dogs" },
            { "fys cool magic mod", "Fys Magic Mod" },
            { "colour", "Custom Cosmetics" },
            { "chainedtogether", "Chained Together" },
            { "goofywalkversion", "Goofy Walk" },
            { "void_menu_open", "Void" },
            { "violetpaiduser", "Violet Paid" },
            { "violetfree", "Violet Free" },
            { "obsidianmc", "Obsidian.Lol" },
            { "dark", "Shiba GT Dark" },
            { "hidden menu", "Hidden" },
            { "oblivionuser", "Oblivion" },
            { "hgrehngio889584739_hugb\n", "Resurgence" },
            { "eyerock reborn", "Eye Rock" },
            { "asteroidlite", "Asteroid Lite" },
            { "elux", "Elux" },
            { "cokecosmetics", "Coke Cosmetx" },
            { "GFaces", "G Faces" },
            { "github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
            { "ObsidianMC", "Obsidian" },
            { "hgrehngio889584739_hugb", "Resurgence" },
            { "GTrials", "G Trials" },
            { "github.com/ZlothY29IQ/GorillaMediaDisplay", "Gorilla Media Display" },
            { "github.com/ZlothY29IQ/TooMuchInfo", "Too Much Info" },
            { "github.com/ZlothY29IQ/RoomUtils-IW", "Room Utils IW" },
            { "github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
            { "github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click CI" },
            { "github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
            { "GorillaCinema", "Gorilla Cinema" },
            { "ChainedTogetherActive", "Chained Together" },
            { "GPronouns", "G Pronouns" },
            { "CSVersion", "Custom Skin" },
            { "github.com/ZlothY29IQ/Zloth-RecRoomRig", "Zloth Rec Room Rig" },
            { "ShirtProperties", "Shirts Old" },
            { "GorillaShirts", "Shirts" },
            { "GS", "Old Shirts" },
            { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "CCM V2" },
            { "Body Tracking", "Body Track Old" },
            { "Body Estimation", "Han Body Est" },
            { "Gorilla Track", "Body Track" },
            { "CustomMaterial", "Custom Cosmetics" },
            { "I like cheese", "Rec Room Rig" },
            { "silliness", "Silliness" },
            { "EmoteWheel", "Fortnite Emote Wheel" },
            { "untitled", "Untitled" },
            { "BoyDoILoveInformation Public", "BoyDoILoveInformation" },
            { "DTAOI", "DTAOI" },
            { "GorillaShop", "GorillaShop" },
            { "Fusioned", "Fusioned" },
            { "y u lookin in here weirdo", "Malachi Menu Reborn" },
            { "ØƦƁƖƬ", "Orbit" },
            { "Atlas", "Atlas" }
        };
    
    [Utilities.Tooltip("Breaks mod checkers.")]
    public static void BreakModCheckers()
    {
        Hashtable hash = new Hashtable();
        foreach (string mod in modsForModCheck.Keys)
        {
            hash[mod] = true;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    
    [Utilities.Tooltip("Sets your name.")]
    public static void SetName(string name)
    {
        var computer = GorillaComputer.instance;
        if (computer == null) return;
        computer.currentName = name;
        computer.savedName = name;
        NetworkSystem.Instance.SetMyNickName(name);
        PlayerPrefs.SetString("playerName", name);
        PlayerPrefs.Save();
        VRRig.LocalRig.SetNameTagText(name);
    }

    [Utilities.Tooltip("Sets your name to HIDE.")]
    public static void SetNameHIDE()
    {
        SetName("HIDE");
    }

    [Utilities.Tooltip("Sets your name to SEEK.")]
    public static void SetNameSEEK()
    {
        SetName("SEEK");
    }

    [Utilities.Tooltip("Sets your name to RUN.")]
    public static void SetNameRUN()
    {
        SetName("RUN");
    }

    [Utilities.Tooltip("Sets your name to HIDDEN.")]
    public static void SetNameHIDDEN()
    {
        SetName("HIDDEN");
    }

    [Utilities.Tooltip("Sets your name to FOUND.")]
    public static void SetNameFOUND()
    {
        SetName("FOUND");
    }

    [Utilities.Tooltip("Sets your name to BEHINDYOU.")]
    public static void SetNameBEHINDYOU()
    {
        SetName("BEHINDYOU");
    }

    [Utilities.Tooltip("Sets your name to STATUE.")]
    public static void SetNameSTATUE()
    {
        SetName("STATUE");
    }

    [Utilities.Tooltip("Sets your name to GHOST.")]
    public static void SetNameGHOST()
    {
        SetName("GHOST");
    }

    [Utilities.Tooltip("Sets your name to HAUNT.")]
    public static void SetNameHAUNT()
    {
        SetName("HAUNT");
    }

    [Utilities.Tooltip("Sets your name to CREEP.")]
    public static void SetNameCREEP()
    {
        SetName("CREEP");
    }

    [Utilities.Tooltip("Sets your name to STALKER.")]
    public static void SetNameSTALKER()
    {
        SetName("STALKER");
    }

    [Utilities.Tooltip("Sets your name to 404.")]
    public static void SetName404()
    {
        SetName("404");
    }

    [Utilities.Tooltip("Spazes ur head when u hold right grip.")]
    public static void SpazHead()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x += Random.Range(1f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += Random.Range(1f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }
    }
    
    [Utilities.Tooltip("Spins ur head X.")]
    public static void SpinHeadX()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
        }
    }
    [Utilities.Tooltip("Spins ur head Y.")]
    public static void SpinHeadY()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
        }
    }
    [Utilities.Tooltip("Spins ur head Z.")]
    public static void SpinHeadZ()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }
    }
    [Utilities.Tooltip("Makes u an helicopter.")]
    public static void HelicopterRig()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;

            GorillaTagger.Instance.offlineVRRig.transform.position += new Vector3(0f, 0.05f, 0f);


            GorillaTagger.Instance.offlineVRRig.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));


            GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation *= Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.leftHand.trackingRotationOffset);
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation *= Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.rightHand.trackingRotationOffset);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;
        }
    }
    
    public static void GrabRig()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = VRRig.LocalRig.rightHandTransform.position;
        }
        else if (!InputHandler.Instance.RightGrip.IsPressed)
        {
            VRRig.LocalRig.enabled = true;
        }
    }
    
    public static void MoveRigGun()
    {
        GorillaTagger.Instance.offlineVRRig.enabled = true;

        GunLib.StartGun(() =>
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            GorillaTagger.Instance.offlineVRRig.transform.position = GunLib.GetPointerPos() + Vector3.up * 1f;
        }, false);
    }
    
    public static GameObject UCam;
    
    public static void SpectateGun()
    {
        GunLib.StartGun(() =>
        {
            if (GunLib.LockedPlayer == null)
                return;

            if (UCam == null)
            {
                UCam = new GameObject("Freecam boiiiiiiiiiiiiiiiiii");

                var c = UCam.AddComponent<Camera>();
                c.fieldOfView = 120;
                c.depth = 4;
                c.nearClipPlane = 0.1f;
                c.cameraType = CameraType.Game;

                UCam.transform.position = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position;
                UCam.transform.rotation = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.rotation;

                Object.DontDestroyOnLoad(UCam);
            }

            float lerpSpeed = 12f;

            UCam.transform.position = Vector3.Lerp(
                UCam.transform.position,
                GunLib.LockedPlayer.head.rigTarget.position,
                lerpSpeed * Time.deltaTime);

            UCam.transform.rotation = Quaternion.Slerp(
                UCam.transform.rotation,
                GunLib.LockedPlayer.head.rigTarget.rotation,
                lerpSpeed * Time.deltaTime);

        }, true);

        if (GunLib.LockedPlayer == null && UCam != null)
        {
            Object.Destroy(UCam);
            UCam = null;
        }
    }
}