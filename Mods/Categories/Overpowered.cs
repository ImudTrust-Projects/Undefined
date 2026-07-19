using BepInEx;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.Utilities.GunLib;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Overpowered
{
    public static HitTargetNetworkState[] tagetcache;

    public static void SpazTargets()
    {
        if (tagetcache == null)
        {
            tagetcache = Resources.FindObjectsOfTypeAll<HitTargetNetworkState>();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (HitTargetNetworkState item in tagetcache)
            {
                item.hitCooldownTime = 0;
                item.TargetHit(Vector3.zero, Vector3.zero);
            }
        }
    }

    public static void BreakTargets()
    {
        if (tagetcache == null)
        {
            tagetcache = Resources.FindObjectsOfTypeAll<HitTargetNetworkState>();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (HitTargetNetworkState item in tagetcache)
            {
                PhotonNetwork.Destroy(item.GetView);
            }
        }
    }

    public static void UntagSelf()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
            gorillaTagManager.currentInfected.Remove(PhotonNetwork.LocalPlayer);
        }
    }

    public static void UntagAll()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                gorillaTagManager.currentInfected.Remove(player);
            }
        }
    }

    public static void ForceTagLag()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
            gorillaTagManager.tagCoolDown = 200000;
        }
    }

    public static void NoTagCooldown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
            gorillaTagManager.tagCoolDown = 0;
        }
    }

    public static void BreakElevator()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveInstantiatedGO(GRElevatorManager._instance.gameObject, false);
        }
    }

    public static void DestroyAll()
    {
        PhotonNetwork.OpRemoveCompleteCache();
    }

    public static float hoverboarddelay = 0f;

    public static void HoverboardMinigun()
    {
        if (hoverboarddelay >= Time.time)
            return;

        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            FreeHoverboardManager.instance.SendDropBoardRPC(
                GorillaTagger.Instance.rightHandTransform.position,
                GorillaTagger.Instance.rightHandTransform.rotation,
                GorillaTagger.Instance.rightHandTransform.forward * 30f,
                Vector3.zero,
                new Color(0, 0, 0));

            hoverboarddelay = Time.time + 0.5f;
        }

        if (InputHandler.Instance.LeftGrip.IsPressed)
        {
            FreeHoverboardManager.instance.SendDropBoardRPC(
                GorillaTagger.Instance.leftHandTransform.position,
                GorillaTagger.Instance.leftHandTransform.rotation,
                GorillaTagger.Instance.leftHandTransform.forward * 30f,
                Vector3.zero,
                new Color(0, 0, 0));

            hoverboarddelay = Time.time + 0.5f;
        }
    }

    private static float waterdelay;

    public static void Watersplash()
    {
        if (Time.time <= waterdelay)
            return;

        waterdelay = Time.time + 0.1f;

        if (!PhotonNetwork.InRoom)
            return;

        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.myVRRig.SendRPC(
                "RPC_PlaySplashEffect",
                RpcTarget.All,
                GorillaTagger.Instance.rightHandTransform.position,
                GorillaTagger.Instance.rightHandTransform.rotation,
                100f,
                100f,
                true,
                false);

            Variables.RPCProtection();
        }

        if (InputHandler.Instance.LeftGrip.IsPressed)
        {
            GorillaTagger.Instance.myVRRig.SendRPC(
                "RPC_PlaySplashEffect",
                RpcTarget.All,
                GorillaTagger.Instance.leftHandTransform.position,
                GorillaTagger.Instance.leftHandTransform.rotation,
                100f,
                100f,
                true,
                false);

            Variables.RPCProtection();
        }
    }

    public static void Watergun()
    {
        start2guns(delegate ()
        {
            if (PhotonNetwork.InRoom)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = LockedPlayer.transform.position - new Vector3(0f, 1.9f, 0f);
                if (Time.time > waterdelay)
                {
                    waterdelay = Time.time + 0.3f;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, LockedPlayer.transform.position, LockedPlayer.transform.rotation, 100f, 100f, true, false);
                    Variables.RPCProtection();
                }
            }
        }, true);
        VRRig.LocalRig.enabled = LockedPlayer == null;
    }

    private static float LagDelay;

    public static void StutterMaster()
    {
        if (Time.time > LagDelay)
        {
            var whackamole = GameObject.FindObjectOfType<WhackAMole>().GetView;
            LagDelay = Time.time + 11f;
            for (int i = 0; i < 3850; i++)
            {
                whackamole.RPC("WhackAMoleButtonPressed", RpcTarget.MasterClient, null);
            }
        }
        Variables.RPCProtection();
    }

    public static void LagGun()
    {
        start2guns(() =>
        {
            if (Time.time > LagDelay)
            {
                for (int i = 0; i < 900; i++)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(3, new Hashtable() { }, new RaiseEventOptions() { TargetActors = new int[] { LockedPlayer.creator.ActorNumber } }, SendOptions.SendUnreliable);
                }
                Variables.RPCProtection();
                LagDelay = Time.time + 2.5f;
            }
        }, true);
    }

    public static void LagAll()
    {
        if (Time.time > LagDelay)
        {
            for (int i = 0; i < 900; i++)
            {
                PhotonNetwork.NetworkingClient.OpRaiseEvent(3, new Hashtable() { }, new RaiseEventOptions() { Receivers = ReceiverGroup.Others }, SendOptions.SendUnreliable);
            }
            Variables.RPCProtection();
            LagDelay = Time.time + 2.2f;
        }
    }

    public static void LagOnTouch()
    {
        if (Time.time > LagDelay)
        {
            foreach (VRRig vrrig in VRRigCache.ActiveRigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig &&
                    (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.headMesh.transform.position) < 0.25f ||
                     Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.headMesh.transform.position) < 0.25f ||
                     Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.bodyTransform.position) < 0.25f ||
                     Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.bodyTransform.position) < 0.25f))
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(
                        3,
                        new Hashtable(),
                        new RaiseEventOptions()
                        {
                            TargetActors = new int[] { vrrig.Creator.ActorNumber }
                        },
                        SendOptions.SendUnreliable
                    );

                    Variables.RPCProtection();
                }
            }

            LagDelay = Time.time + 2.2f;
        }
    }

    public static bool IsLocalPlayerGuardian() =>
        GorillaGuardianZoneManager.zoneManagers[0].IsPlayerGuardian(PhotonNetwork.LocalPlayer);

    public static void FlingGun()
    {
        try
        {
            GunLib.start2guns(() =>
            {
                try
                {
                    if (PhotonNetwork.InRoom && GunLib.LockedPlayer != null && Overpowered.IsLocalPlayerGuardian())
                    {
                        NetworkView view = extarstuff.GetNetViewFromVRRig(GunLib.LockedPlayer);

                        if (view != null)
                        {
                            view.SendRPC("GrabbedByPlayer", 1, true, false, false);
                            view.SendRPC("DroppedByPlayer", 1, new Vector3(0f, 9998.99f, 0f));
                        }
                    }
                }
                catch { }
            }, true);
        }
        catch { }
    }

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
}