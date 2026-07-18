using GorillaGameModes;
using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Tag
{
    public static void TagGun()
    {
        VRRig rig = GorillaTagger.Instance.offlineVRRig;
        GunLib.start2guns(delegate ()
        {
            rig.enabled = false;
            rig.transform.position = GunLib.LockedPlayer.transform.position + new Vector3(0f, -2f, 0f);
            GameMode.ReportTag(GunLib.LockedPlayer.Creator);
        }, true);
        rig.enabled = true;
    }

    public static void TagAll()
    {
        foreach (VRRig vrrig in VRRigCache.m_activeRigs)
        {
            if (vrrig != GorillaTagger.Instance.offlineVRRig)
            {
                if (!vrrig.mainSkin.material.name.Contains("fected") && GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    GorillaTagger.Instance.offlineVRRig.transform.position = vrrig.transform.position;
                    GameMode.ReportTag(vrrig.Creator);
                }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }
    }

    public static void TagSelf()
    {
        foreach (VRRig Player in VRRigCache.ActiveRigs)
        {
            if (Player != GorillaTagger.Instance.offlineVRRig)
            {
                if (Player.mainSkin.material.name.Contains("infected"))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = Player.leftHandTransform.position;
                    GameMode.ReportTag(Player.Creator);
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    break;
                }
            }
        }
    }

    private static float tagReachDistance = 0.8f;

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
}
