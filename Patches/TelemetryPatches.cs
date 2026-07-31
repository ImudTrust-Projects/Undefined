using HarmonyLib;
using JetBrains.Annotations;
using Liv.Lck.Telemetry;
using PlayFab;
using PlayFab.EventsModels;
using System.Collections.Generic;

namespace Undefined.Patches;

public class TelemetryPatches
{
    public static bool enabled = true;

    [Plugin.PatchOnAwake]
    [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.EnqueueTelemetryEvent))]
    public class EnqueueTelemetryEvent
    {
        private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null) =>
            !enabled;
    }

    [Plugin.PatchOnAwake]
    [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.FlushMothershipTelemetry))]
    public class FlushMothershipTelemetry
    {
        private static bool Prefix() =>
            !enabled;
    }

    [Plugin.PatchOnAwake]
    [HarmonyPatch(typeof(LckTelemetryClient), nameof(LckTelemetryClient.SendTelemetry))]
    public class SendTelemetry
    {
        private static bool Prefix(LckTelemetryEvent lckTelemetryEvent) =>
            !enabled;
    }

    [Plugin.PatchOnAwake]
    [HarmonyPatch(typeof(PlayFabEventsAPI), nameof(PlayFabEventsAPI.WriteTelemetryEvents))]
    public class WriteTelemetryEvents
    {
        private static bool Prefix(WriteEventsRequest request, System.Action<WriteEventsResponse> resultCallback, System.Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null) =>
            !enabled;
    }
}
