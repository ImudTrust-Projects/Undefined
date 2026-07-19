using Newtonsoft.Json.Linq;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using CXS;
using Photon.Pun;
using UnityEngine;

namespace Undefined.Menu;

public class DiscordPresence : MonoBehaviour
{
    private const string ClientId = "1527431390130475129";
    private const string LargeImageKey = "undefined_logo";
    private string Currentroom;
    private string Playersinlobby;

    public static DiscordPresence Instance;

    private NamedPipeClientStream pipe;
    private bool connected;
    private float retryTimer = 15f;
    private static long startTimestamp;

    private string details = "Undefined Menu";
    private string state = "In room: Just joined";

    private void Awake()
    {
        Instance = this;
        startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private void Update()
    {
        // Logic for room stuff idk
        Currentroom = !PhotonNetwork.InRoom ? "Not in a room😭" : PhotonNetwork.CurrentRoom.Name;
        Playersinlobby = !PhotonNetwork.InRoom ? "0" : PhotonNetwork.CurrentRoom.PlayerCount.ToString();

        string newState = "In room: " + Currentroom + " Current Players " + Playersinlobby;
        if (newState != state)
        {
            state = newState;
            if (connected)
                SendActivity();
        }

        // Other shit
        if (!Mods.Categories.Settings.DiscordRPC)
        {
            if (connected)
            {
                Disconnect();
                retryTimer = 15f;
            }
            return;
        }
        if (connected)
            return;

        retryTimer += Time.deltaTime;
        if (retryTimer >= 15f)
        {
            retryTimer = 0f;
            TryConnect();
        }
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
            try
            {
                var client = new NamedPipeClientStream(".", $"discord-ipc-{i}", PipeDirection.InOut, PipeOptions.Asynchronous);
                client.Connect(500);

                pipe = client;

                WriteFrame(0, new JObject { ["v"] = 1, ["client_id"] = ClientId }.ToString());

                connected = true;
                Task.Run(ReadLoop);
                Invoke(nameof(SendActivity), 1f);

                Debug.Log($"[Undefined] Discord RPC connected on discord-ipc-{i}");
                return;
            }
            catch
            {
                // pipe not available try the next one
            }
        }
    }

    private void SendActivity()
    {
        if (!connected)
            return;

        try
        {
            JObject payload = new JObject
            {
                ["cmd"] = "SET_ACTIVITY",
                ["args"] = new JObject
                {
                    ["pid"] = System.Diagnostics.Process.GetCurrentProcess().Id,
                    ["activity"] = new JObject
                    {
                        ["details"] = details,
                        ["state"] = state,
                        ["timestamps"] = new JObject { ["start"] = startTimestamp },
                        ["assets"] = new JObject
                        {
                            ["large_image"] = LargeImageKey,
                            ["large_text"] = "Undefined"
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
        byte[] data = Encoding.UTF8.GetBytes(json);
        byte[] frame = new byte[8 + data.Length];

        BitConverter.GetBytes(opcode).CopyTo(frame, 0);
        BitConverter.GetBytes(data.Length).CopyTo(frame, 4);
        data.CopyTo(frame, 8);

        pipe.Write(frame, 0, frame.Length);
        pipe.Flush();
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
            // Discord closed the pipe
        }

        Disconnect();
    }

    private void Disconnect()
    {
        connected = false;
        try { pipe?.Dispose(); } catch { }
        pipe = null;
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}