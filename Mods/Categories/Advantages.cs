using GorillaGameModes;
using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Advantages
{
    public static void TagGun()
    {
        GunLib.StartGun(() =>
        {
            if (GunLib.LockedPlayer == null)
                return;

            if (!VRRig.LocalRig.IsTagged() || GunLib.LockedPlayer.IsTagged())
                return;

            Variables.bypasstp(
                GunLib.LockedPlayer.transform.position + new Vector3(0f, -2f, 0f),
                true
            );

            GameMode.ReportTag(GunLib.LockedPlayer.Creator);
        }, true);
    }

    public static void TagAll()
    {
        foreach (VRRig rig in VRRigCache.m_activeRigs)
        {
            if (rig == GorillaTagger.Instance.offlineVRRig)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                continue;
            }

            if (rig.mainSkin.material.name.Contains("fected"))
                continue;

            Variables.bypasstp(rig.transform.position, true);
            GameMode.ReportTag(rig.Creator);
        }
    }

    public static void TagSelf()
    {
        if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("infected"))
            return;

        foreach (VRRig player in VRRigCache.ActiveRigs)
        {
            if (player == GorillaTagger.Instance.offlineVRRig)
                continue;

            if (!player.mainSkin.material.name.Contains("infected"))
                continue;

            Variables.bypasstp(player.leftHandTransform.position, true);
            GameMode.ReportTag(player.Creator);
            break;
        }
    }

    private static float tagReachDistance = 2.5f;

    public static void TagReach()
    {
        if (!VRRig.LocalRig.IsTagged()) return;
        GorillaTagger.Instance.maxTagDistance = float.MaxValue;

        GorillaTagger.Instance.tagRadiusOverride = tagReachDistance;
        GorillaTagger.Instance.tagRadiusOverrideFrame = Time.frameCount + 16;
    }

    public static void TagFix()
    {
        GorillaTagger.Instance.maxTagDistance = float.MaxValue;
    }

    public static void DisableTagFix()
    {
        GorillaTagger.Instance.maxTagDistance = 1.2f;
    }
    
    public static void NoTagOnJoin()
    {
        PlayerPrefs.SetString("tutorial", "nope");
        PlayerPrefs.SetString("didTutorial", "nope");
        Hashtable hash = new Hashtable();
        hash.Add("didTutorial", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash, null, null);
        PlayerPrefs.Save();
    }

    public static void TrackingAbuseFlick()
    {
        if (!ControllerInputPoller.instance.rightControllerSecondaryButton)
        {
            return;
        }

        Transform head = GorillaTagger.Instance.headCollider.transform;
        Vector3 forward = head.forward.normalized;
        Vector3 right = head.right.normalized;
        float time = Time.time;
        Vector3 trackingJitter = new Vector3(
            Mathf.PerlinNoise(time * 10f, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, time * 10f) - 0.5f,
            Mathf.PerlinNoise(time * 10f, time * 10f) - 0.5f) * 0.05f;
        Vector3 handOffset = new Vector3(0f, 1.5f, 0f) + right * Mathf.Sin(time * 0.1f) * 0.3f;

        GorillaTagger.Instance.rightHandTransform.position = head.position + handOffset + trackingJitter + right * 0.2f;
        GorillaTagger.Instance.leftHandTransform.position = head.position + handOffset + trackingJitter - right * 0.2f;
        GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity = (-forward + right * 0.1f).normalized * 16f;
    }
    
    private static int oldFPS;

    public static void FPS(bool enable, int fps = 0)
    {
        if (enable)
        {
            oldFPS = Application.targetFrameRate;
            Application.targetFrameRate = fps;
        }
        else
        {
            Application.targetFrameRate = oldFPS;
        }
    }

    private static int oldFPSs;
    private static int oldVSync;

    public static void UnlockFps(bool enable)
    {
        if (enable)
        {
            oldFPSs = Application.targetFrameRate;
            oldVSync = QualitySettings.vSyncCount;

            Application.targetFrameRate = int.MaxValue;
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            Application.targetFrameRate = oldFPSs;
            QualitySettings.vSyncCount = oldVSync;
        }
    }
    
    public static void NoTagFreeze() =>
        GTPlayer.Instance.disableMovement = false;

    private static GameObject quitBox;
    
    public static void QuitBoxTP()
    {
        if (GTPlayer.Instance == null)
            return;

        quitBox ??= GameObject.Find("QuitBox");

        if (quitBox == null)
            return;

        if (Vector3.Distance(GTPlayer.Instance.transform.position, quitBox.transform.position) >= 1f)
            return;

        GTPlayer.Instance.TeleportTo(
            new Vector3(-68.647f, 12.406f, -83.699f),
            GTPlayer.Instance.transform.rotation,
            false,
            true
        );

        GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
    }
}