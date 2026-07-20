using BepInEx;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.Utilities.GunLib;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Guardian
{
    public static TappableGuardianIdol[] guardianIdolcache = null;

    private static float Delay;

    public static void GuardianSelf()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaGuardianZoneManager.zoneManagers[0].SetGuardian(NetworkSystem.Instance.LocalPlayer);
        }
        else
        {
            if (guardianIdolcache == null)
            {
                guardianIdolcache = Object.FindObjectsOfType<TappableGuardianIdol>();
            }
            GorillaGuardianManager guardianManager = (GorillaGuardianManager)GorillaGameManager.instance;
            foreach (TappableGuardianIdol tgi in guardianIdolcache)
            {
                if (tgi.manager && tgi.manager.photonView && !tgi.isChangingPositions)
                {
                    GorillaGuardianZoneManager zoneManager = tgi.zoneManager;
                    if (!guardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer) && zoneManager.IsZoneValid() && tgi.manager)
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = tgi.transform.position;
                        VRRig.LocalRig.leftHand.rigTarget.transform.position = tgi.transform.position;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = tgi.transform.position;

                        if (Time.time > Delay)
                        {
                            Delay = Time.time + (zoneManager._currentActivationTime >= zoneManager.requiredActivationTime - 1f ? 0f : 0.2f);
                            tgi.OnTap(Random.Range(0f, 1f));
                            Variables.RPCProtection();
                        }
                    }
                }
                else
                    VRRig.LocalRig.enabled = true;
            }
        }
    }

    public static void GuardianGrabAll()
    {
        var g = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
        if (g == null || !g.IsPlayerGuardian(PhotonNetwork.LocalPlayer))
        {
            return;
        }
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (!rig.isOfflineVRRig)
                {
                    extarstuff.GetViewFromRig(rig).RPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                }
            }
        }
        else
        {
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (!rig.isOfflineVRRig)
                {
                    extarstuff.GetViewFromRig(rig).RPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 10f, 0f) });
                }
            }
        }
    }
    public static float flingCooldown = 0;
    public static void GuardianFlingAll()
    {
        var g = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
        if (g == null || !g.IsPlayerGuardian(PhotonNetwork.LocalPlayer))
        {
            return;
        }
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (!rig.isOfflineVRRig)
                {
                    if (flingCooldown < Time.time)
                    {
                        extarstuff.GetViewFromRig(rig).RPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        extarstuff.GetViewFromRig(rig).RPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(20f, Random.Range(-10, 10), 10f) });
                        flingCooldown = Time.time + 0.1f;
                    }
                }
            }
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;
        }
    }

    public static void GuardianSpazAll()
    {
        var g = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
        if (g == null || !g.IsPlayerGuardian(PhotonNetwork.LocalPlayer))
        {
            return;
        }
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (!rig.isOfflineVRRig)
                {
                    if (flingCooldown < Time.time)
                    {
                        extarstuff.GetViewFromRig(rig).RPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        extarstuff.GetViewFromRig(rig).RPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)) });
                        flingCooldown = Time.time + 0.1f;
                    }
                }
            }
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;
        }
    }
    
    // broken right now, I will fix it sometime!
    public static void GuardianAll()
    {
        if (!Variables.IsMaster()) return;

        int Players = 0;
        GorillaGuardianZoneManager.zoneManagers[0].SetGuardian(PhotonNetwork.PlayerList[Players]);
        Players++;
    }

    public static void DisableGuardianAll()
    {
        foreach (var ZoneManager in GorillaGuardianZoneManager.zoneManagers.Where(gorillaGuardianZoneManager => gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid()))
        {
            ZoneManager.SetGuardian(null);
        }
    }
}