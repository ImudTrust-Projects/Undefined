using BepInEx;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GorillaLocomotion;
using HarmonyLib;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.Utilities.GunLib;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Overpowered
{
    public static void DestroyAll()
    {
        PhotonNetwork.OpRemoveCompleteCache();
    }

    private static float delay = 0.5f;

    public static void BreakMovementAll()
    {
        try
        {
            if (!PhotonNetwork.InRoom) return;
            float currentTime = Time.time;
            if (currentTime > delay)
            {
                delay = currentTime + 0.5f;

                if (!IsLocalPlayerGuardian()) return;
                var activeRigs = VRRigCache.ActiveRigs;

                foreach (VRRig vrrig in activeRigs)
                {
                    if (vrrig == null || vrrig.isMyPlayer) continue;

                    NetworkView netView = extarstuff.GetNetViewFromVRRig(vrrig);
                    if (netView != null)
                    {
                        Vector3 groundPosition = vrrig.transform.position;
                        groundPosition.y = -10f;

                        netView.SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        netView.SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { (groundPosition - vrrig.transform.position) * 100f });
                    }
                }
            }
        }
        catch { }
    }

    private static float grabCooldown;

    private static bool HasGrabbableHand(VRRig rig)
    {
        if (rig == null)
            return false;

        return rig.leftHandLink.CanBeGrabbed() || rig.rightHandLink.CanBeGrabbed();
    }

    private static void SetGrabPatch(bool state)
    {
        Patches.GrabPatches.GrabPatch.enabled = state;

        if (!state)
            VRRig.LocalRig.enabled = true;
    }

    private static void GrabPlayer(VRRig rig, Vector3 position)
    {
        if (rig == null || rig.isLocal)
            return;

        if (!HasGrabbableHand(rig))
        {
            SetGrabPatch(false);
            VRRig.LocalRig.BreakHandLinks();
            return;
        }

        SetGrabPatch(true);

        VRRig.LocalRig.enabled = false;
        VRRig.LocalRig.transform.position = position;

        bool useLeftHand = rig.leftHandLink.CanBeGrabbed();

        var targetHand = useLeftHand ? rig.leftHandLink : rig.rightHandLink;
        var localHand = useLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;

        if (targetHand.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
            return;

        if (grabCooldown <= Time.time)
        {
            VRRig.LocalRig.transform.position = rig.syncPos;
            localHand.TentacleTryCreateLink(targetHand);
        }

        grabCooldown = Mathf.Max(targetHand.rejectGrabsUntilTimestamp, Time.time + 0.2f);
    }

    public static void GrabFlingGun()
    {
        GunLib.start2guns(() =>
        {
            Vector3 flingPosition = new(
                UnityEngine.Random.value < 0.5f ? -95000f : 95000f,
                95000f,
                UnityEngine.Random.value < 0.5f ? -95000f : 95000f);

            GrabPlayer(GunLib.LockedPlayer, flingPosition);
        }, true);

        bool isHoldingInput =
            InputHandler.Instance.RightGrip.IsPressed ||
            InputHandler.Instance.LeftGrip.IsPressed ||
            InputHandler.Instance.RightTrigger.IsPressed ||
            InputHandler.Instance.LeftTrigger.IsPressed;

        if (isHoldingInput || !Patches.GrabPatches.GrabPatch.enabled)
            return;

        VRRig.LocalRig.BreakHandLinks();
        SetGrabPatch(false);
    }

    public static void GrabFlingAll()
    {
        foreach (var rig in VRRigCache.ActiveRigs)
        {
            if (rig == null || rig.isMyPlayer || rig.isOfflineVRRig || !HasGrabbableHand(rig))
                continue;

            Vector3 flingPosition = new(
                UnityEngine.Random.value < 0.5f ? -95000f : 95000f,
                95000f,
                UnityEngine.Random.value < 0.5f ? -95000f : 95000f);

            GrabPlayer(rig, flingPosition);
        }

        bool isHoldingInput =
            InputHandler.Instance.RightGrip.IsPressed ||
            InputHandler.Instance.LeftGrip.IsPressed ||
            InputHandler.Instance.RightTrigger.IsPressed ||
            InputHandler.Instance.LeftTrigger.IsPressed;

        if (isHoldingInput || !Patches.GrabPatches.GrabPatch.enabled)
            return;

        VRRig.LocalRig.BreakHandLinks();
        SetGrabPatch(false);
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

    public static void Flinggunv2()
    {
        GunLib.start2guns(delegate ()
        {
            NetPlayer slapper = NetworkSystem.Instance.LocalPlayer;
            NetPlayer target = GunLib.LockedPlayer.Creator;

            RigContainer targetRig;
            if (!VRRigCache.Instance.TryGetVrrig(target, out targetRig))
                return;
            Vector3 handVelocity = GTPlayer.Instance.GetHandVelocityTracker(false).GetAverageVelocity(true);
            if (handVelocity.magnitude < 6f)
            {
                handVelocity = new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(5f, 15f),
                    Random.Range(10f, 20f)
                );
            }

            Vector3 clampedVelocity = Vector3.ClampMagnitude(handVelocity, 20f);
            Vector3 launchVelocity = clampedVelocity * 1f;

            Vector3 groundNormal;
            if (targetRig.Rig.IsOnGround(1.2f, 0.4f, out groundNormal))
            {
                launchVelocity += groundNormal * 3f * Mathf.Clamp01(1f - Vector3.Dot(groundNormal, launchVelocity.normalized));
            }

            GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC(
                "GuardianLaunchPlayer",
                target,
                launchVelocity
            );

        }, true);
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
}