using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Master
{
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

    private static float delay;
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
    
}