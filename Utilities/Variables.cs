using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Undefined.Menu;
using Undefined.Mods;
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
    
    public static bool NotifySelf = false;
    public static bool NotifyOthers = false;
    public static bool HideReason = false;

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

    public static bool UseMinecraftFont = false;

    public static KeyCode keyboardButton = KeyCode.X;

    public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f);

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

    [Tooltip("Join the Undefined discord server.")]
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
    
    public static bool Overseer = true;

    public static bool IsMaster(bool notify = true)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            return true;

        if (notify)
        {
            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Error,
                "You are not the master client!",
                3f
            );
        }

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

    public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
    {
        var rotation = GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handRotOffset;

        return (
            GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handOffset,
            rotation,
            rotation * Vector3.up,
            rotation * Vector3.forward,
            rotation * Vector3.right
        );
    }

    public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
    {
        var rotation = GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handRotOffset;

        return (
            GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handOffset,
            rotation,
            rotation * Vector3.up,
            rotation * Vector3.forward,
            rotation * Vector3.right
        );
    }

    public static int[] bones = new int[]
        {
            4,
            3,
            5,
            4,
            19,
            18,
            20,
            19,
            3,
            18,
            21,
            20,
            22,
            21,
            25,
            21,
            29,
            21,
            31,
            29,
            27,
            25,
            24,
            22,
            6,
            5,
            7,
            6,
            10,
            6,
            14,
            6,
            16,
            14,
            12,
            10,
            9,
            7
        };

    public static Vector3 HeadPosition(VRRig rig)
    {
        try
        {
            if (rig.headMesh != null)
                return rig.headMesh.transform.position;
        }
        catch { }
        return rig.transform.position;
    }

    public static Vector3 RandomJitter()
    {
        Vector3 o = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
        return (o.sqrMagnitude < 0.01f ? Vector3.forward : o).normalized / 1.7f;
    }

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

    public static AssetBundle assetBundle = null;

    public static GameObject LoadAssetBundle(
        string bundleName,
        string assetName,
        int anchor = -1)
    {
        GameObject gameObject = null;

        Stream stream =
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(
                    "Undefined.Resources.Assets." + bundleName
                );

        if (stream != null)
        {
            if (assetBundle == null)
                assetBundle = AssetBundle.LoadFromStream(stream);

            GameObject prefab =
                assetBundle.LoadAsset<GameObject>(assetName);

            if (prefab == null)
            {
                Debug.LogError(
                    "Failed to find asset: " + assetName
                );

                return null;
            }

            gameObject =
                UnityEngine.Object.Instantiate(prefab);

            if (anchor >= 0)
            {
                Transform anchorTransform =
                    GetAnchor(anchor);

                if (anchorTransform != null)
                {
                    gameObject.transform.SetParent(
                        anchorTransform,
                        false
                    );
                }
            }
        }
        else
        {
            Debug.LogError(
                "Failed to load asset from resource: " + bundleName
            );
        }

        return gameObject;
    }
    
    private static Transform GetAnchor(int anchor)
    {
        switch (anchor)
        {
            case 0:
                return GorillaTagger.Instance.leftHandTransform;

            case 1:
                return GorillaTagger.Instance.leftHandTransform;

            case 2:
                return GorillaTagger.Instance.rightHandTransform;

            case 3:
                return GorillaTagger.Instance.headCollider.transform;

            default:
                return null;
        }
    }

    public static string ToTitleCase(string text) =>
    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());

    private static readonly Dictionary<string, GameObject> objectPool = new Dictionary<string, GameObject>();
    public static GameObject GetObject(string find)
    {
        if (objectPool.TryGetValue(find, out GameObject go))
            return go;

        GameObject tgo = GameObject.Find(find);
        if (!tgo && find.Contains("/"))
        {
            var split = find.Split('/');
            var rootName = split[0];

            var root = GameObject.Find(rootName);

            if (root != null)
            {
                var path = find[(rootName.Length + 1)..];
                var tr = root.transform.Find(path);

                if (tr != null)
                    tgo = tr.gameObject;
            }
        }
        if (tgo != null)
            objectPool.Add(find, tgo);

        return tgo;
    }

}

public class ModButtonInfo
{
    public string buttonText = "-";
    public string overlapText;
    public Action method;
    public Action enableMethod;
    public Action disableMethod;
    public bool enabled;
    public bool isTogglable = true;
    public string toolTip = "";

    public bool isIncremental;
    public List<string> incrementalValues = new();
    public int currentIncrementalIndex;
    public string incrementalDisplayName = "";
    public Action<string> incrementalMethod;

    public ModButtonInfo()
    {
    }

    public ModButtonInfo(string buttonText, Action method, bool isTogglable = true)
    {
        this.buttonText = buttonText;
        this.method = method;
        this.isTogglable = isTogglable;
        
        if (method != null)
        {
            var tooltipAttr = method.Method.GetCustomAttribute<TooltipAttribute>();
            if (tooltipAttr != null)
                this.toolTip = tooltipAttr.Tooltip;
        }
    }

    public ModButtonInfo(string buttonText, Action enableMethod, Action disableMethod)
    {
        this.buttonText = buttonText;
        this.enableMethod = enableMethod;
        this.disableMethod = disableMethod;
        isTogglable = true;
        
        if (enableMethod != null)
        {
            var tooltipAttr = enableMethod.Method.GetCustomAttribute<TooltipAttribute>();
            if (tooltipAttr != null)
                this.toolTip = tooltipAttr.Tooltip;
        }
    }

    public ModButtonInfo(string buttonText, Action method, Action disableMethod, bool isTogglable = true)
    {
        this.buttonText = buttonText;
        this.method = method;
        this.disableMethod = disableMethod;
        this.isTogglable = isTogglable;
        
        if (method != null)
        {
            var tooltipAttr = method.Method.GetCustomAttribute<TooltipAttribute>();
            if (tooltipAttr != null)
                this.toolTip = tooltipAttr.Tooltip;
        }
    }

    public ModButtonInfo(string buttonText, List<string> incrementalValues, Action<string> incrementalMethod, int currentIncrementalIndex = 0)
    {
        this.buttonText = buttonText;
        this.isTogglable = false;
        this.isIncremental = true;
        this.incrementalValues = incrementalValues;
        this.incrementalMethod = incrementalMethod;
        this.currentIncrementalIndex = currentIncrementalIndex;
    }

    public static ModButtonInfo Category(string name, Category category)
    {
        return new ModButtonInfo(
            name,
            () => Main.activeCategory = category,
            false
        );
    }

    public static ModButtonInfo Back(Category category)
    {
        return Category("Back", category);
    }

    public string GetCurrentIncrementalValue()
    {
        if (incrementalValues == null || incrementalValues.Count == 0)
            return null;

        if (currentIncrementalIndex >= incrementalValues.Count)
            currentIncrementalIndex = 0;

        return incrementalValues[currentIncrementalIndex];
    }

    public void CycleIncrementalValue()
    {
        if (incrementalValues == null || incrementalValues.Count == 0)
            return;

        currentIncrementalIndex = (currentIncrementalIndex + 1) % incrementalValues.Count;
        incrementalMethod?.Invoke(GetCurrentIncrementalValue());
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class TooltipAttribute : Attribute
{
    public string Tooltip { get; }

    public TooltipAttribute(string tooltip)
    {
        Tooltip = tooltip;
    }
}

public static class Extensions
{
    public static void Obliterate(this GameObject obj) => Object.Destroy(obj);
    public static void Obliterate(this Component comp) => Object.Destroy(comp);

    public static void Obliterate(this GameObject obj, float delay) => Object.Destroy(obj, delay);
    public static void Obliterate(this Component comp, float delay) => Object.Destroy(comp, delay);
}