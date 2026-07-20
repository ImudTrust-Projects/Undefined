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
using Object = UnityEngine.Object;

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

    public static void SetQuestScore(int score)
    {
        VRRig.LocalRig.SetQuestScore(score);
    }

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

    public static void UnlockFanClub()
    {
        SubscriptionPatches.enabled = !SubscriptionPatches.enabled;
    }
    
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

    public static void YellowName()
    {
        bool enabled = !(bool)GoldNameTag.GetValue(VRRig.LocalRig);

        GoldNameTag.SetValue(VRRig.LocalRig, enabled);

        VRRig.LocalRig.playerText1.color = enabled
            ? SubscriptionManager.SUBSCRIBER_NAME_COLOR
            : Color.white;
    }

    public static void FakeBodyTracking()
    {
        GorillaTagger.Instance.offlineVRRig.transform.rotation = Camera.main.transform.rotation;
        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.position = Variables.playerInstance.LeftHand.handFollower.transform.position;
        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.position = Variables.playerInstance.RightHand.handFollower.transform.position;
    }

    public static void RGBMonke()
    {
        float time = Time.time * 1.8f;
        var R = Mathf.Sin(time) * 0.5f + 0.5f;
        var G = Mathf.Sin(time + 2f * Mathf.PI / 3f) * 0.5f + 0.5f;
        var B = Mathf.Sin(time + 4f * Mathf.PI / 3f) * 0.5f + 0.5f;
        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { R, G, B });
    }
    
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
    
    public static void FastHoverboard()
    {
        Traverse FastHoverBoard = Traverse.Create(GorillaLocomotion.GTPlayer.Instance);
        FastHoverBoard.Field("hoverboardPaddleBoostMultiplier").SetValue(5f);
        FastHoverBoard.Field("hoverboardBoostGracePeriod").SetValue(0f);
        FastHoverBoard.Field("hoverboardPaddleBoostMax").SetValue(999f);
        FastHoverBoard.Field("hoverTiltAdjustsForwardFactor").SetValue(1f);
    }

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
    
    public static void BreakModCheckers()
    {
        Hashtable hash = new Hashtable();
        foreach (string mod in modsForModCheck.Keys)
        {
            hash[mod] = true;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

}