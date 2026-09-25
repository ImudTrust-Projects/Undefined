using Newtonsoft.Json.Linq;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Constants;
using UnityEngine;
using Photon.Pun;

namespace Undefined.Menu;

public class DiscordPresence : MonoBehaviour
{
    private const string ClientId = "1527431390130475129";
    private const string LargeImageKey = "undefined_logo";

    public static DiscordPresence Instance;

    private NamedPipeClientStream pipe;
    private bool connected;

    private float retryTimer;
    private readonly int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

    private static long startTimestamp;

    private string details = "Undefined Menu";
    private string state = "Loading...";
    private string lastState = "";

    public static bool DiscordRPC = true; // some people maybe wanna show that they are using the best menu ever!!
    public bool privacyRPC = false; // some people maybe don't wanna be tracked?

    private void Awake()
    {
        Instance = this;
        startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private void Update()
    {
        if (!DiscordRPC)
        {
            if (connected)
                Disconnect();

            return;
        }

        if (!connected)
        {
            retryTimer += Time.deltaTime;

            if (retryTimer >= 3f)
            {
                retryTimer = 0f;
                TryConnect();
            }

            return;
        }

        UpdateRoomState();
    }

    private void UpdateRoomState()
    {
        string newState;

        if (privacyRPC)
        {
            newState = "Using Undefined Menu";
        }
        else
        {
            string room = NetworkSystem.Instance.InRoom
                ? PhotonNetwork.CurrentRoom.Name
                : "Not in a room";

            string players = NetworkSystem.Instance.InRoom
                ? PhotonNetwork.CurrentRoom.PlayerCount.ToString()
                : "0";

            newState = $"Room: {room} | Players: {players}";
        }

        if (newState == lastState)
            return;

        lastState = newState;
        state = newState;

        SendActivity();
    }

    public void SetPrivacyRPC(bool enabled)
    {
        privacyRPC = enabled;
        lastState = "";

        if (connected)
            UpdateRoomState();
    }

    public void SetPresence(string newDetails, string newState)
    {
        details = newDetails;
        state = newState;

        if (connected)
            SendActivity();
    }

    private void TryConnect()
    {
        for (int i = 0; i < 10; i++)
        {
            NamedPipeClientStream client = null;

            try
            {
                client = new NamedPipeClientStream(
                    ".",
                    $"discord-ipc-{i}",
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous
                );

                client.Connect(100);

                pipe = client;

                WriteFrame(0, new JObject
                {
                    ["v"] = 1,
                    ["client_id"] = ClientId
                }.ToString());

                connected = true;

                Task.Run(ReadLoop);

                SendActivity();

                Debug.Log($"[Undefined] Discord RPC connected on discord-ipc-{i}");

                return;
            }
            catch
            {
                client?.Dispose();
            }
        }
    }

    private void SendActivity()
    {
        if (!connected || pipe == null)
            return;

        try
        {
            JObject payload = new()
            {
                ["cmd"] = "SET_ACTIVITY",
                ["args"] = new JObject
                {
                    ["pid"] = pid,

                    ["activity"] = new JObject
                    {
                        ["details"] = details,
                        ["state"] = state,

                        ["timestamps"] = new JObject
                        {
                            ["start"] = startTimestamp
                        },

                        ["assets"] = new JObject
                        {
                            ["large_image"] = LargeImageKey,
                            ["large_text"] = "Undefined Menu"
                        }
                    }
                },

                ["nonce"] = Guid.NewGuid().ToString()
            };

            WriteFrame(1, payload.ToString());
        }
        catch
        {
            Disconnect();
        }
    }

    private void WriteFrame(int opcode, string json)
    {
        if (pipe == null || !pipe.IsConnected)
            return;

        byte[] data = Encoding.UTF8.GetBytes(json);

        byte[] frame = new byte[data.Length + 8];

        BitConverter.GetBytes(opcode).CopyTo(frame, 0);
        BitConverter.GetBytes(data.Length).CopyTo(frame, 4);

        Buffer.BlockCopy(data, 0, frame, 8, data.Length);

        pipe.Write(frame, 0, frame.Length);
    }

    private void ReadLoop()
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (pipe != null && pipe.IsConnected)
            {
                if (pipe.Read(buffer, 0, buffer.Length) <= 0)
                    break;
            }
        }
        catch
        {
        }

        Disconnect();
    }

    private void Disconnect()
    {
        connected = false;

        try
        {
            pipe?.Dispose();
        }
        catch
        {
        }

        pipe = null;
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}