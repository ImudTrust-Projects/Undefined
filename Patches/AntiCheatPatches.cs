using HarmonyLib;
using Photon.Pun;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Patches;

public class AntiCheatPatches
{
    
    [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.SendReport))]
    public class SendReportPatch
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
            if (susReason.ToLower() == "empty rig")
                return false;

            if (Variables.NotifySelf && susId == PhotonNetwork.LocalPlayer.UserId)
            {
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.AntiCheat,
                    $"You were reported for {(Variables.HideReason ? "hidden reason" : susReason)}."
                );
            }

            if (Variables.NotifyOthers && susId != PhotonNetwork.LocalPlayer.UserId)
            {
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.AntiCheat,
                    $"{susNick} was reported for {(Variables.HideReason ? "hidden reason" : susReason)}."
                );
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(MonkeAgent), "CloseInvalidRoom")]
    public class NoCloseInvalidRoom
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "CheckReports")]
    public class NoCheckReports
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "DispatchReport")]
    public class NoDispatchReport
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "LogErrorCount")]
    public class NoLogErrorCount
    {
        private static bool Prefix(string logString, string stackTrace, LogType type) =>
            false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "QuitDelay", MethodType.Enumerator)]
    public class NoQuitDelay
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
    public class NoQuitOnBan
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "ShouldDisconnectFromRoom")]
    public class NoShouldDisconnectFromRoom
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
    public class GracePeriodPatch1
    {
        private static bool Prefix() =>
            false;
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
    public class GracePeriodPatch2
    {
        private static bool Prefix() =>
            false;
    }
}

