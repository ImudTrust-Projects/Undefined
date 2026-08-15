using BepInEx;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.Utilities.GunLib;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Overpowered
{
    public static void DestroyAll()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (Player p in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(p.ActorNumber);
            }
        }
    }
    public static void DestroyGun()
    {
        GunLib.StartGun(() =>
        {
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(GunLib.LockedPlayer.OwningNetPlayer.ActorNumber);
        }, true);
    }
    
    public static void STumpkickall()
    {
        GorillaComputer.instance.OnGroupJoinButtonPress(0, GorillaComputer.instance.friendJoinCollider);
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
        GunLib.StartGun(() =>
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
        GunLib.StartGun(() =>
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

    public static void ElevatorKickGun()
    {
        GunLib.StartGun(() =>
        {
            GRElevatorManager._instance.photonView.RPC("RemoteActivateTeleport", LockedPlayer.Creator.GetPlayerRef(), new object[] { GRElevatorManager._instance.currentLocation, GRElevatorManager.ElevatorLocation.GhostReactor, GRElevatorManager.LowestActorNumberInElevator() });
        }, true);
    }

    public static void ElevatorKickAll()
    {
        GRElevatorManager._instance.photonView.RPC("RemoteActivateTeleport", RpcTarget.Others, new object[] { GRElevatorManager._instance.currentLocation, GRElevatorManager.ElevatorLocation.GhostReactor, GRElevatorManager.LowestActorNumberInElevator() });
    }

    public static void shit()
    {
        ArtilleryCannonState.print("hello");
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
        GunLib.StartGun(() =>
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

    private const float AnchorResetTime = 5f;
    private const float SpamCooldownTime = 0.08f;
    private const float DisableAfter = 0.3f;

    private static readonly Dictionary<string, SnowballThrowable> Pool = new Dictionary<string, SnowballThrowable>();
    private static GameObject _anchor;
    private static Coroutine _disableRoutine;
    private static float _anchorCooldown;
    private static float _spamCooldown;
    private static float _rebuildAt;
    private static bool _seeded;

    private static float _flingCooldown;

    public static void SnowBallLauncherGun()
    {
        GunLib.StartGun(() =>
        {
            VRRig locked = GunLib.LockedPlayer;
            if (locked == null) return;
            if (Time.time <= _flingCooldown) return;

            Player target = RigManager.PlayerFromRig(locked);
            if (target == null) return;

            SnowballFlingTarget(target);
            _flingCooldown = Time.time + 0.1f;
        }, true);
    }

    public static void SnowballFlingTarget(Player target)
        => SnowballFlingTargetPower(target, -500f, 5f);
    
    private static float _upAwayCooldown;
    private const float UpAwayCooldownTime = 0.05f;

    public static void SnowballUpAwayGun()
    {
        GunLib.StartGun(() =>
        {
            if (Time.time <= _upAwayCooldown) return;

            VRRig locked = GunLib.LockedPlayer;
            if (locked == null) return;

            Player target = RigManager.PlayerFromRig(locked);
            if (target == null) return;

            SnowballUpAway(target);

            _upAwayCooldown = Time.time + UpAwayCooldownTime;
            
            Variables.RPCProtection();
        }, true);
    }

    public static void SnowballUpAway(Player target)
    {
        if (target == null) return;

        var rig = extarstuff.GetRigFromPlayer(target);
        if (rig == null) return;

        Vector3 position = rig.transform.position + Vector3.down * 1.2f;

        SpawnSnowball(
            position,
            Vector3.up * 2400f,
            8f,
            target: true,
            targets: new[] { target.ActorNumber },
            toofar: true
        );
        SpawnSnowball(
            position,
            Vector3.up * 2400f,
            8f,
            target: true,
            targets: new[] { target.ActorNumber },
            toofar: true
        );
    }

    public static void SnowballFlingTargetPower(Player target, float velocityY, float size)
    {
        if (target == null) return;
        var rig = extarstuff.GetRigFromPlayer(target);
        if (rig == null) return;

        Vector3 pos = Variables.HeadPosition(rig) + Vector3.up * 0.5f + Variables.RandomJitter();
        SpawnSnowball(pos, Vector3.up * velocityY, size,
            target: true,
            targets: new[] { target.ActorNumber },
            toofar: true);
    }

    public static int GetProjectileIncrement(Vector3 Position, Vector3 Velocity, float Scale)
    {
        return int.MaxValue;
    }

    public static void SpawnSnowball(Vector3 pos, Vector3 vel, float size,
        bool target = false, int[] targets = null,
        bool disable = false, bool toofar = false)
    {
        if (disable || !PhotonNetwork.InRoom) return;
        if (target && (targets == null || targets.Length == 0)) return;

        try
        {
            var throwable = GetThrowable();
            if (throwable == null) return;

            if (_disableRoutine != null)
                CoroutineManager.EndCoroutine(_disableRoutine);
            _disableRoutine = CoroutineManager.RunCoroutine(DisableSnowball());

            var options = target
                ? new RaiseEventOptions { TargetActors = targets }
                : new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            if (!target)
            {
                if (Time.time <= _spamCooldown) return;
                _spamCooldown = Time.time + SpamCooldownTime;
            }

            Vector3? archive = toofar ? MoveRigToSpawnPoint(pos, vel) : null;
            int increment = GetProjectileIncrement(pos, vel, size);

            var sizeEvent = GetField<PhotonEvent>(throwable, "changeSizeEvent");
            if (size != 0f && sizeEvent != null)
                Raise(176, new object[] { EventId(sizeEvent), (int)size }, options);

            var throwEvent = GetField<PhotonEvent>(throwable, "snowballThrowEvent");
            if (throwEvent != null)
                Raise(176, new object[] { EventId(throwEvent), pos, vel, increment, "bs" }, options);

            if (archive != null) RestoreRig(archive.Value);
        }
        catch (Exception e)
        {
            Debug.LogWarning("SpawnSnowball failed: " + e.Message);
            ReEnableRig();
        }
    }

    private static IEnumerator DisableSnowball()
    {
        yield return new WaitForSeconds(DisableAfter);
        GetProjectile("GrowingSnowballRightAnchor")?.SetSnowballActiveLocal(false);
    }
    
    public static class PhotonTimePatch
    {
        public static bool enabled;
        public static int distTime;
    }

    private static void Raise(byte code, object[] data, RaiseEventOptions options)
        => PhotonNetwork.RaiseEvent(code, data, options, SendOptions.SendReliable);

    private static int EventId(PhotonEvent e)
        => (int)Traverse.Create(e).Field("_eventId").GetValue();

    private static T GetField<T>(object source, string name)
        => Traverse.Create(source).Field(name).GetValue<T>();

    private static void RunViewUpdate()
    {
        var method = typeof(PhotonNetwork).GetMethod("RunViewUpdate",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static);
        method?.Invoke(null, null);
    }

    private static Vector3? MoveRigToSpawnPoint(Vector3 pos, Vector3 vel)
    {
        var rig = GorillaTagger.Instance?.offlineVRRig;
        if (rig == null) return null;

        Vector3 archive = rig.transform.position;
        rig.enabled = false;
        rig.transform.position = pos + Vector3.up * (vel.y > 0f ? -3f : 3f);
        PatchPhotonTime();
        return archive;
    }

    private static void PatchPhotonTime()
    {
        try
        {
            PhotonTimePatch.enabled = true;
            PhotonTimePatch.distTime = -50;
            RunViewUpdate();
            PhotonTimePatch.enabled = false;
            PhotonTimePatch.distTime = 0;
            RunViewUpdate();
        }
        catch
        {
            PhotonTimePatch.enabled = false;
            PhotonTimePatch.distTime = 0;
        }
    }

    private static void RestoreRig(Vector3 archive)
    {
        var rig = GorillaTagger.Instance?.offlineVRRig;
        if (rig == null) return;

        rig.transform.position = archive;
        rig.enabled = true;
        try { RunViewUpdate(); } catch { }
    }

    private static void ReEnableRig()
    {
        try
        {
            var rig = GorillaTagger.Instance?.offlineVRRig;
            if (rig != null) rig.enabled = true;
        }
        catch { }
    }

    private static SnowballThrowable GetThrowable()
    {
        if (Time.time > _anchorCooldown || _anchor == null)
        {
            var proj = GetProjectile("GrowingSnowballRightAnchor");
            if (proj == null) return null;

            _anchor = proj.gameObject;
            proj.SetSnowballActiveLocal(true);
            _anchorCooldown = Time.time + AnchorResetTime;
        }

        return _anchor == null
            ? null
            : _anchor.GetComponent<SnowballThrowable>() ??
              _anchor.GetComponentInChildren<SnowballThrowable>(true);
    }

    public static SnowballThrowable GetProjectile(string projectileName)
    {
        if (string.IsNullOrEmpty(projectileName)) return null;

        try
        {
            RebuildSnowballDict();

            string key = projectileName.EndsWith("(Clone)", StringComparison.Ordinal)
                ? projectileName
                : projectileName + "(Clone)";

            if (Pool.TryGetValue(key, out var exact) && exact != null)
                return exact;

            string bare = projectileName.Replace("(Clone)", "");
            foreach (var pair in Pool)
                if (pair.Value != null &&
                    pair.Key.IndexOf(bare, StringComparison.OrdinalIgnoreCase) >= 0)
                    return pair.Value;

            foreach (var t in Resources.FindObjectsOfTypeAll<SnowballThrowable>())
            {
                if (t == null || t.gameObject == null || !t.gameObject.scene.IsValid()) continue;
                string n = t.transform.parent != null ? t.transform.parent.gameObject.name : t.gameObject.name;
                if (n.IndexOf(bare, StringComparison.OrdinalIgnoreCase) < 0) continue;
                Pool[n] = t;
                return t;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("GetProjectile failed: " + e.Message);
        }
        return null;
    }

    private static void RebuildSnowballDict()
    {
        if (Pool.Count > 0 && Time.time < _rebuildAt) return;
        _rebuildAt = Time.time + 2f;

        SeedSnowballs();
        CollectFromMakers();

        if (Pool.Count == 0)
            CollectFromResources();
    }

    private static void SeedSnowballs()
    {
        try
        {
            if (_seeded || !CosmeticsV2Spawner_Dirty.isPrepared) return;

            var left = CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft;
            var right = CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight;
            if (left == null || right == null || left.Count < 1 || right.Count < 1) return;
            if (VRRig.LocalRig == null) return;

            _seeded = true;
            foreach (var id in left.Values)
                VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(id);
            foreach (var id in right.Values)
                VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(id);
        }
        catch { }
    }

    private static void CollectFromMakers()
    {
        try
        {
            foreach (var maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
            {
                if (maker == null || maker.snowballs == null) continue;
                foreach (var t in maker.snowballs)
                {
                    if (t == null || t.transform == null || t.transform.parent == null) continue;
                    try { Pool[t.transform.parent.gameObject.name] = t; } catch { }
                }
            }
        }
        catch { }
    }

    private static void CollectFromResources()
    {
        try
        {
            foreach (var t in Resources.FindObjectsOfTypeAll<SnowballThrowable>())
            {
                if (t == null || t.gameObject == null || !t.gameObject.scene.IsValid()) continue;
                string k = t.transform.parent != null ? t.transform.parent.gameObject.name : t.gameObject.name;
                Pool[k] = t;
            }
        }
        catch { }
    }
}