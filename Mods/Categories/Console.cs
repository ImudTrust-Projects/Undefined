using CXS;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using Undefined.Menu;
using Undefined.Patches;
using Undefined.Utilities;
using UnityEngine;
using static Bindings;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Console
{
    #region No Admin Indicator
    private static int lastPlayerCount = -1;

    public static void EnableNoAdminIndicator()
    {
        CXS.CXS.ExecuteCommand("nocone", ReceiverGroup.Others, true);

        CXS.CXS.excludedCones.Add(PhotonNetwork.LocalPlayer);

        lastPlayerCount = -1;
    }

    public static void DisableNoAdminIndicator()
    {
        CXS.CXS.ExecuteCommand("nocone", ReceiverGroup.All, false);
    }

    public static void UpdateNoAdminIndicator()
    {
        if (PhotonNetwork.PlayerList.Length == lastPlayerCount)
            return;

        CXS.CXS.ExecuteCommand("nocone", ReceiverGroup.All, true);
        lastPlayerCount = PhotonNetwork.PlayerList.Length;
    }
    #endregion

    #region Admin Notificator
    public static void AdminNotificatorEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        CXS.CXS.ExecuteCommand("isusing", ReceiverGroup.All);
    }

    public static void AdminNotificatorDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private static void OnEvent(EventData data)
    {
        if (data.Code == 255)
        {
            CXS.CXS.ExecuteCommand("isusing", ReceiverGroup.All);
            return;
        }

        if (data.Code != CXS.CXS.CXSByte)
            return;

        if (data.CustomData is not object[] args)
            return;

        if (args.Length == 0 || (string)args[0] != "confirmusing")
            return;

        Player player = PhotonNetwork.CurrentRoom?.GetPlayer(data.Sender);

        if (player == null || player == PhotonNetwork.LocalPlayer)
            return;

        string menu = args.Length > 2 ? args[2]?.ToString() : "Unknown";
        string version = args.Length > 3 ? args[3]?.ToString() : "0.0.0";

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Info,
            $"{player.NickName} is using CXS version {version} - {menu}"
        );
    }
    #endregion

    #region Admin Mods
    private static float aaaaa;

    public static void AdminPunchMod()
    {
        if (Time.time > aaaaa)
        {
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                bool leftHand = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f;
                bool rightHand = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f;

                if (!rig.isLocal && (leftHand || rightHand))
                {
                    Vector3 vel = rightHand ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0);

                    CXS.CXS.ExecuteCommand("vel", RigManager.GetPlayerFromVRRig(rig).ActorNumber, vel);
                    aaaaa = Time.time + 0.1f;
                }
            }
        }
    }

    private static float beamDelay;

    public static void AdminBeam()
    {
        if (InputHandler.Instance.RightTrigger.IsPressed && Time.time > beamDelay)
        {
            beamDelay = Time.time + 0.05f;
            float h = Time.frameCount / 180f % 1f;
            Color color = Color.HSVToRGB(h, 1f, 1f);
            CXS.CXS.ExecuteCommand("lr", ReceiverGroup.All, color.r, color.g, color.b, color.a, 0.5f, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f), GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30) * 100f, 0.5f, Mathf.Sin((float)Time.frameCount / 30) * 100f), 0.1f);
        }
    }

    private static float startTimeTrigger;
    private static bool lastTriggerLaserSpam;

    public static void AdminFractals()
    {
        if (InputHandler.Instance.RightTrigger.IsPressed && !lastTriggerLaserSpam)
            startTimeTrigger = Time.time;

        lastTriggerLaserSpam = InputHandler.Instance.RightTrigger.IsPressed;

        if (InputHandler.Instance.RightTrigger.IsPressed && Time.time > beamDelay)
        {
            beamDelay = Time.time + 0.5f;
            float h = Time.frameCount / 180f % 1f;
            Color.HSVToRGB(h, 1f, 1f);
            CXS.CXS.ExecuteCommand("lr", ReceiverGroup.All, "lr", 0f, 1f, 1f, 0.3f, 0.25f, GorillaTagger.Instance.bodyCollider.transform.position, GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 1000f, 20f - (Time.time - startTimeTrigger));
        }
    }

    public static void AdminBringGun()
    {
        GunLib.start2guns(() =>
        {
            if (Time.time < adminEventDelay)
                return;

            adminEventDelay = Time.time + 0.1f;

            CXS.CXS.ExecuteCommand(
                "tpnv",
                RigManager.GetPlayerFromVRRig(GunLib.LockedPlayer).ActorNumber,
                GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f)
            );
        }, true);
    }

    public static void BringAllUsing()
    {
        if (Time.time > adminEventDelay)
        {
            adminEventDelay = Time.time + 0.05f;
            CXS.CXS.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
        }
    }

    private static float adminEventDelay;
    private static bool lastLasering;

    public static void AdminLaser()
    {
        if (InputHandler.Instance.LeftPrimary.IsPressed || InputHandler.Instance.RightPrimary.IsPressed)
        {
            Vector3 dir = InputHandler.Instance.RightPrimary.IsPressed
                                  ? VRRig.LocalRig.rightHandTransform.right
                                  : -VRRig.LocalRig.leftHandTransform.right;

            Vector3 startPos =
                    (InputHandler.Instance.RightPrimary.IsPressed
                             ? VRRig.LocalRig.rightHandTransform.position
                             : VRRig.LocalRig.leftHandTransform.position) + dir * 0.1f;

            try
            {
                Physics.Raycast(startPos + dir / 3f, dir, out RaycastHit Ray, 512f, Variables.NoInvisLayerMask());
                VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                if (gunTarget && !gunTarget.isLocal)
                    CXS.CXS.ExecuteCommand("silkick", ReceiverGroup.All,
                            gunTarget.Creator.UserId);
            }
            catch { }

            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.1f;
                CXS.CXS.ExecuteCommand("laser", ReceiverGroup.All, true,
                        InputHandler.Instance.RightPrimary.IsPressed);
            }
        }

        bool isLasering = InputHandler.Instance.LeftPrimary.IsPressed || InputHandler.Instance.RightPrimary.IsPressed;
        if (lastLasering && !isLasering)
            CXS.CXS.ExecuteCommand("laser", ReceiverGroup.All, false, false);

        lastLasering = isLasering;
    }

    private static float lastnetscale = 1f;
    private static float scalenetdel;
    private static int lastplayercountscale;

    public static void AdminNetworkScale()
    {
        float scale = InputHandler.Instance.RightTrigger.IsPressed ? 2f : 1f;

        if (Time.time > scalenetdel && (!Mathf.Approximately(lastnetscale, scale) || PhotonNetwork.PlayerList.Length != lastplayercountscale))
        {
            CXS.CXS.ExecuteCommand("scale", ReceiverGroup.All, scale);
            scalenetdel = Time.time + 0.05f;
            lastnetscale = scale;
            lastplayercountscale = PhotonNetwork.PlayerList.Length;
        }
    }

    public static void UnAdminNetworkScale()
    {
        CXS.CXS.ExecuteCommand("scale", ReceiverGroup.All, 1f);
    }
    #endregion

    #region Admin stuff
    public static void ConsoleBeacon(string id, string version, string menuName)
    {
        NetPlayer sender = extarstuff.GetPlayerFromID(id);
        VRRig vrrig = extarstuff.GetVRRigFromPlayer(sender);

        Color userColor = Color.red;

        NotificationLib.SendNotification(NotificationLib.NotificationType.Alert, "<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> " + sender.NickName + " is using " + menuName + " version " + version + ".");
        VRRig.LocalRig.PlayHandTapLocal(29, false, 99999f);
        VRRig.LocalRig.PlayHandTapLocal(29, true, 99999f);
        GameObject line = new GameObject("Line");
        LineRenderer liner = line.AddComponent<LineRenderer>();
        liner.startColor = userColor; liner.endColor = userColor; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 2; liner.useWorldSpace = true;

        liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
        liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
        liner.material.shader = Shader.Find("GUI/Text Shader");
        Object.Destroy(line, 3f);
    }

    public static void EnableAdminMenuUserTags()
    {
        if (!userTagHooked)
        {
            userTagHooked = true;
            PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;
        }
    }

    private static bool lastInRoom;
    private static int lastPlayerCountConduct = -1;

    public static bool userTagHooked;
    public static readonly Dictionary<string, string> onConduct = new Dictionary<string, string>();

    public static void AdminUserTagSys(EventData data)
    {
        try
        {
            if (data.Code != CXS.CXS.CXSByte)
                return;

            if (data.CustomData is not object[] args)
                return;

            if (args.Length == 0 || (string)args[0] != "confirmusing")
                return;

            Player sender = PhotonNetwork.CurrentRoom?.GetPlayer(data.Sender);
            if (sender == null || sender == PhotonNetwork.LocalPlayer)
                return;

            string menuVersion = args.Length > 1 ? (string)args[1] : "Unknown";
            string menuName = args.Length > 2 ? (string)args[2] : "Unknown";

            string userId = sender.UserId;

            if (!onConduct.ContainsKey(userId))
            {
                bool isAdmin = ServerData.Administrators.ContainsKey(userId);
                string displayName = sender.NickName + " - " + Variables.ToTitleCase(menuName);

                if (isAdmin)
                    displayName = "<color=red>" + displayName + "</color>";

                onConduct.Add(userId, displayName);
            }
            else
            {
                bool isAdmin = ServerData.Administrators.ContainsKey(userId);
                string displayName = sender.NickName + " - " + Variables.ToTitleCase(menuName);

                if (isAdmin)
                    displayName = "<color=red>" + displayName + "</color>";

                onConduct[userId] = displayName;
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"AdminUserTagSys error: {e.Message}");
        }
    }

    public static void ConsoleOnConduct()
    {
        bool currentInRoom = PhotonNetwork.InRoom;
        int currentPlayerCount = currentInRoom ? PhotonNetwork.PlayerList.Length : 0;

        if (currentInRoom && (!lastInRoom || currentPlayerCount != lastPlayerCountConduct))
            CXS.CXS.ExecuteCommand("isusing", ReceiverGroup.All);

        lastInRoom = currentInRoom;
        lastPlayerCountConduct = currentPlayerCount;

        string conductText = "";
        conductText += "<color=red>" + PhotonNetwork.LocalPlayer.NickName + " - " + Variables.ToTitleCase(CXS.CXS.MenuName) + "</color>\\n";

        List<string> keysToRemove = new List<string>();
        foreach (KeyValuePair<string, string> item in onConduct)
        {
            if (extarstuff.GetPlayerFromID(item.Key) == null)
                keysToRemove.Add(item.Key);
            else
                conductText += item.Value + "\\n";
        }

        foreach (string key in keysToRemove)
            onConduct.Remove(key);

        Variables.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().text = conductText;
    }

    public static void EnableCXSDetector()
    {
        PhotonNetwork.NetworkingClient.EventReceived += CXSDetectorEvent;
        CXS.CXS.ExecuteCommand("isusing", ReceiverGroup.All);
    }

    public static void DisableCXSDetector()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= CXSDetectorEvent;
    }

    public static void CXSDetectorEvent(EventData data)
    {
        try
        {
            if (data.Code != CXS.CXS.CXSByte)
                return;

            if (data.CustomData is not object[] args)
                return;

            if (args.Length == 0 || (string)args[0] != "confirmusing")
                return;

            Player sender = PhotonNetwork.CurrentRoom?.GetPlayer(data.Sender);
            if (sender == null || sender == PhotonNetwork.LocalPlayer)
                return;

            string menuVersion = args.Length > 1 ? (string)args[1] : "Unknown";
            string menuName = args.Length > 2 ? (string)args[2] : "Unknown";

            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Alert,
                $"{sender.NickName} is using {menuName} v{menuVersion}"
            );

            VRRig vrrig = extarstuff.GetVRRigFromPlayer(sender);
            if (vrrig != null)
            {
                Color color = CXS.CXS.GetMenuTypeName(menuName.ToLower());

                GameObject line = new GameObject("Beacon");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.startColor = color;
                liner.endColor = color;
                liner.startWidth = 0.25f;
                liner.endWidth = 0.25f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 5f, 0f));
                liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 5f, 0f));
                liner.material.shader = Shader.Find("GUI/Text Shader");
                Object.Destroy(line, 3f);
            }
        }
        catch { }
    }
    #endregion
}