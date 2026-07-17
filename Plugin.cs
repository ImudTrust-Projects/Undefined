using BepInEx;
using CXS;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using Undefined.Menu;
using Undefined.Mods.Categories;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using JObject = Newtonsoft.Json.Linq.JObject;
using GorillaLocomotion;

namespace Undefined;

[BepInPlugin(Constants.PluginGUID, Constants.PluginName, Constants.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public static int allsoundsids;
    public static Plugin Instance { get; private set; }

    public GameObject ComponentHolder { get; private set; }

    private Harmony harmony;

    private bool versionOkay;
    private bool initialized;

    private Version latestVersion;
    private Version minimumVersion;


    private void Awake()
    {
        Instance = this;

        ComponentHolder = new GameObject("Undefined");
        DontDestroyOnLoad(ComponentHolder);

        GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
    }


    private void Start()
    {
        CXS.CXS.LoadCXS();

        AudioHandler.LoadSounds();

        harmony = new Harmony(Constants.PluginGUID);
        harmony.PatchAll();

        ComponentHolder.AddComponent<Main>();
        ComponentHolder.AddComponent<NotificationLib>();
        ComponentHolder.AddComponent<RoomNotifications>();
        ComponentHolder.AddComponent<DiscordPresence>();

        Variables.LoadEmbeddedBackground("Undefined.Resources.Embedded.icon.png");

        StartCoroutine(StartVersionCheck());
    }

    private void OnPlayerSpawned()
    {
        if (initialized)
            return;

        initialized = true;
        allsoundsids = GTPlayer.Instance.materialData.Count;
        SoundMods.PopulateSoundOptions();

        if (ComponentHolder.GetComponent<InputHandler>() == null)
            ComponentHolder.AddComponent<InputHandler>();

        SettingsSaver.Load();

        StartCoroutine(WaitForVersionThenStartLoop());
    }

    private void OnDestroy()
    {
        GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);

        harmony?.UnpatchSelf();
    }

    private IEnumerator WaitForVersionThenStartLoop()
    {
        while (!versionOkay)
            yield return null;

        StartCoroutine(VersionLoop());
    }



    private IEnumerator StartVersionCheck()
    {
        yield return CheckVersion(true);
    }



    private IEnumerator VersionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(300f);

            yield return CheckVersion(false);
        }
    }



    private IEnumerator CheckVersion(bool startup)
    {
        using UnityWebRequest request = UnityWebRequest.Get(Constants.UndefinedDataUrl);

        yield return request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success)
        {
            if (startup)
            {
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Error,
                    "Unable to connect to Undefined servers."
                );
            }

            yield break;
        }


        JObject data = JObject.Parse(request.downloadHandler.text);


        latestVersion = new Version(
            data["menu-version"]?.ToString() ?? "0.0.0"
        );


        minimumVersion = new Version(
            data["min-version"]?.ToString() ?? "0.0.0"
        );


        Version current = new Version(Constants.PluginVersion);

        string buildType = Constants.BetaBuild ? "Beta Build" : "Release Build";



        if (current < minimumVersion)
        {
            versionOkay = false;

            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Error,
                $"Your Undefined {buildType} version is outdated. Please update the menu."
            );

            yield break;
        }



        if (current > latestVersion)
        {
            versionOkay = false;

            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Error,
                $"Modified or invalid Undefined {buildType} detected. Please download the official version."
            );

            yield break;
        }



        if (current < latestVersion)
        {
            if (startup)
            {
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Alert,
                    $"Undefined {buildType} update available.\nLatest: {latestVersion}\nCurrent: {current}"
                );
            }

            versionOkay = true;

            yield break;
        }



        if (startup)
        {
            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Info,
                $"Undefined {buildType} is up to date!"
            );
        }


        versionOkay = true;
    }
}