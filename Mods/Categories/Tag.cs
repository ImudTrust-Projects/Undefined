using GorillaGameModes;
using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Tag
{
    public static void TagGun()
    {
        if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
            return;

        VRRig rig = GorillaTagger.Instance.offlineVRRig;

        GunLib.start2guns(delegate ()
        {
            rig.enabled = false;
            rig.transform.position = GunLib.LockedPlayer.transform.position + new Vector3(0f, -2f, 0f);
            GameMode.ReportTag(GunLib.LockedPlayer.Creator);
            rig.enabled = true;
        }, true);
    }

    public static void TagAll()
    {
        if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
            return;

        foreach (VRRig rig in VRRigCache.m_activeRigs)
        {
            if (rig == GorillaTagger.Instance.offlineVRRig)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                continue;
            }

            if (rig.mainSkin.material.name.Contains("fected"))
                continue;

            GorillaTagger.Instance.offlineVRRig.enabled = true;
            GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
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

            GorillaTagger.Instance.offlineVRRig.enabled = false;
            GorillaTagger.Instance.offlineVRRig.transform.position = player.leftHandTransform.position;
            GameMode.ReportTag(player.Creator);
            GorillaTagger.Instance.offlineVRRig.enabled = true;
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
}