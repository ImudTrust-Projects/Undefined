using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using GorillaLocomotion.Gameplay;
using Photon.Realtime;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Master
{
    private static HalloweenGhostChaser lucy;
    private static GameObject terraformer;
    private static float delay;

    
    public static void GreyScreen()
    {
        if (GreyZoneManager.Instance == null) return;

        if (!Variables.IsMaster()) return;

        GreyZoneManager.Instance.ActivateGreyZoneAuthority();

        // Patched
        /*GTPlayer.Instance?.SetGravityOverride(
            GreyZoneManager.Instance,
            GreyZoneManager.Instance.GravityOverrideFunction
        );*/
    }

    public static void DisableGreyScreen()
    {
        if (GreyZoneManager.Instance == null) return;

        if (!Variables.IsMaster()) return;

        GTPlayer.Instance?.UnsetGravityOverride(GreyZoneManager.Instance);

        GreyZoneManager.Instance.DeactivateGreyZoneAuthority();
    }
    
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
    
    public static void ViberateGun()
    {
        if (!Variables.IsMaster())
            return;

        GunLib.StartGun(() =>
        {
            if (GunLib.LockedPlayer == null)
                return;

            PhotonNetwork.RaiseEvent(3,
                new object[]
                {
                    PhotonNetwork.ServerTimestamp,
                    (byte)2,
                    new object[] { 1 }
                },
                new RaiseEventOptions
                {
                    TargetActors = new[] { GunLib.LockedPlayer.Creator.ActorNumber }
                },
                SendOptions.SendUnreliable);
        }, true);
    }
    
    public static void ViberateAll()
    {
        if (!Variables.IsMaster())
            return;

        PhotonNetwork.RaiseEvent(3,
            new object[]
            {
                PhotonNetwork.ServerTimestamp,
                (byte)2,
                new object[] { 1 }
            },
            new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            },
            SendOptions.SendUnreliable);
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
    public static void shidiik()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveInstantiatedGO(GameEntityManager.activeManager.gameObject, false);
        }
    }
    
    public static void UnlockRoom()
    {
        if (!NetworkSystem.Instance.InRoom || !Variables.IsMaster())
            return;

        PhotonNetwork.CurrentRoom.IsVisible = true;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
    }

    public static void LockRoom()
    {
        if (!NetworkSystem.Instance.InRoom || !Variables.IsMaster())
            return;
        
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
    }
    public static void SpazRoom()
    {
        if (!NetworkSystem.Instance.InRoom || !Variables.IsMaster())
            return;
        
        for (int i = 0; i < 100; i++)
        {
            PhotonNetwork.CurrentRoom.IsVisible = (i % 2 == 0);
            PhotonNetwork.CurrentRoom.IsOpen = (i % 2 == 0);
        }
        GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
    }
    
    
    
    private static void AddInfected(NetPlayer plr)
    {
        if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null || plr == null)
            return;

        var tagManager = GorillaGameManager.instance as GorillaTagManager;
        if (tagManager == null)
            return;

        if (tagManager.isCurrentlyTag)
        {
            tagManager.ChangeCurrentIt(plr, true);
        }
        else if (tagManager.currentInfected != null && !tagManager.currentInfected.Contains(plr))
        {
            tagManager.AddInfectedPlayer(plr, true);
        }
    }

    private static void RemoveInfected(NetPlayer plr)
    {
        if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null || plr == null)
        {
            return;
        }

        if (GorillaGameManager.instance is GorillaTagManager tagManager)
        {
            if (tagManager.isCurrentlyTag)
            {
                if (tagManager.currentIt == plr)
                {
                    tagManager.currentIt = null;
                }
            }
            else
            {
                tagManager.currentInfected?.Remove(plr);
            }
        }
    }
    public static void MatPlayer(NetPlayer netPlayer)
    {
        if (netPlayer == null || !Variables.IsMaster())
            return;

        if (GorillaGameManager.instance is not GorillaTagManager tagManager)
        {
            AddInfected(netPlayer);
            return;
        }

        if (tagManager.isCurrentlyTag)
        {
            if (tagManager.currentIt == netPlayer)
                RemoveInfected(netPlayer);
            else
                AddInfected(netPlayer);

            return;
        }

        if (tagManager.currentInfected != null && tagManager.currentInfected.Contains(netPlayer))
        {
            RemoveInfected(netPlayer);
            return;
        }

        AddInfected(netPlayer);
    }
    
    public static void MatGun()
    {
        GunLib.StartGun(() =>
        {
            if (NetworkSystem.Instance.InRoom && PhotonNetwork.IsMasterClient && Time.time > delay)
            {
                delay = Time.time + 0.1f;
                MatPlayer(GunLib.LockedPlayer.Creator);
            }
        }, true);
    }
    
    public static void MatAll()
    {
        if (NetworkSystem.Instance.InRoom && PhotonNetwork.IsMasterClient && Time.time > delay)
        {
            delay = Time.time + 0.1f;

            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (rig != null)
                    MatPlayer(rig.Creator);
            }
        }
    }
    
    private static HalloweenGhostChaser GetLucy()
    {
        if (lucy == null)
        {
            GameObject obj = GameObject.Find(
                "Environment Objects/05Maze_PersistentObjects/Ghosts/Halloween Ghost/FloatingChaseSkeleton");

            if (obj != null)
                lucy = obj.GetComponent<HalloweenGhostChaser>();
        }

        return lucy;
    }

    private static GameObject GetTerraformer()
    {
        if (terraformer == null)
        {
            terraformer = GameObject.Find(
                "Environment Objects/LocalObjects_Prefab/Forest/2026_Halloween_Forest/Terraformer");
        }

        return terraformer;
    }

    public static void EnableTerraformer()
    {
        GetTerraformer()?.SetActive(true);
    }

    public static void DisableTerraformer()
    {
        GetTerraformer()?.SetActive(false);
    }

    public static void SpawnLucy()
    {
        if (!Variables.IsMaster())
            return;

        HalloweenGhostChaser ghost = GetLucy();

        if (ghost == null || !ghost.IsMine)
            return;

        ghost.timeGongStarted = Time.time;
        ghost.currentState = HalloweenGhostChaser.ChaseState.Gong;
        ghost.isSummoned = false;
    }

    public static void LucyChaseGun()
    {
        GunLib.StartGun(() =>
        {
            HalloweenGhostChaser ghost = GetLucy();

            if (ghost == null || GunLib.LockedPlayer == null)
                return;

            if (!Variables.IsMaster(false) || !ghost.IsMine)
                return;

            ghost.currentState = HalloweenGhostChaser.ChaseState.Chasing;
            ghost.targetPlayer = GunLib.LockedPlayer.creator;
            ghost.followTarget = GunLib.LockedPlayer.head.rigTarget;
        }, true);
    }
    
    public static void LucyGrabGun()
    {
        GunLib.StartGun(() =>
        {
            HalloweenGhostChaser ghost = GetLucy();

            if (ghost == null || GunLib.LockedPlayer == null)
                return;

            if (!Variables.IsMaster(false) || !ghost.IsMine)
                return;

            ghost.isSummoned = true;
            ghost.targetPlayer = GunLib.LockedPlayer.creator;
            ghost.followTarget = GunLib.LockedPlayer.head.rigTarget;
            ghost.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
        }, true);
    }
    
    public static void SlowLucy()
    {
        HalloweenGhostChaser ghost = GetLucy();

        if (ghost == null || !ghost.IsMine || !Variables.IsMaster(false))
            return;

        ghost.currentSpeed = 1f;
        ghost.velocityStep = 0.25f;
        ghost.velocityIncreaseTime = 20f;
    }

    public static void FastLucy()
    {
        HalloweenGhostChaser ghost = GetLucy();

        if (ghost == null || !ghost.IsMine || !Variables.IsMaster(false))
            return;

        ghost.currentSpeed = 15f;
        ghost.velocityStep = 3f;
        ghost.velocityIncreaseTime = 5f;
    }

    public static void ResetLucySpeed()
    {
        HalloweenGhostChaser ghost = GetLucy();

        if (ghost == null || !ghost.IsMine || !Variables.IsMaster(false))
            return;

        ghost.currentSpeed = 3f;
        ghost.velocityStep = 1f;
        ghost.velocityIncreaseTime = 20f;
    }

    public static void FastBroomsticks()
    {
        if (!Variables.IsMaster())
            return;

        SetBroomstickSpeed(10f);
    }

    public static void SlowBroomsticks()
    {
        if (!Variables.IsMaster())
            return;

        SetBroomstickSpeed(60f);
    }

    public static void ResetBroomsticks()
    {
        if (!Variables.IsMaster())
            return;

        SetBroomstickSpeed(30f);
    }

    private static void SetBroomstickSpeed(float duration)
    {
        const string path =
            "Environment Objects/LocalObjects_Prefab/Forest/2026_Halloween_Forest/Broomsticks/";

        GameObject broomstick2 = GameObject.Find(path + "Broomstick2/NoncontrollableBroomstick");
        GameObject broomstick3 = GameObject.Find(path + "Broomstick3/NoncontrollableBroomstick");

        if (broomstick2 != null)
            broomstick2.GetComponent<NoncontrollableBroomstick>().duration = duration;

        if (broomstick3 != null)
            broomstick3.GetComponent<NoncontrollableBroomstick>().duration = duration;
    }
}