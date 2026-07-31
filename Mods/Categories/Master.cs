using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    
    public static void UnlockRoom()
    {
        if (!PhotonNetwork.InRoom || !Variables.IsMaster())
            return;

        PhotonNetwork.CurrentRoom.IsVisible = true;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
    }

    public static void LockRoom()
    {
        if (!PhotonNetwork.InRoom || !Variables.IsMaster())
            return;
        
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
    }
    public static void SpazRoom()
    {
        if (!PhotonNetwork.InRoom || !Variables.IsMaster())
            return;
        
        for (int i = 0; i < 100; i++)
        {
            PhotonNetwork.CurrentRoom.IsVisible = (i % 2 == 0);
            PhotonNetwork.CurrentRoom.IsOpen = (i % 2 == 0);
        }
        GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
    }
    
}