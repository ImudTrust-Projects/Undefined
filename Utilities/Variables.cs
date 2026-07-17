using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Undefined.Utilities;

public class Variables
{
    public static string serverLink = "https://discord.gg/Bq94vsUtGk";

    public static string CosmeticsOwned;

    public static GameObject keyclickerObj1;

    public static GameObject keyclickerObj2;

    public static GameObject searchButton;

    public static bool InPcCondition;

    public static Texture2D backgroundTexture;

    public static GameObject activeMenu;
    public static GameObject bgObject;
    public static GameObject handPointer;
    public static GameObject menuCanvas;
    public static SphereCollider triggerCollider;
    public static Camera spectatorCamera;
    public static Text fpsLabel;
    public static int activePage = 0;
    public static int categoryIndex;

    public static bool hasSetupFeaturedMapVideo;
    public static VideoPlayer videoPlayer;

    public static bool fpsCounter = false;
    public static bool disconnectButton = true;
    public static bool rightHanded;

    public static bool rainbowOutline = true;
    public static float outlineSpeed = 0.5f;

    public static bool pcMenu = true;

    public static bool UseMinecraftFont = false; // this is a test

    public static KeyCode keyboardButton = KeyCode.X;

    public static Vector3 menuSize = new Vector3(0.13f, 1f, 1f); // Depth, width, height

    public static int buttonsPerPage = 8;

    public static float gradientSpeed = 0.5f;

    private static int? noInvisLayerMask;

    public static GTPlayer playerInstance;
    public static int NoInvisLayerMask()
    {
        noInvisLayerMask ??= ~(
            1 << LayerMask.NameToLayer("TransparentFX") |
            1 << LayerMask.NameToLayer("Ignore Raycast") |
            1 << LayerMask.NameToLayer("Zone") |
            1 << LayerMask.NameToLayer("Gorilla Trigger") |
            1 << LayerMask.NameToLayer("Gorilla Boundary") |
            1 << LayerMask.NameToLayer("GorillaCosmetics") |
            1 << LayerMask.NameToLayer("GorillaParticle"));

        return noInvisLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers;
    }

    public static void JoinDiscord() =>
           Process.Start(serverLink);

    public static void TeleportPlayer(Vector3 destination)
    {
        GTPlayer.Instance.TeleportTo(FormatTeleportPosition(destination), GTPlayer.Instance.transform.rotation);
        VRRig.LocalRig.transform.position = destination;
    }

    public static Vector3 FormatTeleportPosition(Vector3 pos) =>
        pos - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position;

    public static Vector3 RandomVector3(float range = 1f)
    {
        return UnityEngine.Random.insideUnitSphere * range;
    }

    public static Quaternion RandomQuaternion()
    {
        return UnityEngine.Random.rotationUniform;
    }

    public static Color RandomColor()
    {
        return (Color32)(new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue));
    }

    public static bool IsMaster()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            return true;
        }

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Error,
            "You are not the master client!",
            3f
        );

        return false;
    }

    public static void RPCProtection()
    {
        if (!PhotonNetwork.InRoom)
            return;

        try
        {
            MonkeAgent.instance.rpcErrorMax = int.MaxValue;
            MonkeAgent.instance.rpcCallLimit = int.MaxValue;
            MonkeAgent.instance.logErrorMax = int.MaxValue;

            PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
            PhotonNetwork.QuickResends = int.MaxValue;

            PhotonNetwork.SendAllOutgoingCommands();
        }
        catch { Debug.Log("RPC protection failed, are you in a lobby?"); }
    }


    // gun lib stuff

    public Vector3 PointerScale { get; set; } = new Vector3(0.2f, 0.2f, 0.2f);
    public Color32 PointerColorStart { get; set; } = new Color32(0, 255, 100, 255);
    public Color32 PointerColorEnd { get; set; } = new Color32(0, 200, 255, 255);
    public Color32 TriggeredPointerColorStart { get; set; } = new Color32(255, 100, 50, 255);
    public Color32 TriggeredPointerColorEnd { get; set; } = new Color32(255, 150, 0, 255);
    public float LineWidth { get; set; } = 0.03f;
    public Color32 LineColorStart { get; set; } = new Color32(0, 255, 150, 255);
    public Color32 LineColorEnd { get; set; } = new Color32(0, 180, 255, 255);
    public Color32 TriggeredLineColorStart { get; set; } = new Color32(255, 100, 50, 255);
    public Color32 TriggeredLineColorEnd { get; set; } = new Color32(255, 150, 0, 255);
    public bool EnableAnimations { get; set; } = true;
    public float PulseSpeed { get; set; } = 2f;
    public float PulseAmplitude { get; set; } = 0.04f;
    public bool EnableParticles { get; set; } = true;
    public float ParticleStartSize { get; set; } = 0.1f;
    public float ParticleStartSpeed { get; set; } = 0.5f;
    public int ParticleMaxCount { get; set; } = 100;
    public float ParticleEmissionRate { get; set; } = 20f;
    public bool EnableBoxESP { get; set; } = true;
    public float BoxESPWidth { get; set; } = 1f;
    public float BoxESPHeight { get; set; } = 2f;
    public Color32 BoxESPColor { get; set; } = new Color32(0, 255, 100, 255);
    public Color32 BoxESPOuterColor { get; set; } = new Color32(255, 150, 0, 255);
    public int LineCurve { get; set; } = 150;
    public float WaveFrequency { get; set; } = 5f;
    public float WaveAmplitude { get; set; } = 0.05f;

    internal bool isShooting;
    internal bool isTriggered;
    internal bool isLocked;

    public static void LoadEmbeddedBackground(string resourceName)
    {
        Debug.Log($"[Undefined] Loading embedded background: {resourceName}");

        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            string[] resourceNames = assembly.GetManifestResourceNames();
            Debug.Log($"[Undefined] Available embedded resources: {string.Join(", ", resourceNames)}");

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.LogError($"[Undefined] Resource not found: {resourceName}");
                    Debug.Log($"[Undefined] Available resources: {string.Join(", ", resourceNames)}");
                    return;
                }

                byte[] imageData = new byte[stream.Length];
                stream.Read(imageData, 0, imageData.Length);
                Debug.Log($"[Undefined] Read {imageData.Length} bytes from resource");

                backgroundTexture = new Texture2D(2, 2);
                if (backgroundTexture.LoadImage(imageData))
                {
                    Debug.Log($"[Undefined] Successfully loaded background image: {resourceName} ({backgroundTexture.width}x{backgroundTexture.height})");
                    backgroundTexture.filterMode = FilterMode.Point;
                    backgroundTexture.wrapMode = TextureWrapMode.Clamp;
                }
                else
                {
                    Debug.LogError("[Undefined] Failed to load embedded background image - LoadImage returned false");
                    backgroundTexture = null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Undefined] Error loading embedded background: {ex.Message}\n{ex.StackTrace}");
            backgroundTexture = null;
        }
    }
}


public class ButtonInfo
{
    public string buttonText = "-";
    public string overlapText = null;
    public Action method = null;
    public Action enableMethod = null;
    public Action disableMethod = null;
    public bool enabled = false;
    public bool isTogglable = true;
    public string toolTip = "";

    public string categoryName = null;

    public bool isIncremental = false;
    public List<string> incrementalValues = new List<string>();
    public int currentIncrementalIndex = 0;
    public string incrementalDisplayName = "";
    public Action<string> incrementalMethod = null;

    public string GetCurrentIncrementalValue()
    {
        if (incrementalValues != null && incrementalValues.Count > 0 && currentIncrementalIndex < incrementalValues.Count)
            return incrementalValues[currentIncrementalIndex];
        return null;
    }

    public void CycleIncrementalValue()
    {
        if (incrementalValues == null || incrementalValues.Count == 0) return;

        currentIncrementalIndex = (currentIncrementalIndex + 1) % incrementalValues.Count;

        incrementalMethod?.Invoke(GetCurrentIncrementalValue());
    }
}


public static class Extensions
{
    public static void Obliterate(this GameObject obj) => Object.Destroy(obj);
    public static void Obliterate(this Component comp) => Object.Destroy(comp);

    public static void Obliterate(this GameObject obj, float delay) => Object.Destroy(obj, delay);
    public static void Obliterate(this Component comp, float delay) => Object.Destroy(comp, delay);
}