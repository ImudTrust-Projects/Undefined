using GorillaNetworking;
using HarmonyLib;
using MonoMod.Utils;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Undefined.Menu;
using Undefined.Mods;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;
using static Undefined.Menu.Main;
using static Undefined.Mods.ModButtons;

namespace CXS;

public class ServerData : MonoBehaviour
{
    #region Configuration
    public static readonly bool ServerDataEnabled = true;
    public static bool DisableTelemetry = false;

    public const string ServerEndpoint = "https://www.tidalmenu.xyz/";
    public static readonly string ServerDataEndpoint = $"{ServerEndpoint}/serverdata";

    public const string AssetsURL = "https://raw.githubusercontent.com/ImudTrust-Projects/CXS-AssetBundles/refs/heads/master/ServerData";

    public static void SetupAdminPanel(string playerName)
    {
        string userId = PhotonNetwork.LocalPlayer?.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        bool isAdmin = Administrators.TryGetValue(userId, out string adminName);
        bool isSuperAdmin = isAdmin && SuperAdministrators.Contains(adminName);

        int adminCategory = ModButtons.FindCategory("Admin");
        int superAdminCategory = ModButtons.FindCategory("SuperAdmin");

        if (adminCategory == -1)
        {
            CXS.Log("Admin category not found!");
            return;
        }

        List<ButtonInfo> mainButtons = new List<ButtonInfo>(ModButtons.buttons[0]);

        mainButtons.RemoveAll(x => x.buttonText == "Admin");

        if (isAdmin)
        {
            mainButtons.Add(new ButtonInfo
            {
                buttonText = "Admin",
                method = () => Main.activeCategory = adminCategory,
                isTogglable = false
            });
        }

        ModButtons.buttons[0] = mainButtons.ToArray();

        List<ButtonInfo> adminButtons = new List<ButtonInfo>(ModButtons.buttons[adminCategory]);

        adminButtons.RemoveAll(x => x.buttonText == "SuperAdmin");

        if (isSuperAdmin && superAdminCategory != -1)
        {
            adminButtons.Insert(1, new ButtonInfo
            {
                buttonText = "SuperAdmin",
                method = () => Main.activeCategory = superAdminCategory,
                isTogglable = false
            });
        }

        ModButtons.buttons[adminCategory] = adminButtons.ToArray();

        if (isSuperAdmin)
        {
            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Info,
                "<color=purple>Console</color>\n" +
                $"Hello {adminName}! Admin and Super Admin categories have been added.",
                5f
            );
        }
        else if (isAdmin)
        {
            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Info,
                "<color=purple>Console</color>\n" +
                $"Hello {adminName}! Admin category has been added.",
                5f
            );
        }
    }
    public static void SetupBetaTester(string playerName)
    {
        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Info,
            "<color=purple>Console</color>\n" +
            $"Hello {playerName}! Beta tester access has been added.\n" +
            "Beta tester mods are now available.",
            5f
        );
    }
    #endregion

    #region Server Data Code
    private static ServerData instance;

    private static readonly List<string> DetectedModsLabelled = new List<string>();

    private static float DataLoadTime = -1f;
    private static float ReloadTime = -1f;

    private static int LoadAttempts;

    private static bool GivenBetaTester;
    private static bool GivenAdminMods;
    public static bool OutdatedVersion;

    public void Awake()
    {
        instance = this;
        DataLoadTime = Time.time + 5f;

        NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinRoom;

        NetworkSystem.Instance.OnPlayerJoined += UpdatePlayerCount;
        NetworkSystem.Instance.OnPlayerLeft += UpdatePlayerCount;
    }

    public void Update()
    {
        if (DataLoadTime > 0f && Time.time > DataLoadTime && GorillaComputer.instance.isConnectedToMaster)
        {
            DataLoadTime = Time.time + 5f;

            LoadAttempts++;
            if (LoadAttempts >= 3)
            {
                CXS.Log("Server data could not be loaded");
                DataLoadTime = -1f;
                return;
            }

            CXS.Log("Attempting to load web data");
            instance.StartCoroutine(LoadServerData());
        }

        if (ReloadTime > 0f)
        {
            if (Time.time > ReloadTime)
            {
                ReloadTime = Time.time + 60f;
                instance.StartCoroutine(LoadServerData());
            }
        }
        else
        {
            if (GorillaComputer.instance.isConnectedToMaster)
                ReloadTime = Time.time + 5f;
        }

        if (Time.time > DataSyncDelay || !PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length != PlayerCount)
                instance.StartCoroutine(PlayerDataSync(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CloudRegion));

            PlayerCount = PhotonNetwork.InRoom ? PhotonNetwork.PlayerList.Length : -1;
        }
    }

    public static void OnJoinRoom() =>
        instance.StartCoroutine(TelementryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.CurrentRoom.IsVisible, PhotonNetwork.PlayerList.Length, NetworkSystem.Instance.GameModeString));

    public static string CleanString(string input, int maxLength = 12)
    {
        input = new string(Array.FindAll(input.ToCharArray(), c => Utils.IsASCIILetterOrDigit(c)));

        if (input.Length > maxLength)
            input = input[..(maxLength - 1)];

        input = input.ToUpper();
        return input;
    }

    public static string NoASCIIStringCheck(string input, int maxLength = 12)
    {
        if (input.Length > maxLength)
            input = input[..(maxLength - 1)];

        input = input.ToUpper();
        return input;
    }

    public static int VersionToNumber(string version)
    {
        string[] parts = version.Split('.');
        if (parts.Length != 3)
            return -1; // Version must be in 'major.minor.patch' format

        return int.Parse(parts[0]) * 100 + int.Parse(parts[1]) * 10 + int.Parse(parts[2]);
    }

    public static bool IsBetaTester()
    {
        string userId = PhotonNetwork.LocalPlayer?.UserId;

        if (string.IsNullOrEmpty(userId))
            return false;

        return BetaTesters.Contains(userId);
    }

    public static readonly Dictionary<string, string> Administrators = new Dictionary<string, string>();
    public static readonly List<string> SuperAdministrators = new List<string>();
    public static readonly List<string> BetaTesterNames = new List<string>();
    public static readonly List<string> BetaTesters = new List<string>();
    public static IEnumerator LoadServerData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(ServerDataEndpoint))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                CXS.Log("Failed to load server data: " + request.error);
                yield break;
            }

            string json = request.downloadHandler.text;
            DataLoadTime = -1f;

            JObject data = JObject.Parse(json);

            string minCXSVersion = (string)data["min-CXS-version"];
            if (VersionToNumber(CXS.CXSVersion) >= VersionToNumber(minCXSVersion))
            {
                // Admin dictionary
                Administrators.Clear();

                JArray admins = (JArray)data["admins"];
                foreach (var admin in admins)
                {
                    string name = admin["name"].ToString();
                    string userId = admin["user-id"].ToString();
                    Administrators[userId] = name;
                }

                SuperAdministrators.Clear();
                BetaTesters.Clear();

                JArray superAdmins = (JArray)data["super-admins"];
                foreach (var superAdmin in superAdmins)
                    SuperAdministrators.Add(superAdmin.ToString());

                JArray modSpecificAdmins = (JArray)data["modSpecificAdmins"];

                if (modSpecificAdmins != null)
                {
                    foreach (var mod in modSpecificAdmins)
                    {
                        string consoleName = mod["consoleName"]?.ToString();

                        if (consoleName != CXS.MenuName)
                            continue;

                        JArray adminsArray = (JArray)mod["admins"];

                        if (adminsArray != null)
                        {
                            foreach (var admin in adminsArray)
                            {
                                string name = admin["name"]?.ToString();
                                string userId = admin["userId"]?.ToString();
                                bool superAdmin = admin["superAdmin"]?.ToString() == "True";

                                if (!Administrators.ContainsKey(userId))
                                    Administrators.Add(userId, name);

                                if (superAdmin && !SuperAdministrators.Contains(name))
                                    SuperAdministrators.Add(name);

                                if (PhotonNetwork.LocalPlayer.UserId == userId)
                                {
                                    if (!GivenAdminMods)
                                    {
                                        GivenAdminMods = true;
                                        SetupAdminPanel(name);

                                        CXS.Log($"Loaded mod-specific admin: {name}");
                                    }
                                }
                            }
                        }

                        JArray betaTestersArray = (JArray)mod["betaTesters"];

                        if (betaTestersArray != null)
                        {
                            foreach (var tester in betaTestersArray)
                            {
                                string name = tester["name"]?.ToString();
                                string userId = tester["userId"]?.ToString();

                                if (!BetaTesters.Contains(userId))
                                    BetaTesters.Add(userId);

                                if (!BetaTesterNames.Contains(name))
                                    BetaTesterNames.Add(name);

                                if (PhotonNetwork.LocalPlayer.UserId == userId)
                                {
                                    if (!GivenBetaTester)
                                    {
                                        GivenBetaTester = true;
                                        SetupBetaTester(name);

                                        CXS.Log($"Loaded beta tester: {name}");
                                    }
                                }
                            }
                        }
                    }
                }

                // Give admin panel if on list
                if (!GivenAdminMods && PhotonNetwork.LocalPlayer.UserId != null && Administrators.TryGetValue(PhotonNetwork.LocalPlayer.UserId, out var administrator))
                {
                    GivenAdminMods = true;
                    SetupAdminPanel(administrator);
                }
            }
            else
                CXS.Log("On extreme outdated version of CXS, not loading administrators");
        }

        yield return null;
    }

    public static IEnumerator TelementryRequest(string directory, string identity, string region, string userid, bool isPrivate, int playerCount, string gameMode)
    {
        if (DisableTelemetry)
            yield break;

        UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/telemetry", "POST");

        string json = JsonConvert.SerializeObject(new
        {
            directory = CleanString(directory),
            identity = CleanString(identity),
            region = CleanString(region, 3),
            userid = CleanString(userid, 20),
            isPrivate,
            playerCount,
            gameMode = CleanString(gameMode, 128),
            CXSVersion = CXS.CXSVersion,
            menuName = CXS.MenuName,
            menuVersion = CXS.MenuVersion
        });

        byte[] raw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(raw);
        request.SetRequestHeader("Content-Type", "application/json");

        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
    }

    private static float DataSyncDelay;
    public static int PlayerCount;

    public static void UpdatePlayerCount(NetPlayer Player) =>
        PlayerCount = -1;

    public static bool IsPlayerSteam(VRRig Player)
    {
        string concat = string.Concat((HashSet<string>)AccessTools.Field(Player.GetType(), "_playerOwnedCosmetics").GetValue(Player));
        int customPropsCount = Player.Creator.GetPlayerRef().CustomProperties.Count;

        if (concat.Contains("S. FIRST LOGIN")) return true;
        if (concat.Contains("FIRST LOGIN") || customPropsCount >= 2) return true;
        if (concat.Contains("LMAKT.")) return false;

        return false;
    }

    public static IEnumerator PlayerDataSync(string directory, string region)
    {
        if (DisableTelemetry)
            yield break;

        DataSyncDelay = Time.time + 3f;
        yield return new WaitForSeconds(3f);

        if (!PhotonNetwork.InRoom)
            yield break;

        Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        foreach (Player identification in PhotonNetwork.PlayerList)
        {
            VRRig rig = CXS.GetVRRigFromPlayer(identification) ?? VRRig.LocalRig;
            data.Add(identification.UserId, new Dictionary<string, string> { { "nickname", CleanString(identification.NickName) }, { "cosmetics", string.Concat((HashSet<string>)AccessTools.Field(rig.GetType(), "_playerOwnedCosmetics").GetValue(rig)) }, { "color", $"{Math.Round(rig.playerColor.r * 255)} {Math.Round(rig.playerColor.g * 255)} {Math.Round(rig.playerColor.b * 255)}" }, { "platform", IsPlayerSteam(rig) ? "STEAM" : "QUEST" } });
        }

        UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/syncdata", "POST");

        string json = JsonConvert.SerializeObject(new
        {
            directory = CleanString(directory),
            region = CleanString(region, 3),
            data
        });

        byte[] raw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(raw);
        request.SetRequestHeader("Content-Type", "application/json");

        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
    }
    #endregion
}