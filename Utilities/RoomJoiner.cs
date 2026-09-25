using Photon.Pun;
using UnityEngine;

namespace Undefined.Utilities;

public class RoomJoiner : MonoBehaviour
{
    private string roomCode = "";
    private Rect windowRect = new Rect(20, 20, 320, 120);

    private void OnGUI()
    {
        windowRect = GUI.Window(12345, windowRect, DrawWindow, "Room Joiner");
    }

    private void DrawWindow(int windowID)
    {
        GUI.Label(new Rect(10, 25, 90, 25), "Room Code:");

        roomCode = GUI.TextField(
            new Rect(90, 25, 210, 25),
            roomCode
        );

        if (GUI.Button(new Rect(10, 60, 140, 35), "Join"))
        {
            JoinRoom();
        }

        if (GUI.Button(new Rect(160, 60, 140, 35), "Disconnect"))
        {
            Disconnect();
        }

        GUI.DragWindow();
    }

    private void JoinRoom()
    {
        string code = roomCode.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
            return;

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        PhotonNetwork.JoinRoom(code);
    }

    private void Disconnect()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
}