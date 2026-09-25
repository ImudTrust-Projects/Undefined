using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Undefined.Utilities;

public class RoomNotifications : MonoBehaviour
{
    private static string currentRoom = "";

    private void Update()
    {
        if (!NotificationLib.RoomNotifications)
            return;

        if (NetworkSystem.Instance.InRoom)
        {
            if (currentRoom != PhotonNetwork.CurrentRoom.Name)
            {
                currentRoom = PhotonNetwork.CurrentRoom.Name;

                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Room,
                    $"You joined the room \"{currentRoom}\""
                );
            }
        }
        else if (!string.IsNullOrEmpty(currentRoom))
        {
            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Room,
                $"You left the room \"{currentRoom}\""
            );

            currentRoom = "";
        }
    }
}

[HarmonyPatch(typeof(MonoBehaviourPunCallbacks))]
internal class RoomNotificationPatches
{
    [HarmonyPatch("OnPlayerEnteredRoom")]
    [HarmonyPostfix]
    private static void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!NotificationLib.RoomNotifications || newPlayer == PhotonNetwork.LocalPlayer)
            return;

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Room,
            $"\"{newPlayer.NickName}\" joined the room"
        );
    }

    [HarmonyPatch("OnPlayerLeftRoom")]
    [HarmonyPostfix]
    private static void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!NotificationLib.RoomNotifications || otherPlayer == PhotonNetwork.LocalPlayer)
            return;

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Room,
            $"\"{otherPlayer.NickName}\" left the room"
        );
    }
}