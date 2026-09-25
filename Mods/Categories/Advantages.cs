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
}