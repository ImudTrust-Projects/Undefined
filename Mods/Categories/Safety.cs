using GorillaNetworking;
using Photon.Voice.Unity;
using System.Linq;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Safety
{
    private static float delay;

    public static void AntiReport()
    {
        if (!NetworkSystem.Instance.InRoom) return;

        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
        {
            if (line.linePlayer != NetworkSystem.Instance.LocalPlayer) continue;

            Transform report = line.reportButton.gameObject.transform;

            foreach (VRRig vrrig in VRRigCache.ActiveRigs.Where(v =>
                         !v.isLocal &&
                         (Vector3.Distance(v.rightHandTransform.position, report.position) < 0.35f ||
                          Vector3.Distance(v.leftHandTransform.position, report.position) < 0.35f)))
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();

                if (!(Time.time > delay)) return;
                delay = Time.time + 1f;
            }
        }
    }
    static float ProximityThreshold = 0.35f, lastVol, startSilenceTime = -1f;
    private static bool reloaded;
    public static void BypassAutomod()
    {
        GorillaTagger.moderationMutedTime = -1f;

        if (GorillaComputer.instance.autoMuteType != "OFF")
        {
            GorillaComputer.instance.autoMuteType = "OFF";
            PlayerPrefs.SetInt("autoMute", 0);
            PlayerPrefs.Save();
        }

        Recorder mic = GorillaTagger.Instance.myRecorder;
        if (mic == null)
            return;

        float volume = 0f;
        GorillaSpeakerLoudness recorder = VRRig.LocalRig.GetComponent<GorillaSpeakerLoudness>();
        if (recorder != null)
            volume = recorder.Loudness;

        if (volume == 0f)
        {
            if (lastVol != 0f)
            {
                startSilenceTime = Time.time;
                reloaded = false;
            }

            if (startSilenceTime > 0f && !reloaded && Time.time - startSilenceTime >= 0.5f)
            {
                mic.RestartRecording(true);
                reloaded = true;
            }
        }
        else
        {
            startSilenceTime = -1f;
            reloaded = false;
        }

        lastVol = volume;
    }
}