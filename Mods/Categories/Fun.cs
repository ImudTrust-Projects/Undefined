using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;
using UnityEngine;

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

    public static void Rainbowhoverboard()
    {
        if (Hoverboard_Stuff.IsHeld)
        {
            float time = Time.time * 1.8f;
            var R = Mathf.Sin(time) * 0.5f + 0.5f;
            var G = Mathf.Sin(time + 2f * Mathf.PI / 3f) * 0.5f + 0.5f;
            var B = Mathf.Sin(time + 4f * Mathf.PI / 3f) * 0.5f + 0.5f;
            Color RGB = new Color(R, G, B);
            VRRig.LocalRig.hoverboardVisual.SetIsHeld(Hoverboard_Stuff.Hand, Hoverboard_Stuff.HandPosition, Hoverboard_Stuff.HandRotation, RGB);
        }
    }

    public static void Colorhoverboard(Color color)
    {
        if (!Hoverboard_Stuff.IsHeld) return;

        VRRig.LocalRig.hoverboardVisual.SetIsHeld(Hoverboard_Stuff.Hand, Hoverboard_Stuff.HandPosition, Hoverboard_Stuff.HandRotation, color);
    }
}

public struct Hoverboard_Stuff
{
    public static bool Hand = VRRig.LocalRig.hoverboardVisual.IsLeftHanded;
    public static Vector3 HandPosition = VRRig.LocalRig.hoverboardVisual.NominalLocalPosition;
    public static Quaternion HandRotation = VRRig.LocalRig.hoverboardVisual.NominalLocalRotation;
    public static bool IsHeld = VRRig.LocalRig.hoverboardVisual.IsHeld;
}
