using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Room
{
    public static void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public static void JoinRandomPublic()
    {
        GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest");
        PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger, GorillaNetworking.JoinType.Solo);
    }

    public static void PrimaryDisconnect()
    {
        if (InputHandler.Instance.RightPrimary.WasPressed | UnityInput.Current.GetKey(KeyCode.F))
        {
            PhotonNetwork.Disconnect();
        }
    }

    public static void Servers(string svr)
    {
        string currentsvr = PhotonNetwork.CloudRegion;
        if (!string.IsNullOrEmpty(currentsvr))
            currentsvr = currentsvr.Replace("/*", "");

        if (currentsvr != svr)
            PhotonNetwork.ConnectToRegion(svr);

        NetworkSystem.Instance.currentRegionIndex = Array.IndexOf(NetworkSystem.Instance.regionNames, svr);

        NetworkSystemPUN punNetwork = (NetworkSystemPUN)NetworkSystem.Instance;
        for (int i = 0; i < punNetwork.regionData.Length; i++)
        {
            NetworkRegionInfo regionInfo = punNetwork.regionData[i];
            regionInfo.pingToRegion = Array.IndexOf(NetworkSystem.Instance.regionNames, regionInfo) == i ? 0 : 9999;
        }
    }

    public static void JoinRoom(string RoomCode)
    {
        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(RoomCode, GorillaNetworking.JoinType.Solo);
    }

    public static void EnableAntiAFK()
    {
        PhotonNetworkController.Instance.disableAFKKick = true;
    }

    public static void DisableAntiAFK()
    {
        PhotonNetworkController.Instance.disableAFKKick = false;
    }

    public static void DisableNetworkTriggers()
    {
        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false);
    }

    public static void EnableNetworkTriggers()
    {
        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true);
    }

    public static void GetIdSelf()
    {
        string id = PhotonNetwork.LocalPlayer.UserId;

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Info,
            $"Copied ID: {id}"
        );

        GUIUtility.systemCopyBuffer = id;
    }

    public static void GetIdGun()
    {
        GunLib.start2guns(() =>
        {
            var targetedPlayer = GunLib.LockedPlayer;

            if (targetedPlayer != null)
            {
                string id = targetedPlayer.Creator.UserId;
                GUIUtility.systemCopyBuffer = id;

                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Info,
                    $"Copied ID: {id}"
                );
            }
        }, true);
    }
}