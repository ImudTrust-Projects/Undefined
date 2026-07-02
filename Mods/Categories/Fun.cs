using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;

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
}
